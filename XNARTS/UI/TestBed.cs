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
					//Test_Panel();
					Test_Button();
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
				XUI.Panel panel_1 = new XUI.Panel(  ui.GetScreenWidget(),  ui.GetStyle( eStyle.FrontendTest ),
													new xAABB2( new Vector2( 200, 600 ), new Vector2( 1500, 1100 ) ) );

				XUI.Panel panel_2 = new XUI.Panel(	panel_1, ui.GetStyle( eStyle.FrontendTest ),
													new xAABB2( new Vector2( 210, 220 ), new Vector2( 450, 440 ) ) );

				XUI.Panel panel_3 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 100, 50 ),
													ePlacement.Centered );

				XUI.Panel panel_4 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 80, 50 ),
													ePlacement.TopLeft );

				XUI.Panel panel_5 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 100, 150 ),
													ePlacement.CenteredTop );

				XUI.Panel panel_6 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 90, 150 ),
													ePlacement.TopRight );

				XUI.Panel panel_7 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 70, 120 ),
													ePlacement.CenteredRight );

				XUI.Panel panel_8 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 90, 120 ),
													ePlacement.BottomRight );

				XUI.Panel panel_9 = new XUI.Panel(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 110, 80 ),
													ePlacement.CenteredBottom );

				XUI.Panel panel_10 = new XUI.Panel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 60, 110 ),
													ePlacement.BottomLeft );

				XUI.Panel panel_11 = new XUI.Panel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 120, 150 ),
													ePlacement.CenteredLeft );

				XUI.Panel panel_12 = new XUI.Panel( panel_2, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 70, 50 ),
													ePlacement.CenteredLeft );

				panel_1.AddChild( panel_2 );
				panel_1.AddChild( panel_3 );
				panel_1.AddChild( panel_4 );
				panel_1.AddChild( panel_5 );
				panel_1.AddChild( panel_6 );
				panel_1.AddChild( panel_7 );
				panel_1.AddChild( panel_8 );
				panel_1.AddChild( panel_9 );
				panel_1.AddChild( panel_10 );
				panel_1.AddChild( panel_11 );
				panel_2.AddChild( panel_12 );

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
