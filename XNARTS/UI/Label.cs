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
			private eFont   mFont;
			private String  mText;
			private Color   mColor;

			public Label( Widget parent, String text, eFont font, Color color )
			{
				mFont = font;
				mText = text;
				mColor = color;

				// calculate size of widget by text and init.
				XFontDraw.FontInfo info = XFontDraw.Instance().GetFontInfo( font );

				// position it at the origin of the parent for now, and it can be translated afterwards
				// when the creator sees how big it is via aabb
				xAABB2 aabb = new xAABB2( Vector2.Zero, new Vector2( info.mSize.X * text.Length, info.mSize.Y ) );
				InitWidget( parent, aabb );
			}

			//			public Label( Widget parent, ePlacement placement ) : base( parent, placement )
			//			{ }

			public override void Render( XSimpleDraw simple_draw )
			{
				XFontDraw.Instance().DrawString( mFont, GetPosition().GetAABB().GetMin(), mColor, mText );
			}
		}

	}
}
