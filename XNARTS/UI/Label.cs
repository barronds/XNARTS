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
		public class Label : Widget
		{
			private String	mText;

			public Label()
			{
			}

			public void AssembleLabel( Style style, String text )
			{
				Vector2 size = CalcSize( text, style );
				mText = text;
				AssembleWidget( size );
			}

			public void PlaceLabel( Widget parent, Style style, ePlacement placement, eInitialState state )
			{
				PlaceWidget( parent, style, placement, state );
			}

			public void PlaceLabel( Widget parent, Style style, Vector2 pos, eInitialState state )
			{
				xAABB2 relative_aabb = new xAABB2( pos, pos + GetAssembledSize() );
				PlaceWidget( parent, style, relative_aabb, state );
			}

			private static Vector2 CalcSize( String text, Style style )
			{
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( style.mNormalFont ).mSize;
				Vector2 label_size = new Vector2( font_size.X * text.Length, font_size.Y );
				return label_size;
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				base.Render( simple_draw );
				XFontDraw.Instance().DrawString(	GetStyle().mNormalFont, GetPosition().GetScreenAABB().GetMin(), 
													GetStyle().mTextColor, mText );
			}
		}

	}
}
