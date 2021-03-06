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
		public interface _ISelector
		{
			long GetID();
			void Draw( XSimpleDraw simple_draw );
			int CheckSelections( long id );
			void Destroy();
		}

		private XBroadcaster< SelectorSelectionEvent >	mBroadcaster_SelectorSelectionEvent;
		private XListener< ButtonUpEvent >				mListener_ButtonUpEvent;
		private Dictionary< long, _ISelector >			mSelectors;

		private void Constructor_Selector()
		{
			mBroadcaster_SelectorSelectionEvent = new XBroadcaster<SelectorSelectionEvent>();
			mListener_ButtonUpEvent = new XListener<ButtonUpEvent>( 1, eEventQueueFullBehaviour.Assert, "XUIselectorbutton" );
			mSelectors = new Dictionary<long, _ISelector>();
		}

		private void Init_Selector()
		{
			mBroadcaster_ButtonUpEvent.Subscribe( mListener_ButtonUpEvent );
		}

		public XBroadcaster<SelectorSelectionEvent> GetBroadcaster_SelectorSelectionEvent()
		{
			return mBroadcaster_SelectorSelectionEvent;
		}

		public class SelectorSelectionEvent
		{
			public SelectorSelectionEvent( long selector_id, int index_selected )
			{
				mSelectorID = selector_id;
				mIndexSelected = index_selected;
			}
			public long mSelectorID;
			public int mIndexSelected;
		}

		public String GetSpacerString()
		{
			return "<<SPACER>>";
		}

		public _ISelector CreateSelector(	_Position pos, String title, eStyle style, eStyle button_style, eStyle title_style, 
											String[] texts )
		{

			_ISelector selector = new Selector(	pos, title, style, button_style, title_style, NextUID(), 
												texts );

			mSelectors.Add( selector.GetID(), selector );
			return selector;
		}

		public void DestroySelector( long id )
		{
			_ISelector selector = mSelectors[ id ];
			selector.Destroy();
			mSelectors.Remove( id );
		}

		public class Selector : _ISelector
		{
			private bool mRenderEnabled;
			private long mID;
			private String mTitle;
			private eStyle mStyle;
			private eStyle mButtonStyle;
			private eStyle mTitleStyle;
			private Vector2 mPos;
			private _Position mPosition;
			private xAABB2 mAABB;
			private _IButton[] mSelections;
			private _IButton mTitleButton;

			public Selector(	_Position pos, String title, eStyle style, eStyle button_style, eStyle title_style, 
								long id, String[] texts )
			{
				mRenderEnabled = true;
				mID = id;
				mPos = pos.GetPosition();
				mPosition = pos;
				mTitle = title;
				mStyle = style;
				mButtonStyle = button_style;
				mTitleStyle = title_style;
				this.mSelections = new _IButton[ texts.Length ];

				// create a default button to see how big it is vertically
				// size and position border accordingly, factoring in width of largest button including title
				// destroy that button
				// create all the proper buttons in the right spot
				// create title 'button' as disabled button
				XUI xui_inst = XUI.Instance();

				_IButton test = xui_inst._CreateRectangularButton( Vector2.Zero, "Test", style );
				xAABB2 button_size = test.GetAABB();
				float button_size_y = button_size.GetSize().Y;
				xui_inst._DestroyButton( test );

				const float k_border_padding_scalar = 0.5f;
				float border_padding = k_border_padding_scalar * button_size_y;

				const float k_spacing_scalar = 0.2f;
				float spacing = k_spacing_scalar * button_size_y;

				// pad out the text strings so the buttons can be wide if the text is small
				int longest = GetLongestString( texts );
				PadButtonTexts( texts, longest );

				// create buttons 
				PositionAndCreateButtons( texts, mSelections, border_padding, spacing, button_size_y, button_style, 0 );

				// track largest
				float largest_x = GetWidest( mSelections );

				// create title button (non-functional) and see if it's the largest
				Vector2 title_pos = mPos + new Vector2( border_padding, border_padding );
				mTitleButton = xui_inst._CreateRectangularButton( title_pos, title, title_style );
				mTitleButton.SetActive( false );
				largest_x = Math.Max( largest_x, mTitleButton.GetAABB().GetSize().X );

				// calculate aabb
				const float title_padding_scalar = 4.0f;
				float title_padding = border_padding * title_padding_scalar;
				Vector2 title_padding_v = new Vector2( 0, title_padding );
				float full_width = largest_x + 2 * border_padding;

				float full_height = button_size_y * (mSelections.Length) +
									(mSelections.Length - 1) * spacing +
									2 * border_padding +
									title_padding;

				mAABB.Set( mPos, mPos + new Vector2( full_width, full_height ) );

				// translate each button to be centered, and account for title
				CenterButtons( mSelections, largest_x, title_padding );
				CenterButton( mTitleButton, largest_x, 0 );

				// if the selector has a non-trivial Position, fix it
				if ( mPosition.IsCentered() )
				{
					// see where it is now, figure out where it should be, translate.
					// apply to aabb for selector plus translate all the buttons
					xCoord screen_dim = XRenderManager.Instance().GetScreenDim();
					Vector2 span = mAABB.GetSize();
					Vector2 screen_dim_vec = new Vector2( screen_dim.x, screen_dim.y );
					Vector2 edge = 0.5f * (screen_dim_vec - span);
					Translate( edge );
				}
			}
			private void Translate( Vector2 t )
			{
				mAABB.Translate( t );
				mPos = mAABB.GetMin();

				// translate each button the same amount, and account for title
				for ( int i = 0; i < mSelections.Length; ++i )
				{
					mSelections[ i ].Translate( t );
				}

				mTitleButton.Translate( t );
			}
			private _IButton PositionAndCreateButton(	Vector2 pos, float border_padding, float spacing, 
														float button_size_y, eStyle button_style, int button_num, String text )
			{
				Vector2 button_pos = pos;
				button_pos.X += border_padding;
				button_pos.Y += border_padding + (spacing + button_size_y) * button_num;
				return XUI.Instance()._CreateRectangularButton( button_pos, text, button_style );
			}
			private void PositionAndCreateButtons( String[] strings, _IButton[] dest, float border_padding, float spacing,
													float button_size_y, eStyle style, int offset )
			{
				String spacer = XUI.Instance().GetSpacerString();
				const String spacer_replacement = " ";

				for ( int i = 0; i < strings.Length; ++i )
				{
					bool is_spacer = spacer == strings[ i ];

					if( is_spacer )
					{
						dest[ i ] = PositionAndCreateButton( mPos, border_padding, spacing, button_size_y,
							style, i + offset, spacer_replacement );

						dest[ i ].SetActive( false );
					}
					else
					{
						dest[ i ] = PositionAndCreateButton( mPos, border_padding, spacing, button_size_y,
							style, i + offset, strings[ i ] );
					}


				}
			}
			private int GetLongestString( String[] strings )
			{
				int longest = 0;

				for ( int i = 0; i < strings.Length; ++i )
				{
					longest = Math.Max( longest, strings[ i ].Length );
				}

                return longest;
            }
			private String PadButtonText( String text, int longest )
			{
				int length = text.Length;
				int shortfall = longest - length;
				int even_floor_half_shortfall = shortfall / 2;
				String padding = XUtils.GetNSpaces( even_floor_half_shortfall );
				return padding + text + padding;
			}
			private void PadButtonTexts( String[] strings, int longest )
			{
				for ( int i = 0; i < strings.Length; ++i )
				{
					strings[ i ] = PadButtonText( strings[ i ], longest );
				}
			}
			private float GetWidest( _IButton[] buttons )
			{
				float largest_x = 0.0f;

				for ( int i = 0; i < buttons.Length; ++i )
				{
					float size_x = buttons[ i ].GetAABB().GetSize().X;
					largest_x = Math.Max( size_x, largest_x );
				}

				return largest_x;
			}
			private void CenterButton( _IButton button, float largest, float title_padding )
			{
				float size_x = button.GetAABB().GetSize().X;
				float shift = (largest - size_x) * 0.5f;
				button.Translate( new Vector2( shift, title_padding ) );
			}
			private void CenterButtons( _IButton[] buttons, float largest_x, float title_padding )
			{
				for ( int i = 0; i < buttons.Length; ++i )
				{
					CenterButton( buttons[ i ], largest_x, title_padding );
				}
			}
			public void SetRenderEnabled( bool value )
			{
				mRenderEnabled = value;
			}
			void _ISelector.Destroy()
			{
				XUI ui = XUI.Instance();
				ui._DestroyButton( mTitleButton );

				for( int i = 0; i < mSelections.Length; ++i )
				{
					ui._DestroyButton( mSelections[ i ] );
				}
			}
			int _ISelector.CheckSelections( long id )
			{
				for( int i = 0; i < mSelections.Length; ++i )
				{
					if( mSelections[ i ].GetID() == id )
					{
						return i;
					}
				}

				return -1;
			}
			long _ISelector.GetID()
			{
				return mID;
			}

			void _ISelector.Draw( XSimpleDraw simple_draw )
			{
				if( mRenderEnabled )
				{
					// draw the title and background, buttons will draw themselves
					Style style = XUI.Instance().GetStyle( mStyle );
					Color widget_color = style.mBackgroundColor;
					Color border_color = style.mBorderColor;

					Vector3 lo_x_lo_y = new Vector3( mAABB.GetMin(), 1 );
					Vector3 hi_x_hi_y = new Vector3( mAABB.GetMax(), 1 );

					Vector2 size = mAABB.GetSize();
					Vector3 lo_x_hi_y = lo_x_lo_y + new Vector3( 0, size.Y, 0 );
					Vector3 hi_x_lo_y = lo_x_lo_y + new Vector3( size.X, 0, 0 );

					simple_draw.DrawQuad( lo_x_lo_y, hi_x_hi_y, widget_color );

					simple_draw.DrawLine( lo_x_lo_y, hi_x_lo_y, border_color );
					simple_draw.DrawLine( hi_x_lo_y, hi_x_hi_y, border_color );
					simple_draw.DrawLine( hi_x_hi_y, lo_x_hi_y, border_color );
					simple_draw.DrawLine( lo_x_hi_y, lo_x_lo_y, border_color );
				}
			}
		}
	}

}
