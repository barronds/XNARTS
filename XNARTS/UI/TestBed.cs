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
					//Test_Button();
				}
			}

			private void Test_Label()
			{
				XUI ui = XUI.Instance();
				XUI.Label label_1 = new XUI.Label(	ui.GetScreenWidget(), "Test Widget 1", ui.GetStyle( eStyle.Frontend ), 
													new Vector2( 500, 700 ) );
				ui.AddRootWidget( label_1 );

				XUI.Label label_2 = new XUI.Label(  ui.GetScreenWidget(), "Test Widget 2", ui.GetStyle( eStyle.Frontend ), 
													ePlacement.Centered );
				ui.AddRootWidget( label_2 );
			}

			private void Test_Panel()
			{
				XUI ui = XUI.Instance();
				XUI.Panel panel_1 = new XUI.Panel(  ui.GetScreenWidget(),  ui.GetStyle( eStyle.Frontend ),
													new xAABB2( new Vector2( 200, 600 ), new Vector2( 1500, 1100 ) ) );

				XUI.Panel panel_2 = new XUI.Panel(	panel_1, ui.GetStyle( eStyle.Frontend ),
													new xAABB2( new Vector2( 10, 20 ), new Vector2( 100, 40 ) ) );

				XUI.Panel panel_3 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.Frontend ), new Vector2( 200, 300 ),
													ePlacement.Centered );

				//panel_1.AddChild( panel_2 );
				panel_1.AddChild( panel_3 );
				ui.AddRootWidget( panel_1 );
			}

			private void Test_Button()
			{
				XUI ui = XUI.Instance();
				XUI.Button bap_1 = new XUI.Button(	ui.GetScreenWidget(), ui.GetStyle( eStyle.GameplayUI ),
													"Button As Panel 1", ePlacement.Centered );

				XUI.Button bap_2 = new XUI.Button(	ui.GetScreenWidget(), ui.GetStyle( eStyle.FrontendTest ),
													"Button As Panel 2", new Vector2( 100, 800 ) );
				ui.AddRootWidget( bap_1 );
				ui.AddRootWidget( bap_2 );
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
