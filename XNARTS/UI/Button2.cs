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
				// create a label, give it to the panel.  label's parent is panel, panel's parent is parent passed in here.
				// keep reference to the panel here, but give label to the panel.

				// label needs parent to construct, so create panel first.
				// two constructors, size and placement, or aabb directly.
				// aabb requires that we know the size of the label.
				// so we have a chicken and egg problem here, must move something out of a constructor.
				// actually i added a static getSize to be used before construction, that makes sense.

				Vector2 label_size = Label.GetSizeOfText( text, style );

				// ludicrous poke around for info to get button padding.  
				// especially since surrounding code also gets XUI, Style, Font Info, etc.
				// perhaps optimize later.
				eFont font = XUI.Instance().GetStyle( style ).mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				Style s = XUI.Instance().GetStyle( style );
				float padding = s.CalcButtonPadding( font_size );
				xAABB2 aabb = new xAABB2( pos, pos + label_size + new Vector2( padding, padding ) );

				mPanel = new Panel( parent, style, aabb );
				Label label = new Label( mPanel, text, style, pos );

				mPanel.AddChild( label );
			}

			public Button( Widget parent, eStyle style, String text, ePlacement placement )
			{
				
			}
		}
	}
}
