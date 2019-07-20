using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// event types used by more that one system can live in here

	public class BulletinBoard : XSingleton< BulletinBoard >
	{
		public XBroadcaster< Game1.ExitGameEvent > mBroadcaster_ExitGameEvent;

		public void Init()
		{
			mBroadcaster_ExitGameEvent = new XBroadcaster<Game1.ExitGameEvent>();
		}

		private BulletinBoard()
		{
			// private constructor as per XSingleton
		}
	}
}
