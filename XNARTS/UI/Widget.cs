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
			
			// assemble children before self.  place self before children.
			// 'assemble up, place down'
			enum eConstructionState
			{
				Constructed,	// starts constructed
				Assembled,		// after Assemble() - all it's elements exist and are placed inside this widget
				Placed,			// this widget is placed in its parent
			}

			public Widget()
			{
				mUID = XUI.Instance().NextUID();
				SetConstructed();			
			}

			public void AssembleWidget( Vector2 size )
			{
				XUtils.Assert( mConstructionState == eConstructionState.Constructed );
				mAssembledSize = size;
				mConstructionState = eConstructionState.Assembled;
			}

			public void PlaceWidget( Widget parent, Style style, UIPosSpec spec )
			{
				PlaceWidgetCommon( style );
				mPosition = new UIPosition( parent, spec );
			}

			public void Reparent( Widget new_parent, ePlacement placement )
			{
				XUtils.Assert( new_parent.IsPlaced() );
				UIPosSpec spec = new UIPosSpec( placement, mPosition.GetRelatveAABB().GetSize() );
				mPosition = new UIPosition( new_parent, spec );
			}

			public void Reparent( Widget new_parent, Vector2 pos )
			{
				XUtils.Assert( new_parent.IsPlaced() );
				UIPosSpec spec = new UIPosSpec( new xAABB2( pos, pos + mPosition.GetRelatveAABB().GetSize() ) );
				mPosition = new UIPosition( new_parent, spec );
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

			private void PlaceWidgetCommon( Style style )
			{
				XUtils.Assert( IsAssembled() ); // currently would allow re-placement
				mStyle = style;
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
				UIPosSpec spec = new UIPosSpec( new xAABB2( Vector2.Zero, size ) );
				PlaceWidget( null, XUI.Instance().GetStyle( eStyle.Screen ), spec );
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
