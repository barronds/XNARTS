﻿using System;
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
			private XListener< ButtonUpEvent >          mListener_ButtonUp;
			private bool                                mTesting;
			private int                                 mTestFuncIndex;
			private List< TestFunc >					mTestFuncs;
			private List< Widget >                      mRootWidgets;
			private List< Button >                      mRootButtons;
			private List< BasicMenu >                   mRootBasicMenus;
			private List< FullMenu >                    mRootFullMenus;


			public TestBed()
			{
				mTesting = false;
				mTestFuncIndex = 0;
				mRootWidgets = new List<Widget>();
				mRootButtons = new List<Button>();
				mRootBasicMenus = new List<BasicMenu>();
				mRootFullMenus = new List<FullMenu>();

				// update this manually when a test func is added
				mTestFuncs = new List<TestFunc>();
				mTestFuncs.Add( Test_Label );
				mTestFuncs.Add( Test_Positioning );
				mTestFuncs.Add( Test_Panel );
				mTestFuncs.Add( Test_Button );
				mTestFuncs.Add( Test_LinearStack );
				mTestFuncs.Add( Test_BasicMenu );
				mTestFuncs.Add( Test_FullMenu );
			}

			public void Init()
			{
				mListener_FourContacts = new XListener<XTouch.FourContacts>( 1, eEventQueueFullBehaviour.Ignore, "XUITB4C" );
				XBulletinBoard.Instance().mBroadcaster_FourContacts.Subscribe( mListener_FourContacts );

				mListener_KeyUp = new XListener<XKeyInput.KeyUp>( 1, eEventQueueFullBehaviour.Ignore, "XUITBKU" );
				XBulletinBoard.Instance().mBroadcaster_KeyUp.Subscribe( mListener_KeyUp );

				mListener_ButtonUp = new XListener<ButtonUpEvent>( 1, eEventQueueFullBehaviour.Assert, "XUITBBU" );
				XUI.Instance().mBroadcaster_ButtonUpEvent.Subscribe( mListener_ButtonUp );
			}

			public void Update( GameTime game_time )
			{
				Update_TriggerTests();
				Update_TestInput();
			}

			private void Update_TriggerTests()
			{
				bool trigger_test = false;

				if ( mListener_FourContacts.GetMaxOne() != null )
				{
					if ( mTesting )
					{
						CleanupRootWidgets();
						mTesting = false;
					}
					else
					{
						mTesting = true;
						mTestFuncIndex = 0;
						trigger_test = true;
					}
				}

				if ( mTesting )
				{
					XKeyInput.KeyUp key = mListener_KeyUp.GetMaxOne();

					if ( key != null && key.mKey == Microsoft.Xna.Framework.Input.Keys.N )
					{
						trigger_test = true;
					}
				}

				if( trigger_test )
				{
					CleanupRootWidgets();
					mTestFuncs[ mTestFuncIndex ]();
					mTestFuncIndex = (mTestFuncIndex + 1) % mTestFuncs.Count();
				}
			}

			private void Update_TestInput()
			{
				XListener<ButtonUpEvent>.Enumerator e = mListener_ButtonUp.CreateEnumerator();

				while( e.MoveNext() )
				{
					for( int b = 0; b < mRootBasicMenus.Count(); ++b )
					{
						int input_index =  mRootBasicMenus[ b ].GetInputIndex( e.GetCurrent().mID );

						if( input_index > -1 )
						{
							Console.WriteLine( "TestBed - Basic Menu Input (menu " + b + ", button index " + input_index + ")" );
						}
					}

					for( int f = 0; f < mRootFullMenus.Count(); ++f )
					{
						int options_index = mRootFullMenus[ f ].GetOptionsInputIndex( e.GetCurrent().mID );
						int controls_index = mRootFullMenus[ f ].GetControlsInputIndex( e.GetCurrent().mID );

						if( options_index > -1 )
						{
							Console.WriteLine( "TestBed - Full Menu Option Input (menu " + f + ", button index " + options_index + ")" );
						}

						if ( controls_index > -1 )
						{
							Console.WriteLine( "TestBed - Full Menu Controls Input (menu " + f + ", button index " + controls_index + ")" );
						}
					}
				}
			}

			private void AddRootWidget( XUI ui, Widget w )
			{
				mRootWidgets.Add( w );
				ui.AddRootWidget( w );
			}

			private void AddRootButton( Button b )
			{
				mRootButtons.Add( b );
			}

			private void CleanupRootWidgets()
			{
				XUI ui = XUI.Instance();

				for( int i = 0; i < mRootWidgets.Count(); ++i )
				{
					ui.RemoveRootWidget( mRootWidgets[ i ] );
				}

				for( int i = 0; i < mRootButtons.Count(); ++i )
				{
					ui.DestroyButton( mRootButtons[ i ] );
				}

				for( int i = 0; i < mRootBasicMenus.Count(); ++i )
				{
					ui.DestroyBasicMenu( mRootBasicMenus[ i ] );
				}

				for( int i = 0; i < mRootFullMenus.Count(); ++i )
				{
					ui.DestroyFullMenu( mRootFullMenus[ i ] );
				}

				mRootWidgets.Clear();
				mRootButtons.Clear();
				mRootBasicMenus.Clear();
				mRootFullMenus.Clear();
			}

			private void Test_Label()
			{
				XUI ui = XUI.Instance();

				Label label_1 = new XUI.Label();
				label_1.AssembleLabel( ui.GetStyle( eStyle.Frontend ), "Test Widget 1" );

				label_1.PlaceLabel( ui.GetScreenWidget(), ui.GetStyle( eStyle.Frontend ), 
									new UIPosSpec( ePlacement.CenteredBottom, label_1.GetAssembledSize() ) );

				AddRootWidget( ui, label_1 );

				XUI.Label label_2 = new XUI.Label();
				label_2.AssembleLabel( ui.GetStyle( eStyle.GameplayUI ), "Test Widget 2" );
				Vector2 pos = new Vector2( 200, 200 );
				xAABB2 aabb = new xAABB2( pos, pos + label_2.GetAssembledSize() );
				label_2.PlaceLabel( ui.GetScreenWidget(), ui.GetStyle( eStyle.GameplayUI ), new UIPosSpec( aabb ) );
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
					label.PlaceLabel( w, s, new UIPosSpec( p, label.GetAssembledSize() ) );
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

				panel_1.PlacePanel( ui.GetScreenWidget(),  ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( panel_1_aabb ) );
				panel_2.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( panel_2_aabb ) );
				panel_3.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.Centered, panel_3.GetAssembledSize() ) );
				panel_4.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.TopLeft, panel_4.GetAssembledSize() ) );
				panel_5.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.CenteredTop, panel_5.GetAssembledSize() ) );
				panel_6.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.TopRight, panel_6.GetAssembledSize() ) );
				panel_7.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.CenteredRight, panel_7.GetAssembledSize() ) );
				panel_8.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.BottomRight, panel_8.GetAssembledSize() ) );
				panel_9.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.CenteredBottom, panel_9.GetAssembledSize() ) );
				panel_10.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.BottomLeft, panel_10.GetAssembledSize() ) );
				panel_11.PlacePanel( panel_1, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.CenteredLeft, panel_11.GetAssembledSize() ) );
				panel_12.PlacePanel( panel_2, ui.GetStyle( eStyle.FrontendTest ), new UIPosSpec( ePlacement.CenteredLeft, panel_12.GetAssembledSize() ) );

				AddRootWidget( ui, panel_1 );
			}

			private void Test_Button()
			{
				XUI ui = XUI.Instance();
				Style style_1 = ui.GetStyle( eStyle.GameplayUI );
				Style style_2 = ui.GetStyle( eStyle.FrontendTest );

				XUI.Button bap_1 = ui.CreateButton( style_1, "Button As Panel 1", ui.GetScreenWidget(), style_1, 
													new UIPosSpec( ePlacement.Centered ) );

				XUI.Button bap_2 = ui.CreateButton( style_2, "Button As Panel 2", ui.GetScreenWidget(), style_2,
													new UIPosSpec( new Vector2( 100, 800 ) ) );
				AddRootButton( bap_1 );
				AddRootButton( bap_2 );
			}

			private void Test_LinearStack()
			{
				XUI ui = XUI.Instance();
				Style s = ui.GetStyle( eStyle.GameplayUI );

				XUI.Panel panel_1 = new XUI.Panel();
				XUI.Panel panel_2 = new XUI.Panel();
				XUI.Panel panel_3 = new XUI.Panel();
				XUI.Panel panel_4 = new XUI.Panel();

				panel_1.AssemblePanel( new Vector2( 50, 20 ) );
				panel_2.AssemblePanel( new Vector2( 100, 30 ) );
				panel_3.AssemblePanel( new Vector2( 70, 70 ) );
				panel_4.AssemblePanel( new Vector2( 20, 100 ) );

				XUI.Widget[] widgets1 = { panel_1, panel_2, panel_3, panel_4 };
				XUI.LinearStack stack1 = new LinearStack( LinearStack.eDirection.Vertical );
				stack1.AssembleVerticalStack( widgets1, s );
				stack1.PlacePanel( ui.GetScreenWidget(), s, new UIPosSpec( ePlacement.Centered, stack1.GetAssembledSize() ) );

				panel_1.PlacePanel( stack1, s, new UIPosSpec( stack1.GetRelativePlacement( 0 ) ) );
				panel_2.PlacePanel( stack1, s, new UIPosSpec( stack1.GetRelativePlacement( 1 ) ) );
				panel_3.PlacePanel( stack1, s, new UIPosSpec( stack1.GetRelativePlacement( 2 ) ) );
				panel_4.PlacePanel( stack1, s, new UIPosSpec( stack1.GetRelativePlacement( 3 ) ) );

				AddRootWidget( ui, stack1 );

				XUI.Panel panel_5 = new XUI.Panel();
				XUI.Panel panel_6 = new XUI.Panel();
				XUI.Panel panel_7 = new XUI.Panel();
				XUI.Panel panel_8 = new XUI.Panel();

				panel_5.AssemblePanel( new Vector2( 50, 20 ) );
				panel_6.AssemblePanel( new Vector2( 100, 30 ) );
				panel_7.AssemblePanel( new Vector2( 70, 70 ) );
				panel_8.AssemblePanel( new Vector2( 20, 100 ) );

				XUI.Widget[] widgets2 = { panel_5, panel_6, panel_7, panel_8 };
				XUI.LinearStack stack2 = new LinearStack( LinearStack.eDirection.Horizontal );
				stack2.AssembleVerticalStack( widgets2, s );
				stack2.PlacePanel( ui.GetScreenWidget(), s, new UIPosSpec( ePlacement.CenteredBottom, stack2.GetAssembledSize() ) );

				panel_5.PlacePanel( stack2, s, new UIPosSpec( stack2.GetRelativePlacement( 0 ) ) );
				panel_6.PlacePanel( stack2, s, new UIPosSpec( stack2.GetRelativePlacement( 1 ) ) );
				panel_7.PlacePanel( stack2, s, new UIPosSpec( stack2.GetRelativePlacement( 2 ) ) );
				panel_8.PlacePanel( stack2, s, new UIPosSpec( stack2.GetRelativePlacement( 3 ) ) );

				AddRootWidget( ui, stack2 );
			}

			private void Test_BasicMenu()
			{
				XUI ui = XUI.Instance();
				String[] texts = { "First", "Another Button", "2nd to Last", " ", "5", "Back" };
				BasicMenu m = ui.CreateBasicMenu( eStyle.Frontend, texts, ui.GetScreenWidget(), eStyle.Frontend, ePlacement.TopRight );
				mRootBasicMenus.Add( m );
			}

			private void Test_FullMenu()
			{
				XUI ui = XUI.Instance();
				String title = "Test Full Menu";
				String[] options = { "First", "Another Button", "2nd to Last", " ", "5", "Reset" };
				String[] controls = { "Back", "Exit" };

				FullMenu m = ui.CreateFullMenu( eStyle.FrontendTitle, title,
												eStyle.Frontend, options,
												eStyle.FrontendControl, controls,
												eStyle.Frontend, ui.GetScreenWidget(), ePlacement.Centered );
				mRootFullMenus.Add( m );
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
