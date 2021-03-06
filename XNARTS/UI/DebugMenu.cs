﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
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
		private XUI._ISelector							mRootSelector;
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
			XBulletinBoard.Instance().mBroadcaster_FiveContacts.Subscribe( mListener_FiveContacts );
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
					mRootSelector = XUI.Instance().CreateSelector(	new XUI._Position(), "Debug Menu", 
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
							XBulletinBoard.Instance().mBroadcaster_ExitGameEvent.Post( new Game1.ExitGameEvent() );
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
