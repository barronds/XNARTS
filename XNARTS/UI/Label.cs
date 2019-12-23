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
			private Vector2	mSize;

			public Label()
			{
				mSize = Vector2.Zero;
			}

			public void Asssemble( Style style, String text )
			{
				mSize = CalcSize( text, style );
				mText = text;
				AssembleWidget();
			}

			public void Place( Widget parent, Style style, ePlacement placement, eInitialState state )
			{
				PlaceWidget( parent, style, placement, mSize, state );
			}

			public void Place( Widget parent, Style style, Vector2 pos, eInitialState state )
			{
				xAABB2 relative_aabb = new xAABB2( pos, pos + mSize );
				PlaceWidget( parent, style, relative_aabb, state );
			}

			public Vector2 GetSize()
			{
				XUtils.Assert( IsAssembled() );
				return mSize;
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
