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
		// alternate design, in progress: (outdated, leaving it here until i revise it)
		//
		// - each menu is an XMenu object, (which is a widget maybe)
		// - you can add, delete, grey out menu options dynamically
		//
		// - once selected, an entry can create another menu, another widget, go back one menu, quit menu, or have a side effect
		// - once side effect is chosen, the entry can stay in current menu, go back one menu, or quit menu
		// - if another widget is used and closed, it can stay in current menu or quit menu
		// - if a widget spawns another menu, ... (maybe here we change menu stack to widget stack) 
		//
		// - there is one global 'active menu stack'. only the top menu is visible at any moment. 
		//   (one rendering choice, could be others).  should be able to have other buttons on screen
		//	 that would activate other menu stacks if in a current one.  that would exit any current one.
		//
		// - menus in the middle of the stack can still be accessed for option modification on the fly
		// - each menu has a UID on creation, which changes if the menu is created again.
		// - there is only one event for listenting to menu selections, so many listeners per option selected, unfortunate.
		//   optimization there could be to have systems unsubscribe/subscribe for certain menu creations/destructions on the fly.
		//   to support this, each menu created could have a name, and event fired globally when a menu is created/destroyed.
		// - menu selection events should have the UID of the Menu object and the string for the button selected, rather than 
		//   UID or index of button because of dynamic addition/removal.  strings should be unique otherwise there would be 
		//   an ambiguity visually in the menu.

/*
		public class Menu_TitleEntriesControls : VerticalStack
		{
			// menu is a vertical stack, "layout"
			// the layout holds in it a title, and 2 vertical stacks, "entries" and "controls".
			// this class doesn't need to hold anything.  layout owns the 3.  entries and controls owns the buttons.
						
			public Menu_TitleEntriesControls()
			{ }

			// maybe return the binding here of uids to entries so the client has a chance to map inputs.
			// thought 2: instead just have menu selection event with an enum value that says entry or control,
			// then also return the index of that array, and even maybe the string.
			public void InitMenu(	Widget parent, Style layout_style, Style title_style, Style entry_style, Style control_style, 
									String title, String[] entries, String[] controls, ePlacement placement, 
									eInitialState state )
			{
				Widget[] layout_widgets = Init( parent, layout_style, title_style, entry_style, control_style, title, entries, controls, state );		
				InitVerticalStack( parent, layout_widgets, layout_style, placement, state );

				for( int i = 0; i < layout_widgets.Count(); ++i )
				{
					//layout_widgets[ i ].Reparent( this, )
				}
			}

			public void InitMenu(	Widget parent, Style layout_style, Style title_style, Style entry_style, Style control_style,
									String title, String[] entries, String[] controls, Vector2 pos, eInitialState state )
			{

			}

			private Widget[] Init(	Widget parent, Style layout_style, Style title_style, Style entry_style, Style control_style,
									String title, String[] entries, String[] controls, eInitialState state )
			{
				XUtils.Assert( title != null && entries.Count() > 0 && controls.Count() > 0 );
				VerticalStack entries_stack = new VerticalStack();
				VerticalStack controls_stack = new VerticalStack();

				//Label title_label = new Label( parent, title, title_style, Vector2.Zero, state );
				Label title_label = new Label();
				title_label.Assemble( title_style, title );
				title_label.Place( parent, title_style, Vector2.Zero, state );

				Button[] entry_buttons = new Button[ entries.Count() ];
				Button[] control_buttons = new Button[ controls.Count() ];

				for ( int i = 0; i < entries.Count(); ++i )
				{
					Button b = new Button( parent, entry_style, entries[ i ], Vector2.Zero, state );
					entry_buttons.SetValue( b, i );
				}

				for ( int i = 0; i < controls.Count(); ++i )
				{
					Button b = new Button( parent, control_style, controls[ i ], Vector2.Zero, state );
					control_buttons.SetValue( b, i );
				}

				entries_stack.InitVerticalStack( parent, entry_buttons, entry_style, Vector2.Zero, state );
				controls_stack.InitVerticalStack( parent, control_buttons, control_style, Vector2.Zero, state );
				Widget[] layout_widgets = { title_label, entries_stack, controls_stack };
				return layout_widgets;
			}
		}
		*/
	}
}
