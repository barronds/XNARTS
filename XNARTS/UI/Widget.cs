using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public partial class XUI
	{
		// a widget has basic functionality:
		// - positionable
		// - has AABB
		// - can be enabled/disabled

		// and provides interface for
		// - rendering
		// - touch input
		// - updating

		// base class for concrete widgets
		// eg
		// button
		// symbol button
		// text button
		// image
		// label
		// panel (arrangement of other widgets only)
		// slider
		// text entry
		// radio buttons
		// check box
		// menu (arrangement of buttons)
		// titled menu (arrangement of label and menu)
		// draggable (usable in scope of panel?)
		// the screen? maybe super useful as parent of screen relative widgets.

		// every concrete widget should inherit from widget (is-a) and have sub widgets (has-a) if necessary.

		// examples:
		// panel inherits from widget and has-a list of widgets within it.


		public class Widget
		{
			public Widget()
			{ }


		}
	}
}
