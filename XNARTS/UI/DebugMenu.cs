using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	class XDebugMenu : XSingleton< XDebugMenu >
	{
		public class MenuSelectionEvent
		{
			// this design is not right. ambiguous for listening systems - could be multiple 
			// different menu items with same name from different menus.
			// maybe menu entries should be pushed from listener classes. (like realtime plugin)
			public MenuSelectionEvent( String selection_text )
			{
				mSelectionText = selection_text;
			}

			public String mSelectionText;
		}

		// just holds the root of the debug menu.  
		// individual systems should own the rest fo the tree.
		private XListener< XTouch.FiveContacts >		mListener_FiveContacts;
		private XListener< XUI.SelectorSelectionEvent > mListener_SelectorSelection;
		private XListener< XUI.SelectorControlEvent >   mListener_SelectorControl;
		private XBroadcaster< MenuSelectionEvent >      mBroadcaster_MenuSelection;
		private XUI.ISelector							mRootSelector;
		private String[]                                mOptions;
		private String[]                                mControls;

		private XDebugMenu()
		{
			mListener_FiveContacts = new XListener<XTouch.FiveContacts>( 1, eEventQueueFullBehaviour.Assert, "5contacts" );
			mListener_SelectorSelection = new XListener<XUI.SelectorSelectionEvent>( 1, eEventQueueFullBehaviour.Assert, "dmss" );
			mListener_SelectorControl = new XListener<XUI.SelectorControlEvent>( 1, eEventQueueFullBehaviour.Assert, "dmsc" );
			mBroadcaster_MenuSelection = new XBroadcaster<MenuSelectionEvent>();
			mRootSelector = null;
			mOptions = new String[ 1 ]{ "Map" };
			mControls = new String[ 1 ]{ "Exit" };
		}

		public void Init()
		{
			((XIBroadcaster<XTouch.FiveContacts>)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_FiveContacts );
			((XIBroadcaster<XUI.SelectorSelectionEvent>)XUI.Instance()).GetBroadcaster().Subscribe( mListener_SelectorSelection );
			((XIBroadcaster<XUI.SelectorControlEvent>)XUI.Instance()).GetBroadcaster().Subscribe( mListener_SelectorControl );
		}

		public void Update()
		{
			// check for create menu
			var enumerator_fiveContacts = mListener_FiveContacts.CreateEnumerator();

			if ( enumerator_fiveContacts.MoveNext() )
			{
				if( mRootSelector == null )
				{
					mRootSelector = XUI.Instance().CreateSelector( new Vector2( 500, 400 ), "Debug Menu", XUI.eStyle.Frontend,
																	XUI.eStyle.FrontendButton, XUI.eStyle.FrontendTitle,
																	XUI.eStyle.FrontendControl, mOptions, mControls );
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
							mBroadcaster_MenuSelection.Post( new MenuSelectionEvent( mOptions[ 0 ] ) );
							break;
						default:
							// problem
							break;
					}
				}
			}

			// check for control selection
			var control_data = mListener_SelectorControl.GetMaxOne();

			if( control_data != null )
			{
				if ( control_data.mSelectorID == mRootSelector.GetID() )
				{
					// destroy this selector
					XUI.Instance().DestroySelector( mRootSelector.GetID() );
					mRootSelector = null;

					switch ( control_data.mIndexSelected )
					{
						case 0:
							// exit selected, shut it down
							break;
						default:
							// problem
							break;
					}
				}
			}
		}
	}
}
