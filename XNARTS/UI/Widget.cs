using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI
	{
		// a widget has basic functionality:
		// - positionable
		// - has AABB
		// - can be enabled/disabled

		// and provides interface for
		// - rendering
		// - touch input
		// - updating

		// base class for concrete widgets
		// eg
		// button
		// symbol button
		// text button
		// image
		// label
		// panel (arrangement of other widgets only)
		// slider
		// text entry
		// radio buttons
		// check box
		// menu (arrangement of buttons)
		// titled menu (arrangement of label and menu)
		// draggable (usable in scope of panel?)
		// the screen? maybe super useful as parent of screen relative widgets.

		// every concrete widget should inherit from widget (is-a) or have sub widgets (has-a) if necessary.

		// examples:
		// panel inherits from widget and has-a list of widgets within it.

		public class Widget
		{
			private UIPosition			mPosition;
			private Style				mStyle;
			private long				mUID;
			private eConstructionState	mConstructionState;
			private Vector2             mAssembledSize;

			// general usability/visibility state of the button
			private bool        mInputEnabled;	// if in focus and visible, can still be interactive or not
			private bool        mInFocus;		// can only be interacted with if it's in focus and visible
			private bool        mVisible;		// can still exist, but will also be not in focus so won't handle input if invisible
			
			// TODO: maybe this isn't right - maybe assemble means all elements exist are are assembled.
			// ie, when assemble is called, you assemble your children, recursively.
			// then the same for place.  will know more when i've implemented it for all the classes so far and used it.
			enum eConstructionState
			{
				Constructed,	// starts constructed
				Assembled,		// after Assemble() - all it's elements exist and are placed inside this widget
				Placed,			// this widget is placed in its parent
			}

			public Widget()
			{
				mUID = XUI.Instance().NextUID();
				SetUIState( true, false, false );
				SetConstructed();			
			}

			// convenient initial combinations of input enabled, visibility and focus.  more can be added if needed.
			// after initial state, call SetState() to change
			public enum eInitialState
			{
				Invalid = -1,

				Dormant,	// input enabled, but invisible and out of focus. (input will function when visible and brought to focus)
				Active,		// input enabled, visible, and in focus.

				Num
			}

			public void AssembleWidget( Vector2 size )
			{
				XUtils.Assert( mConstructionState == eConstructionState.Constructed );
				mAssembledSize = size;
				mConstructionState = eConstructionState.Assembled;
			}

			public void PlaceWidget( Widget parent, Style style, xAABB2 relative_aabb, eInitialState state )
			{
				PlaceWidgetCommon( style, state );
				mPosition = new UIPosition( parent, relative_aabb );
			}

			public void PlaceWidget( Widget parent, Style style, ePlacement placement, eInitialState state )
			{
				PlaceWidgetCommon( style, state );
				mPosition = new UIPosition( parent, placement, mAssembledSize );
			}

			public void Reparent( Widget new_parent, ePlacement placement )
			{
				XUtils.Assert( new_parent.IsPlaced() );
				mPosition = new UIPosition( new_parent, placement, mPosition.GetRelatveAABB().GetSize() );
			}

			public void Reparent( Widget new_parent, Vector2 pos )
			{
				XUtils.Assert( new_parent.IsPlaced() );
				mPosition = new UIPosition( new_parent, new xAABB2( pos, pos + mPosition.GetRelatveAABB().GetSize() ) );
			}

			public Vector2 GetAssembledSize()
			{
				return mAssembledSize;
			}

			public UIPosition GetPosition()
			{
				XUtils.Assert( IsPlaced() );
				return mPosition;
			}

			public Style GetStyle()
			{
				XUtils.Assert( IsPlaced() );
				return mStyle;
			}

			public long GetUID()
			{
				return mUID;
			}

			public bool IsInputEnabled()
			{
				return mInputEnabled;
			}

			public bool IsInFocus()
			{
				return mInFocus;
			}

			public bool IsVisible()
			{
				return mVisible;
			}

			public bool IsInteractable()
			{
				return mInputEnabled && mInFocus && mVisible;
			}

			public enum eInputChange
			{
				None,
				Enable,
				Disable
			}

			public enum eFocusChange
			{
				None,
				In,
				Out
			}

			public enum eVisibilityChange
			{
				None,
				Show,
				Hide
			}

			public virtual void SetState( eInputChange i, eFocusChange f, eVisibilityChange v )
			{
				// IsInteractable will assert on initialized so skip it here
				bool previously_interactable = IsInteractable();

				bool next_i = i == eInputChange.None ? mInputEnabled : i == eInputChange.Enable;
				bool next_f = f == eFocusChange.None ? mInFocus : f == eFocusChange.In;
				bool next_v = v == eVisibilityChange.None ? mVisible : v == eVisibilityChange.Show;

				// combinations				valid ?
				// disabled, out, hidden	y
				// disabled, out, visible	y
				// disabled, in, hidden,	n
				// disabled, in, visible	y
				// enabled, out, hidden		y	
				// enabled, out, visible	y
				// enabled, in, hidden,		n
				// enabled, in, visible		y

				if( next_f == true && next_v == false )
				{
					XUtils.Assert( false, "cannot have in focus widget that is invisible" );
				}				

				mInputEnabled = next_i;
				mInFocus = next_f;
				mVisible = next_v;

				bool currently_interactable = IsInteractable();
				
				if( currently_interactable != previously_interactable )
				{
					FacilitateInteractability( currently_interactable );
				}
			}

			public virtual void FacilitateInteractability( bool interactable )
			{
				// override this if there is something to do, like add to widget-subclass-specific input management collection.
				// only called when the interactability changes, so no need to check previous widget state or modify any.
				// interactable true means it's becoming interactable.  false means it is becoming non-interactable.
			}

			public bool Contains( Vector2 pos )
			{
				return mPosition.GetScreenAABB().Contains( pos );
			}

			public virtual void Render( XSimpleDraw simple_draw )
			{
				XUtils.Assert( IsPlaced() );
			}

			public static Predicate<Widget> CompareWidgets( Widget w1 )
			{
				return delegate ( Widget w2 )
				{
					return w1.GetUID() == w2.GetUID();
				};
			}

			private void SetConstructed()
			{
				mConstructionState = eConstructionState.Constructed;
			}

			private void SetPlaced()
			{
				XUtils.Assert( mConstructionState == eConstructionState.Assembled );
				mConstructionState = eConstructionState.Placed;
			}

			public bool IsConstructed()
			{
				return IsConstructionStateAtLeast( eConstructionState.Constructed );
			}

			public bool IsAssembled()
			{
				return IsConstructionStateAtLeast( eConstructionState.Assembled );
			}

			public bool IsPlaced()
			{
				return IsConstructionStateAtLeast( eConstructionState.Placed );
			}

			private bool IsConstructionStateAtLeast( eConstructionState s )
			{
				return ((int)mConstructionState) >= (int)s;
			}

			private void SetUIState( bool input_enabled, bool in_focus, bool visible )
			{
				mInputEnabled = input_enabled;
				mInFocus = in_focus;
				mVisible = visible;
			}

			private void PlaceWidgetCommon( Style style, eInitialState state )
			{
				XUtils.Assert( IsAssembled() ); // currently would allow re-placement
				mStyle = style;

				switch ( state )
				{
					case eInitialState.Dormant:
						SetState( eInputChange.Enable, eFocusChange.Out, eVisibilityChange.Hide );
						break;
					case eInitialState.Active:
						SetState( eInputChange.Enable, eFocusChange.In, eVisibilityChange.Show );
						break;
					default:
						XUtils.Assert( false );
						break;
				}

				SetPlaced();
			}
		}

		public class ScreenWidget : Widget
		{
			public ScreenWidget()
			{
				xCoord screen_dim = XRenderManager.Instance().GetScreenDim();
				Vector2 size = new Vector2( screen_dim.x, screen_dim.y );
				AssembleWidget( size );
				PlaceWidget( null, XUI.Instance().GetStyle( eStyle.Screen ), 
							new xAABB2( Vector2.Zero, size ), Widget.eInitialState.Dormant );
				SetState( eInputChange.Disable, eFocusChange.None, eVisibilityChange.None );
			}
		}

		private Widget mScreenWidget;

		private void Constructor_Widget()
		{
		}

		private void Init_Widget()
		{
			mScreenWidget = new ScreenWidget();
		}

		public Widget GetScreenWidget()
		{
			XUtils.Assert( mScreenWidget != null );
			return mScreenWidget;
		}
	}
}
