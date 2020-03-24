using System;
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
		private XListener< XUI.ButtonUpEvent >			mListener_ButtonUp;
		private XBroadcaster< MenuSelectionEvent >      mBroadcaster_MenuSelection;
		private XUI.FullMenu							mRootSelector;
		private String[]                                mOptions;
		private String[]                                mControls;

		private XRootDebugMenu()
		{
			mListener_FiveContacts = new XListener<XTouch.FiveContacts>( 1, eEventQueueFullBehaviour.Assert, "5contacts" );
			mListener_ButtonUp = new XListener<XUI.ButtonUpEvent>( 1, eEventQueueFullBehaviour.Assert, "XUIRDMBU" );
			mBroadcaster_MenuSelection = new XBroadcaster<MenuSelectionEvent>();
			mRootSelector = null;
			String[] options = { "Map" };
			String[] controls = { "Exit", "Quit" };
			mOptions = options;
			mControls = controls;
		}

		public void Init()
		{
			XBulletinBoard.Instance().mBroadcaster_FiveContacts.Subscribe( mListener_FiveContacts );
			XUI.Instance().mBroadcaster_ButtonUpEvent.Subscribe( mListener_ButtonUp );
		}

		public void Update()
		{
			// check for create menu
			var enumerator_fiveContacts = mListener_FiveContacts.CreateEnumerator();
			XUI ui = XUI.Instance();

			if ( enumerator_fiveContacts.MoveNext() )
			{
				if( mRootSelector == null )
				{
					mRootSelector = ui.CreateFullMenu(	XUI.eStyle.FrontendTitle, "Debug Menu",
														XUI.eStyle.Frontend, mOptions,
														XUI.eStyle.Frontend, mControls,
														XUI.eStyle.Frontend, ui.GetScreenWidget(), XUI.ePlacement.Centered );
				}
			}

			// check for menu selection
			var selection_data = mListener_ButtonUp.GetMaxOne();

			if ( selection_data != null )
			{
				if( selection_data.mID == mRootSelector.GetUID() )
				{
					// destroy this selector
					XUI.Instance().DestroyFullMenu( mRootSelector );
					mRootSelector = null;

					int options_index = mRootSelector.GetOptionsInputIndex( selection_data.mID );
					int controls_index = mRootSelector.GetControlsInputIndex( selection_data.mID );

					if( controls_index > -1 )
					{
						switch ( controls_index )
						{
							case 0:
								// exit selected, do nothing, menu will close
								break;
							case 1:
								// quit selected, send message to end program.  this menu will close
								XBulletinBoard.Instance().mBroadcaster_ExitGameEvent.Post( new Game1.ExitGameEvent() );
								break;
							default:
								// problem
								XUtils.Assert( false );
								break;
						}
					}

					if( options_index > -1 )
					{
						switch( options_index )
						{
							case 0:
								// map selected, sent message for that system to do what it wants
								Console.WriteLine( "map selected" );
								long TODO_fix_up_output_id = 0;
								mBroadcaster_MenuSelection.Post( new MenuSelectionEvent( TODO_fix_up_output_id, mOptions[ 0 ] ) );
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
}
