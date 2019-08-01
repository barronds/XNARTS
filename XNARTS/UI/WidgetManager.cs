using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public partial class XUI
	{
		// assumed at this time that we would only want one global widget stack, as it is not ui practice to be able
		// to navigate two or more widgets at once.  also facilitates global access.

		// really, we have a screen widget, and on it we have any number of interactable widgets, and any one of them could 
		// spawn the widget stack.  

		// this class should own all the widgets that have been created.
		// there should be multiple collections of widgets so that they can be rendered or not 
		// or service input or not, or disbled or not on a whim efficiently.
		// ie, root widgets, gameplay widgets, menu widgets, whatever.

		// only widgets that are the root of a widget tree need to be in this class, as each widget
		// will know how to enable/disable, render/hide, and handle input for those it owns.

		public class WidgetManager
		{
			private List< XUI.Widget > mRootWidgets;

			public WidgetManager()
			{
				mRootWidgets = new List<XUI.Widget>();
			}

			public void AddRootWidget( Widget w )
			{
				Widget existing = mRootWidgets.Find( CompareWidgets( w ) );
				XUtils.Assert( existing == null );
				mRootWidgets.Add( w );
			}

			public void RemoveRootWidget( Widget w )
			{
				mRootWidgets.Remove( w );
			}
		}


		private WidgetManager mWidgetManager;

		private void Constructor_WidgetManager()
		{
			mWidgetManager = new WidgetManager();
		}

		// TODO: move this into widget class or nearby
		static Predicate<XUI.Widget> CompareWidgets( Widget w1 )
		{
			return delegate( XUI.Widget w2 )
			{
				return w1.GetUID() == w2.GetUID();
			};
		}
	}

}
