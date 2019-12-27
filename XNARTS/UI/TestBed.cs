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
			private delegate void TestFunc();

			private XListener< XTouch.FourContacts >	mListener_FourContacts;
			private XListener< XKeyInput.KeyUp >        mListener_KeyUp;
			private bool                                mTesting;
			private int                                 mTestFuncIndex;
			private List< TestFunc >					mTestFuncs;
			private List< Widget >                      mRootWidgets;


			public TestBed()
			{
				mTesting = false;
				mTestFuncIndex = 0;
				mRootWidgets = new List<Widget>();

				// update this manually when a test func is added
				mTestFuncs = new List<TestFunc>();
				mTestFuncs.Add( Test_Label );
				mTestFuncs.Add( Test_Positioning );
				mTestFuncs.Add( Test_Panel );
				mTestFuncs.Add( Test_Button );
				mTestFuncs.Add( Test_State );
				mTestFuncs.Add( Test_VerticalStack );
			}

			public void Init()
			{
				mListener_FourContacts = new XListener<XTouch.FourContacts>( 1, eEventQueueFullBehaviour.Ignore, "XUITB4C" );
				XBulletinBoard.Instance().mBroadcaster_FourContacts.Subscribe( mListener_FourContacts );

				mListener_KeyUp = new XListener<XKeyInput.KeyUp>( 1, eEventQueueFullBehaviour.Ignore, "XUITBKU" );
				XBulletinBoard.Instance().mBroadcaster_KeyUp.Subscribe( mListener_KeyUp );
			}

			public void Update( GameTime game_time )
			{
				bool trigger_test = false;

				if( mListener_FourContacts.GetMaxOne() != null )
				{
					if( mTesting )
					{
						RemoveRootWidgets();
						mTesting = false;
					}
					else
					{
						mTesting = true;
						mTestFuncIndex = 0;
						trigger_test = true;
					}
				}

				if( mTesting )
				{
					XKeyInput.KeyUp key = mListener_KeyUp.GetMaxOne();

					if ( key != null && key.mKey == Microsoft.Xna.Framework.Input.Keys.N )
					{
						trigger_test = true;
					}
				}

				if( trigger_test )
				{
					RemoveRootWidgets();
					mTestFuncs[ mTestFuncIndex ]();
					mTestFuncIndex = (mTestFuncIndex + 1) % mTestFuncs.Count();
				}
			}

			private void AddRootWidget( XUI ui, Widget w )
			{
				mRootWidgets.Add( w );
				ui.AddRootWidget( w );
			}

			private void RemoveRootWidgets()
			{
				XUI ui = XUI.Instance();

				for( int i = 0; i < mRootWidgets.Count(); ++i )
				{
					ui.RemoveRootWidget( mRootWidgets[ i ] );
				}

				mRootWidgets.Clear();
			}

			private void Test_Label()
			{
				XUI ui = XUI.Instance();

				Label label_1 = new XUI.Label();
				label_1.AssembleLabel( ui.GetStyle( eStyle.Frontend ), "Test Widget 1" );
				label_1.PlaceLabel( ui.GetScreenWidget(), ui.GetStyle( eStyle.Frontend ), ePlacement.CenteredBottom, Widget.eInitialState.Active );
				AddRootWidget( ui, label_1 );

				XUI.Label label_2 = new XUI.Label();
				label_2.AssembleLabel( ui.GetStyle( eStyle.GameplayUI ), "Test Widget 2" );
				label_2.PlaceLabel( ui.GetScreenWidget(), ui.GetStyle( eStyle.GameplayUI ), new Vector2( 200, 200 ), Widget.eInitialState.Active );
				AddRootWidget( ui, label_2 );
			}

			private void Test_Positioning()
			{
				XUI ui = XUI.Instance();
				Style s = ui.GetStyle( eStyle.Frontend );
				Widget w = ui.GetScreenWidget();

				for( int i = 0; i < (int)ePlacement.Num; ++i )
				{
					ePlacement p = (ePlacement)i;
					XUI.Label label = new XUI.Label();
					label.AssembleLabel( s, p.ToString() );
					label.PlaceLabel( w, s, p, Widget.eInitialState.Active );
					AddRootWidget( ui, label );
				}
			}

			private void Test_Panel()
			{
				XUI ui = XUI.Instance();

				XUI.Panel panel_1 = new XUI.Panel();
				XUI.Panel panel_2 = new XUI.Panel();
				XUI.Panel panel_3 = new XUI.Panel();
				XUI.Panel panel_4 = new XUI.Panel();
				XUI.Panel panel_5 = new XUI.Panel();
				XUI.Panel panel_6 = new XUI.Panel();
				XUI.Panel panel_7 = new XUI.Panel();
				XUI.Panel panel_8 = new XUI.Panel();
				XUI.Panel panel_9 = new XUI.Panel();
				XUI.Panel panel_10 = new XUI.Panel();
				XUI.Panel panel_11 = new XUI.Panel();
				XUI.Panel panel_12 = new XUI.Panel();

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

				xAABB2 panel_1_aabb = new xAABB2( new Vector2( 200, 600 ), new Vector2( 1500, 1100 ) );
				xAABB2 panel_2_aabb = new xAABB2( new Vector2( 210, 220 ), new Vector2( 450, 440 ) );

				Vector2 panel_3_size = new Vector2( 100, 50 );
				Vector2 panel_4_size = new Vector2( 80, 50 );
				Vector2 panel_5_size = new Vector2( 100, 150 );
				Vector2 panel_6_size = new Vector2( 90, 150 );
				Vector2 panel_7_size = new Vector2( 70, 120 );
				Vector2 panel_8_size = new Vector2( 90, 120 );
				Vector2 panel_9_size = new Vector2( 110, 80 );
				Vector2 panel_10_size = new Vector2( 60, 110 );
				Vector2 panel_11_size = new Vector2( 120, 150 );
				Vector2 panel_12_size = new Vector2( 70, 50 );

				panel_3.AssemblePanel( panel_3_size );
				panel_4.AssemblePanel( panel_4_size );
				panel_5.AssemblePanel( panel_5_size );
				panel_6.AssemblePanel( panel_6_size );
				panel_7.AssemblePanel( panel_7_size );
				panel_8.AssemblePanel( panel_8_size );
				panel_9.AssemblePanel( panel_9_size );
				panel_10.AssemblePanel( panel_10_size );
				panel_11.AssemblePanel( panel_11_size );
				panel_12.AssemblePanel( panel_12_size );
				panel_2.AssemblePanel( panel_2_aabb.GetSize() );
				panel_1.AssemblePanel( panel_1_aabb.GetSize() );

				panel_1.PlacePanel( ui.GetScreenWidget(),  ui.GetStyle( eStyle.FrontendTest ), panel_1_aabb, Widget.eInitialState.Active );
				panel_2.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), panel_2_aabb, Widget.eInitialState.Active );
				panel_3.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.Centered, Widget.eInitialState.Active );
				panel_4.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.TopLeft, Widget.eInitialState.Active );
				panel_5.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.CenteredTop, Widget.eInitialState.Active );
				panel_6.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.TopRight, Widget.eInitialState.Active );
				panel_7.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.CenteredRight, Widget.eInitialState.Active );
				panel_8.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.BottomRight, Widget.eInitialState.Active );
				panel_9.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.CenteredBottom, Widget.eInitialState.Active );
				panel_10.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.BottomLeft, Widget.eInitialState.Active );
				panel_11.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), ePlacement.CenteredLeft, Widget.eInitialState.Active );
				panel_12.PlacePanel( panel_2, ui.GetStyle( eStyle.FrontendTest ), ePlacement.CenteredLeft, Widget.eInitialState.Active );

				AddRootWidget( ui, panel_1 );
			}

			private void Test_Button()
			{
				XUI ui = XUI.Instance();
				Style style_1 = ui.GetStyle( eStyle.GameplayUI );
				Style style_2 = ui.GetStyle( eStyle.FrontendTest );

				XUI.Button bap_1 = ui.CreateButton( style_1, "Button As Panel 1", ui.GetScreenWidget(), style_1, 
													ePlacement.Centered, Widget.eInitialState.Active );

				XUI.Button bap_2 = ui.CreateButton( style_2, "Button As Panel 2", ui.GetScreenWidget(), style_2,
													new Vector2( 100, 800 ), Widget.eInitialState.Active );
				AddRootWidget( ui, bap_1 );
				AddRootWidget( ui, bap_2 );
			}

			private void Test_State()
			{

			}

			private void Test_VerticalStack()
			{
				/*
				XUI ui = XUI.Instance();
				Style s = ui.GetStyle( eStyle.GameplayUI );

				XUI.Panel panel_1 = new XUI.Panel();
				XUI.Panel panel_2 = new XUI.Panel();
				XUI.Panel panel_3 = new XUI.Panel();
				XUI.Panel panel_4 = new XUI.Panel();

				panel_1.Assemble();
				panel_2.Assemble();
				panel_3.Assemble();
				panel_4.Assemble();

				//panel_1.Place(	ui.GetScreenWidget(), s, new Vector2( 50, 20 ), ePlacement.TopLeft, Widget.eInitialState.Active );
				//panel_2.Place(	ui.GetScreenWidget(), s, new Vector2( 100, 30 ), ePlacement.TopLeft, Widget.eInitialState.Active );
				//panel_3.Place(	ui.GetScreenWidget(), s, new Vector2( 70, 70 ), ePlacement.TopLeft, Widget.eInitialState.Active );
				//panel_4.Place(	ui.GetScreenWidget(), s, new Vector2( 20, 100 ), ePlacement.TopLeft, Widget.eInitialState.Active );

				XUI.Panel[] panels = { panel_1, panel_2, panel_3, panel_4 };

				XUI.VerticalStack stack = new VerticalStack();
				stack.AssembleWidget(  );
				stack.InitVerticalStack( ui.GetScreenWidget(), panels, s, ePlacement.Centered, Widget.eInitialState.Active );

				AddRootWidget( ui, stack );
				*/
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
