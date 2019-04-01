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
		// just holds the root of the debug menu.  
		// individual systems should own the rest fo the tree.
		private XListener< XTouch.FiveContacts >	mListener_FiveContacts;
		private XUI.ISelector                       mRootSelector;

		private XDebugMenu()
		{
			mListener_FiveContacts = new XListener<XTouch.FiveContacts>( 1, eEventQueueFullBehaviour.Assert, "5contacts" );
			mRootSelector = null;
		}

		public void Init()
		{
			((XIBroadcaster<XTouch.FiveContacts>)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_FiveContacts );
		}

		public void Update()
		{
			var enumerator = mListener_FiveContacts.GetEnumerator();
			XTouch.FiveContacts e = null;

			while( (e = enumerator.MoveNext()) != null )
			{
				// intentionally iterate through no matter what to drain listener queue
				if( mRootSelector == null )
				{
					String[] options = { "Map" };
					String[] controls = { "Exit" };

					mRootSelector = XUI.Instance().CreateSelector( new Vector2( 500, 400 ), "Debug Menu", XUI.eStyle.Frontend,
																	XUI.eStyle.FrontendButton, XUI.eStyle.FrontendTitle,
																	XUI.eStyle.FrontendControl, options, controls );
				}


			}
		}
	}
}
