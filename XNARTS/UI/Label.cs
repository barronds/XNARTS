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
			private Style	mStyle;
			private String  mText;

			public Label( Widget parent, String text, eStyle style, Vector2 pos )
			{
				// parent can query aabb afterwards and translate if necessary.  start at prescribed position.
				Vector2 label_size = Init( text, style );
				xAABB2 aabb = new xAABB2( pos, pos + label_size );
				InitWidget( parent, aabb );
			}

			public Label( Widget parent, String text, eStyle style, ePlacement placement )
			{
				Vector2 label_size = Init( text, style );
				InitWidget( parent, placement, label_size );
			}

			private Vector2 Init( String text, eStyle style )
			{
				mText = text;
				mStyle = XUI.Instance().GetStyle( style );

				// return size of text
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( mStyle.mNormalFont ).mSize;
				Vector2 label_size = new Vector2( font_size.X * text.Length, font_size.Y );
				return label_size;
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				XFontDraw.Instance().DrawString(	mStyle.mNormalFont, GetPosition().GetRelatveAABB().GetMin(), mStyle.mTextColor,
													mText );
			}
		}

	}
}
