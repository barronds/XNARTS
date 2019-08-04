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
					//Test_Label();
					Test_Panel();
				}
			}

			private void Test_Label()
			{
				XUI.Label label_1 = new XUI.Label(	XUI.Instance().GetScreenWidget(), "Test Widget 1", eStyle.Frontend, 
													new Vector2( 500, 700 ) );
				XUI.Instance().AddRootWidget( label_1 );

				XUI.Label label_2 = new XUI.Label(  XUI.Instance().GetScreenWidget(), "Test Widget 2", eStyle.Frontend, 
													ePlacement.Centered );
				XUI.Instance().AddRootWidget( label_2 );
			}

			private void Test_Panel()
			{
				XUI.Panel panel_1 = new XUI.Panel(  XUI.Instance().GetScreenWidget(), eStyle.Frontend,
													new xAABB2( new Vector2( 400, 400 ), new Vector2( 700, 500 ) ) );
				XUI.Instance().AddRootWidget( panel_1 );
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
