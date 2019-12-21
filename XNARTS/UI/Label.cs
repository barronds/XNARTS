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

			// use static GetSizeOfText() if size is needed before construction
			//public Label( Widget parent, String text, Style style, Vector2 pos, eInitialState state )
			//{
			//	// parent can query aabb afterwards and translate if necessary.  start at prescribed position.
			//	Vector2 label_size = Init( text, style );
			//	xAABB2 relative_aabb = new xAABB2( pos, pos + label_size );
			//	PlaceWidget( parent, style, relative_aabb, state );
			//}

			//public Label( Widget parent, String text, Style style, ePlacement placement, eInitialState state )
			//{
			//	Vector2 label_size = Init( text, style );
			//	PlaceWidget( parent, style, placement, label_size, state );
			//}

			public Label()
			{
				mSize = Vector2.Zero;
				SetConstructed();
			}

			public void Asssemble( Style style, String text )
			{
				mSize = CalcSize( text, style );
				mText = text;
				SetAssembled();
			}

			public void Place( Widget parent, Style style, ePlacement placement, eInitialState state )
			{
				PlaceWidget( parent, style, placement, mSize, state );
				SetPlaced();
			}

			public void Place( Widget parent, Style style, Vector2 pos, eInitialState state )
			{
				xAABB2 relative_aabb = new xAABB2( pos, pos + mSize );
				PlaceWidget( parent, style, relative_aabb, state );
				SetPlaced();
			}

			//public static Vector2 GetSizeOfText( String text, Style style )
			//{
			//	return CalcSize( text, style );
			//}

			public Vector2 GetSize()
			{
				XUtils.Assert( IsAssembled() );
				return mSize;
			}

			//private Vector2 Init( String text, Style style )
			//{
			//	mText = text;
			//	return CalcSize( text, style );
			//}

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
