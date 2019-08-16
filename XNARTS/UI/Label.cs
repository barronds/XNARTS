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
			private String  mText;

			// use static GetSizeOfText() if size is needed before construction

			public Label( Widget parent, String text, Style style, Vector2 pos )
			{
				// parent can query aabb afterwards and translate if necessary.  start at prescribed position.
				Vector2 label_size = Init( text, style );
				xAABB2 relative_aabb = new xAABB2( pos, pos + label_size );
				InitWidget( parent, style, relative_aabb );
			}

			public Label( Widget parent, String text, Style style, ePlacement placement )
			{
				Vector2 label_size = Init( text, style );
				InitWidget( parent, style, placement, label_size );
			}

			public static Vector2 GetSizeOfText( String text, Style style )
			{
				return CalcSize( text, style );
			}

			private Vector2 Init( String text, Style style )
			{
				mText = text;
				return CalcSize( text, style );
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
