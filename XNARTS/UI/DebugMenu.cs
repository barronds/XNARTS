using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

	class XRootDebugMenu : XSingleton< XRootDebugMenu >
	{
		public class MenuSelectionEvent
		{
			// this design is not right. ambiguous for listening systems - could be multiple 
			// different menu items with same name from different menus.
			// maybe menu entries should be pushed from listener classes. (like realtime plugin)
			public MenuSelectionEvent( long selector_uid, String selection )
			{
				mSelectorUID = selector_uid;
				mSelection = selection;
			}

			public long		mSelectorUID;
			public String	mSelection;
		}

		// just holds the root of the debug menu.  
		// individual systems should own the rest of the tree.
		private XListener< XTouch.FiveContacts >		mListener_FiveContacts;
		private XListener< XUI.SelectorSelectionEvent > mListener_SelectorSelection;
		private XBroadcaster< MenuSelectionEvent >      mBroadcaster_MenuSelection;
		private XUI.ISelector							mRootSelector;
		private String[]                                mOptions;

		private XRootDebugMenu()
		{
			mListener_FiveContacts = new XListener<XTouch.FiveContacts>( 1, eEventQueueFullBehaviour.Assert, "5contacts" );
			mListener_SelectorSelection = new XListener<XUI.SelectorSelectionEvent>( 1, eEventQueueFullBehaviour.Assert, "dmss" );
			mBroadcaster_MenuSelection = new XBroadcaster<MenuSelectionEvent>();
			mRootSelector = null;
			String spacer = XUI.Instance().GetSpacerString();

			mOptions = new String[ 5 ]{ "Map", spacer, "Exit", spacer, "Quit" };
		}

		public void Init()
		{
			XTouch.Instance().GetBroadcaster_FiveContacts().Subscribe( mListener_FiveContacts );
			XUI.Instance().GetBroadcaster_SelectorSelectionEvent().Subscribe( mListener_SelectorSelection );
		}

		public void Update()
		{
			// check for create menu
			var enumerator_fiveContacts = mListener_FiveContacts.CreateEnumerator();

			if ( enumerator_fiveContacts.MoveNext() )
			{
				if( mRootSelector == null )
				{
					mRootSelector = XUI.Instance().CreateSelector(	new XUI.Position(), "Debug Menu", 
																	XUI.eStyle.Frontend, XUI.eStyle.FrontendButton, 
																	XUI.eStyle.FrontendTitle, mOptions );
				}
			}

			// check for menu selection
			var selection_data = mListener_SelectorSelection.GetMaxOne();

			if ( selection_data != null )
			{
				if( selection_data.mSelectorID == mRootSelector.GetID() )
				{
					// destroy this selector
					XUI.Instance().DestroySelector( mRootSelector.GetID() );
					mRootSelector = null;

					switch( selection_data.mIndexSelected )
					{
						case 0:
							// map selected, sent message for that system to do what it wants
							Console.WriteLine( "map selected" );
							mBroadcaster_MenuSelection.Post( new MenuSelectionEvent( selection_data.mSelectorID, mOptions[ 0 ] ) );
							break;
						case 2:
							// exit selected, do nothing, menu will close
							break;
						case 4:
							// quit selected, send message to end program.  this menu will close
							BulletinBoard.Instance().mBroadcaster_ExitGameEvent.Post( new Game1.ExitGameEvent() );
							break;
						default:
							// problem
							XUtils.Assert( false );
							break;
					}
				}
			}
		}
	}
}
