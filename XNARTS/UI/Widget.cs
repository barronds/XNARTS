﻿using System;
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

		// every concrete widget should inherit from widget (is-a) and have sub widgets (has-a) if necessary.

		// examples:
		// panel inherits from widget and has-a list of widgets within it.


		public class Widget
		{
			private Position	mPosition;
			private bool		mInputEnabled;
			private bool        mInitialized;

			private static Widget sScreenWidget = null;

			public Widget()
			{
				mInitialized = false;
			}

//			public Widget( Widget parent, ePlacement placement )
//			{
//				mInputEnabled = true;
//				// maybe do the work here to figure out just where this is?
//				mPosition = new Position( parent, placement );
//			}

			public static Widget GetScreenWidget()
			{
				XUtils.Assert( sScreenWidget != null );
				return sScreenWidget;
			}

			public static void ClassInit()
			{
				sScreenWidget = new ScreenWidget();
			}

			public void InitWidget( Widget parent, xAABB2 aabb )
			{
				mInputEnabled = true;
				mPosition = new Position( parent, aabb );
				mInitialized = true;
			}

			public Position GetPosition()
			{
				XUtils.Assert( mInitialized );
				return mPosition;
			}

			public void SetInputEnabled( bool value )
			{
				XUtils.Assert( mInitialized );
				mInputEnabled = value;
			}
		}


		public class ScreenWidget : Widget
		{
			public ScreenWidget()
			{
				xCoord screen_dim = XRenderManager.Instance().GetScreenDim();
				InitWidget( null, new xAABB2( Vector2.Zero, new Vector2( screen_dim.x, screen_dim.y ) ) );
				SetInputEnabled( false );
			}
		}
	}
}
