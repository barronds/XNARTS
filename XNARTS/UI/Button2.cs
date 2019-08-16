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
		public class Button : Panel
		{
			public Button( Widget parent, Style style, String text, Vector2 pos )
			{
				// optimize later
				Vector2 label_size = Label.GetSizeOfText( text, style );
				eFont font = style.mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				float padding = style.mButtonPadding;
				xAABB2 aabb = new xAABB2( pos, pos + label_size + 2.0f * new Vector2( padding, padding ) );
				InitPanel( parent, style, aabb );

				Label label = new Label( this, text, style, ePlacement.Centered );
				AddChild( label );
			}

			public Button( Widget parent, Style style, String text, ePlacement placement )
			{
				// optimize later
				Vector2 label_size = Label.GetSizeOfText( text, style );
				eFont font = style.mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				float padding = style.mButtonPadding;
				Vector2 size = label_size + 2.0f * new Vector2( padding, padding );
				InitPanel( parent, style, size, placement );

				Label label = new Label( this, text, style, ePlacement.Centered );
				AddChild( label );
			}
		}
	}
}
