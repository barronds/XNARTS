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
				label_1.Asssemble( ui.GetStyle( eStyle.Frontend ), "Test Widget 1" );
				label_1.Place( ui.GetScreenWidget(), ui.GetStyle( eStyle.Frontend ), ePlacement.CenteredBottom, Widget.eInitialState.Active );
				AddRootWidget( ui, label_1 );

				XUI.Label label_2 = new XUI.Label();
				label_2.Asssemble( ui.GetStyle( eStyle.GameplayUI ), "Test Widget 2" );
				label_2.Place( ui.GetScreenWidget(), ui.GetStyle( eStyle.GameplayUI ), new Vector2( 200, 200 ), Widget.eInitialState.Active );
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
					label.Asssemble( s, p.ToString() );
					label.Place( w, s, p, Widget.eInitialState.Active );
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

				panel_1.Assemble();
				panel_2.Assemble();
				panel_3.Assemble();
				panel_4.Assemble();
				panel_5.Assemble();
				panel_6.Assemble();
				panel_7.Assemble();
				panel_8.Assemble();
				panel_9.Assemble();
				panel_10.Assemble();
				panel_11.Assemble();
				panel_12.Assemble();

				panel_1.Place(  ui.GetScreenWidget(),  ui.GetStyle( eStyle.FrontendTest ),
								new xAABB2( new Vector2( 200, 600 ), new Vector2( 1500, 1100 ) ),
								Widget.eInitialState.Active );

				panel_2.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ),
								new xAABB2( new Vector2( 210, 220 ), new Vector2( 450, 440 ) ),
								Widget.eInitialState.Active );

				panel_3.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 100, 50 ),
								ePlacement.Centered, Widget.eInitialState.Active );

				panel_4.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 80, 50 ),
								ePlacement.TopLeft, Widget.eInitialState.Active );

				panel_5.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 100, 150 ),
								ePlacement.CenteredTop, Widget.eInitialState.Active );

				panel_6.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 90, 150 ),
								ePlacement.TopRight, Widget.eInitialState.Active );

				panel_7.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 70, 120 ),
								ePlacement.CenteredRight, Widget.eInitialState.Active );

				panel_8.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 90, 120 ),
								ePlacement.BottomRight, Widget.eInitialState.Active );

				panel_9.Place(  panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 110, 80 ),
								ePlacement.CenteredBottom, Widget.eInitialState.Active );

				panel_10.Place( panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 60, 110 ),
								ePlacement.BottomLeft, Widget.eInitialState.Active );

				panel_11.Place( panel_1, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 120, 150 ),
								ePlacement.CenteredLeft, Widget.eInitialState.Active );

				panel_12.Place( panel_2, ui.GetStyle( eStyle.FrontendTest ), new Vector2( 70, 50 ),
								ePlacement.CenteredLeft, Widget.eInitialState.Active );

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

				AddRootWidget( ui, panel_1 );
			}

			private void Test_Button()
			{
				XUI ui = XUI.Instance();
				XUI.Button bap_1 = new XUI.Button(	ui.GetScreenWidget(), ui.GetStyle( eStyle.GameplayUI ),
													"Button As Panel 1", ePlacement.Centered, Widget.eInitialState.Active );

				XUI.Button bap_2 = new XUI.Button(	ui.GetScreenWidget(), ui.GetStyle( eStyle.FrontendTest ),
													"Button As Panel 2", new Vector2( 100, 800 ), Widget.eInitialState.Active );
				AddRootWidget( ui, bap_1 );
				AddRootWidget( ui, bap_2 );
			}

			private void Test_State()
			{

			}

			private void Test_VerticalStack()
			{
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

				panel_1.Place(	ui.GetScreenWidget(), s, new Vector2( 50, 20 ), ePlacement.TopLeft, Widget.eInitialState.Active );
				panel_2.Place(	ui.GetScreenWidget(), s, new Vector2( 100, 30 ), ePlacement.TopLeft, Widget.eInitialState.Active );
				panel_3.Place(	ui.GetScreenWidget(), s, new Vector2( 70, 70 ), ePlacement.TopLeft, Widget.eInitialState.Active );
				panel_4.Place(	ui.GetScreenWidget(), s, new Vector2( 20, 100 ), ePlacement.TopLeft, Widget.eInitialState.Active );

				XUI.Panel[] panels = { panel_1, panel_2, panel_3, panel_4 };

				XUI.VerticalStack stack = new VerticalStack();
				stack.AssembleWidget();
				stack.InitVerticalStack( ui.GetScreenWidget(), panels, s, ePlacement.Centered, Widget.eInitialState.Active );

				AddRootWidget( ui, stack );
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
