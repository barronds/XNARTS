using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public class TestBed : XSingleton< TestBed >
	{
		private XListener< XTouch.FourContacts > mListener_FourContacts;

		public void Init()
		{
			mListener_FourContacts = new XListener<XTouch.FourContacts>( 1, eEventQueueFullBehaviour.Ignore, "XUITB4C" );
			XBulletinBoard.Instance().mBroadcaster_FourContacts.Subscribe( mListener_FourContacts );
		}

		public void Update( GameTime game_time )
		{
			if( mListener_FourContacts.GetMaxOne() != null )
			{
				Test_Label();
			}
		}

		private void Test_Label()
		{
			XUI.Label label = new XUI.Label( XUI.Widget.GetScreenWidget(), "Test Widget", eFont.Consolas24 );
		}

		private TestBed()
		{
			// private constructor as per XSingleton
		}
	}
}
