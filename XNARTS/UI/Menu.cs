using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// alternate design, in progress:
	//
	// - each menu is an XMenu object
	// - you can add, delete, grey out menu options dynamically
	// - once selected, an entry can create another menu, another widget, go back one menu, quit menu, or have a side effect
	// - once side effect is chosen, the entry can stay in current menu, go back one menu, or quite menu
	// - there is one global 'active menu stack'. only the top menu is visible at any moment. (one rendering choice, could be others)
	// - menus in the middle of the stack can still be accessed for option modification on the fly
	// - each menu has a UID on creation, which changes if the menu is created again.
	// - there is only one event for listenting to menu selections, so many listeners per option selected, unfortunate.
	//   optimization there could be to have systems unsubscribe/subscribe for certain menu creations/destructions on the fly.
	//   to support this, each menu created could have a name, and event fired globally when a menu is created/destroyed.
	// - menu selection events should have the UID of the Menu object and the string for the button selected, rather than 
	//   UID or index of button because of dynamic addition/removal.  strings should be unique otherwise there would be 
	//   an ambiguity visually in the menu.

	public class Menu
	{
	}
}
