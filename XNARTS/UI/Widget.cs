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
			private Position    mPosition;
			private Style       mStyle;
			private long        mUID;
			private bool        mInitialized;

			// general usability/visibility state of the button
			private bool        mInputEnabled;	// if in focus and visible, can still be interactive or not
			private bool        mInFocus;		// can only be interacted with if it's in focus and visible
			private bool        mVisible;		// can still exist, but will also be not in focus so won't handle input if invisible
			
			public Widget()
			{
				mUID = XUI.Instance().NextUID();
				SetState( true, false, false );
				mInitialized = false;
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

			public void InitWidget( Widget parent, Style style, xAABB2 relative_aabb, eInitialState state )
			{
				InitWidgetCommon( style, state );
				mPosition = new Position( parent, relative_aabb );
			}

			public void InitWidget( Widget parent, Style style, ePlacement placement, Vector2 size, eInitialState state )
			{
				InitWidgetCommon( style, state );
				mPosition = new Position( parent, placement, size );
			}

			private void InitWidgetCommon( Style style, eInitialState state )
			{
				XUtils.Assert( !mInitialized );
				mStyle = style;

				switch( state )
				{
					case eInitialState.Dormant:		SetState( true, false, false );		break;
					case eInitialState.Active:		SetState( true, true, true );		break;
					default:						XUtils.Assert( false );				break;
				}

				mInitialized = true;
			}

			public Position GetPosition()
			{
				XUtils.Assert( mInitialized );
				return mPosition;
			}

			public Style GetStyle()
			{
				XUtils.Assert( mInitialized );
				return mStyle;
			}

			public long GetUID()
			{
				XUtils.Assert( mInitialized );
				return mUID;
			}

			public bool IsInputEnabled()
			{
				XUtils.Assert( mInitialized );
				return mInputEnabled;
			}

			public bool IsInFocus()
			{
				XUtils.Assert( mInitialized );
				return mInFocus;
			}

			public bool IsVisible()
			{
				XUtils.Assert( mInitialized );
				return mVisible;
			}

			public bool IsInteractable()
			{
				XUtils.Assert( mInitialized );
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
				XUtils.Assert( mInitialized );
			}

			public static Predicate<Widget> CompareWidgets( Widget w1 )
			{
				return delegate ( Widget w2 )
				{
					return w1.GetUID() == w2.GetUID();
				};
			}

			private void SetState( bool input_enabled, bool in_focus, bool visible )
			{
				mInputEnabled = input_enabled;
				mInFocus = in_focus;
				mVisible = visible;
			}
		}

		public class ScreenWidget : Widget
		{
			public ScreenWidget()
			{
				xCoord screen_dim = XRenderManager.Instance().GetScreenDim();
				InitWidget( null, XUI.Instance().GetStyle( eStyle.Screen ), 
							new xAABB2( Vector2.Zero, new Vector2( screen_dim.x, screen_dim.y ) ), Widget.eInitialState.Dormant );
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
