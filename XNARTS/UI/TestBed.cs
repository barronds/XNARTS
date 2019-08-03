using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI
	{
		public class TestBed
		{
			private XListener< XTouch.FourContacts >	mListener_FourContacts;
			private int                                 mTestTriggerCount;

			public TestBed()
			{
				mTestTriggerCount = 0;
			}

			public void Init()
			{
				mListener_FourContacts = new XListener<XTouch.FourContacts>( 1, eEventQueueFullBehaviour.Ignore, "XUITB4C" );
				XBulletinBoard.Instance().mBroadcaster_FourContacts.Subscribe( mListener_FourContacts );
			}

			public void Update( GameTime game_time )
			{
				if ( mTestTriggerCount == 0 && mListener_FourContacts.GetMaxOne() != null )
				{
					++mTestTriggerCount;
					Test_Label();
				}
			}

			private void Test_Label()
			{
				XUI.Label label = new XUI.Label( XUI.Instance().GetScreenWidget(), "Test Widget", eFont.Consolas24, Color.Black );
				XUI.Instance().AddRootWidget( label );
			}
		}

		private TestBed mTestBed;

		private void Constructor_TestBed()
		{
			mTestBed = new TestBed();
		}

		private void Init_TestBed()
		{
			mTestBed.Init();
		}

		private void Update_TestBed( GameTime t )
		{
			mTestBed.Update( t );
		}
	}
}
