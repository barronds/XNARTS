using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public class ExitGameEvent
	{
		public ExitGameEvent() { }
	}

	public class BulletinBoard : XSingleton< BulletinBoard >
	{
		public XBroadcaster< ExitGameEvent > mBroadcaster_ExitGameEvent;

		public void Init()
		{
			mBroadcaster_ExitGameEvent = new XBroadcaster<ExitGameEvent>();
		}

		private BulletinBoard()
		{
			// private constructor as per XSingleton
		}
	}
}
