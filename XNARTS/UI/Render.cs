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
		private XSimpleDraw mSimpleDraw;

		private void Init_Render()
		{
			mSimpleDraw = XSimpleDraw.GetInstance( xeSimpleDrawType.ScreenSpace_Transient );
		}

		public void Draw()
		{
			Draw_Selector();	
			Draw_Buttons();
			Draw_Widgets();
		}

		public void Util_DrawBox( XSimpleDraw simple_draw, Style style, xAABB2 screen_aabb )
		{
			Vector3 lo_x_lo_y = new Vector3( screen_aabb.GetMin(), 0 );
			Vector3 hi_x_hi_y = new Vector3( screen_aabb.GetMax(), 0 );

			Vector2 size = screen_aabb.GetSize();
			Vector3 lo_x_hi_y = lo_x_lo_y + new Vector3( 0, size.Y, 0 );
			Vector3 hi_x_lo_y = lo_x_lo_y + new Vector3( size.X, 0, 0 );

			simple_draw.DrawQuad( lo_x_lo_y, hi_x_hi_y, style.mBackgroundColor );

			simple_draw.DrawLine( lo_x_lo_y, hi_x_lo_y, style.mBorderColor );
			simple_draw.DrawLine( hi_x_lo_y, hi_x_hi_y, style.mBorderColor );
			simple_draw.DrawLine( hi_x_hi_y, lo_x_hi_y, style.mBorderColor );
			simple_draw.DrawLine( lo_x_hi_y, lo_x_lo_y, style.mBorderColor );
		}

		private void Draw_Buttons()
		{
			var enumerator = mButtons.GetEnumerator();

			while ( enumerator.MoveNext() )
			{
				enumerator.Current.Value.Draw( mSimpleDraw );
			}
		}
		private void Draw_Selector()
		{
			var enumerator = mSelectors.GetEnumerator();

			while ( enumerator.MoveNext() )
			{
				enumerator.Current.Value.Draw( mSimpleDraw );
			}
		}
		private void Draw_Widgets()
		{
			for ( int i = 0; i < mRootWidgets.Count; ++i )
			{
				mRootWidgets[ i ].Render( mSimpleDraw );
			}
		}
	}
}
