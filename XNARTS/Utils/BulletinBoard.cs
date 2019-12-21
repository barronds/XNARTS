using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// event types used by more that one system can live in here

	public class XBulletinBoard : XSingleton< XBulletinBoard >
	{
		public XBroadcaster< Game1.ExitGameEvent >		mBroadcaster_ExitGameEvent;

		public XBroadcaster< XTouch.MultiDragData >     mBroadcaster_MultiDrag;
		public XBroadcaster< XTouch.SinglePokeData >	mBroadcaster_SinglePoke;
		public XBroadcaster< XTouch.FiveContacts >		mBroadcaster_FiveContacts;
		public XBroadcaster< XTouch.FourContacts >      mBroadcaster_FourContacts;

		public XBroadcaster< XKeyInput.KeyDown >    mBroadcaster_KeyDown;
		public XBroadcaster< XKeyInput.KeyUp >      mBroadcaster_KeyUp;
		public XBroadcaster< XKeyInput.KeyHeld >    mBroadcaster_KeyHeld;

		public void Init()
		{
			mBroadcaster_ExitGameEvent = new XBroadcaster<Game1.ExitGameEvent>();

			mBroadcaster_MultiDrag = new XBroadcaster<XTouch.MultiDragData>();
			mBroadcaster_SinglePoke = new XBroadcaster<XTouch.SinglePokeData>();
			mBroadcaster_FourContacts = new XBroadcaster<XTouch.FourContacts>();
			mBroadcaster_FiveContacts = new XBroadcaster<XTouch.FiveContacts>();

			mBroadcaster_KeyDown = new XBroadcaster<XKeyInput.KeyDown>();
			mBroadcaster_KeyUp = new XBroadcaster<XKeyInput.KeyUp>();
			mBroadcaster_KeyHeld = new XBroadcaster<XKeyInput.KeyHeld>();
		}

		private XBulletinBoard()
		{
			// private constructor as per XSingleton
		}
	}
}
