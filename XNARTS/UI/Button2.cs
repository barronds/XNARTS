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
		// trying to make a button as a label on a panel.  the panel would handle the input
		// and hold the label as a child.

		// imagining this as a root widget, it's parent would be the screen widget.

		// parent is only used for placement and there is no placement for the button, only the panel.


		public class Button : Widget
		{
			private Panel mPanel;

			public Button( Widget parent, eStyle style, String text, Vector2 pos )
			{
				// optimize later
				Vector2 label_size = Label.GetSizeOfText( text, style );
				eFont font = XUI.Instance().GetStyle( style ).mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				Style s = XUI.Instance().GetStyle( style );
				float padding = s.CalcButtonPadding( font_size );
				xAABB2 aabb = new xAABB2( pos, pos + label_size + 2.0f * new Vector2( padding, padding ) );

				// make sure this widget is initialized before creating children
				InitWidget( parent, aabb );

				mPanel = new Panel( parent, style, aabb );
				Label label = new Label( mPanel, text, style, ePlacement.Centered );

				mPanel.AddChild( label );
			}

			public Button( Widget parent, eStyle style, String text, ePlacement placement )
			{
				// optimize later
				Vector2 label_size = Label.GetSizeOfText( text, style );
				eFont font = XUI.Instance().GetStyle( style ).mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				Style s = XUI.Instance().GetStyle( style );
				float padding = s.CalcButtonPadding( font_size );
				Vector2 size = label_size + 2.0f * new Vector2( padding, padding );

				// make sure this widget is initialized before creating children
				InitWidget( parent, placement, size );

				mPanel = new Panel( parent, style, size, placement );
				Label label = new Label( mPanel, text, style, ePlacement.Centered );

				mPanel.AddChild( label );
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				base.Render( simple_draw );
				mPanel.Render( simple_draw );
			}
		}


		public class ButtonAsPanel : Panel
		{
			// this class is because i am not liking that button inherits from widget but it doesn't have a 
			// graphical representation of its own.  ie, it owns a panel which does, and also has an aabb.
			// so keeping two aabbs in sync in weird.  this is a test.

			public ButtonAsPanel( Widget parent, eStyle style, String text, Vector2 pos )
			{
				// optimize later
				Vector2 label_size = Label.GetSizeOfText( text, style );
				eFont font = XUI.Instance().GetStyle( style ).mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				Style s = XUI.Instance().GetStyle( style );
				float padding = s.CalcButtonPadding( font_size );
				xAABB2 aabb = new xAABB2( pos, pos + label_size + 2.0f * new Vector2( padding, padding ) );
				InitPanel( parent, style, aabb );

				Label label = new Label( this, text, style, ePlacement.Centered );
				AddChild( label );
			}

			public ButtonAsPanel( Widget parent, eStyle style, String text, ePlacement placement )
			{
				// optimize later
				Vector2 label_size = Label.GetSizeOfText( text, style );
				eFont font = XUI.Instance().GetStyle( style ).mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				Style s = XUI.Instance().GetStyle( style );
				float padding = s.CalcButtonPadding( font_size );
				Vector2 size = label_size + 2.0f * new Vector2( padding, padding );
				InitPanel( parent, style, size, placement );

				Label label = new Label( this, text, style, ePlacement.Centered );
				AddChild( label );
			}
		}
	}
}
