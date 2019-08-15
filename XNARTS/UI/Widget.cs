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
			private bool        mInputEnabled;
			private bool        mInitialized;

			public Widget()
			{
				mUID = XUI.Instance().NextUID();
				mInitialized = false;
			}

			public void InitWidget( Widget parent, Style style, xAABB2 aabb )
			{
				InitWidgetCommon( style );
				mPosition = new Position( parent, aabb );
			}

			public void InitWidget( Widget parent, Style style, ePlacement placement, Vector2 size )
			{
				InitWidgetCommon( style );
				mPosition = new Position( parent, placement, size );
			}

			private void InitWidgetCommon( Style style )
			{
				XUtils.Assert( !mInitialized );
				mStyle = style;
				mInputEnabled = true;
				mInitialized = true;
			}

			public Position GetPosition()
			{
				XUtils.Assert( mInitialized );
				return mPosition;
			}

			public Style GetStyle()
			{
				return mStyle;
			}

			public long GetUID()
			{
				return mUID;
			}

			public void SetInputEnabled( bool value )
			{
				XUtils.Assert( mInitialized );
				mInputEnabled = value;
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
		}

		public class ScreenWidget : Widget
		{
			public ScreenWidget()
			{
				xCoord screen_dim = XRenderManager.Instance().GetScreenDim();
				InitWidget( null, XUI.Instance().GetStyle( eStyle.Screen ), 
							new xAABB2( Vector2.Zero, new Vector2( screen_dim.x, screen_dim.y ) ) );
				SetInputEnabled( false );
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
