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
		public class Spacer : Widget
		{
			public Spacer()
			{ }

			public void AssembleSpacer( Vector2 size )
			{
				AssembleWidget( size );
			}

			public void ReassembleSpacer( Vector2 size )
			{
				ReassembleWidget( size );
			}

			public void PlaceSpacer( Widget parent, Style style, UIPosSpec spec )
			{
				PlaceWidget( parent, style, spec );
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				base.Render( simple_draw );

				// nothing to draw, spaces are invisible.  default this to false.
				bool debug_draw = false;

				if( debug_draw )
				{
					XUI ui = XUI.Instance();
					xAABB2 aabb = GetPosition().GetScreenAABB();
					Style debug = ui.GetStyle( eStyle.FrontendDebug );
					ui.Util_DrawBox( simple_draw, debug.mBackgroundColor, debug.mBorderColor, aabb );
				}
			}
		}
	}
}
