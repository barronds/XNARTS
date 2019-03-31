using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI : XIBroadcaster< XUI.SelectorEvent >
	{
		public interface ISelector
		{
			long GetID();
			void Draw( XSimpleDraw simple_draw );
		}

		private XBroadcaster< SelectorEvent >	mBroadcaster_SelectorEvent;
		private XListener< ButtonUpEvent >		mListener_ButtonUpEvent;
		private Dictionary< long, ISelector >   mSelectors;

		private void Constructor_Selector()
		{
			mBroadcaster_SelectorEvent = new XBroadcaster<SelectorEvent>();
			mListener_ButtonUpEvent = new XListener<ButtonUpEvent>( 1, eEventQueueFullBehaviour.Assert, "XUIselectorbutton" );
			mSelectors = new Dictionary<long, ISelector>();
		}

		private void Init_Selector()
		{
			//mBroadcaster_ButtonEvent.Subscribe( mListener_ButtonEvent );
		}

		XBroadcaster<SelectorEvent> XIBroadcaster<SelectorEvent>.GetBroadcaster()
		{
			return mBroadcaster_SelectorEvent;
		}
		public class SelectorEvent
		{
			public SelectorEvent( int index_selected, long id )
			{
				mIndexSelected = index_selected;
				mID = id;
			}

			public int mIndexSelected;
			public long mID;
		}

		public ISelector CreateSelector(	Vector2 pos, String title, eStyle style, eStyle button_style, eStyle title_style, 
											eStyle control_style, String[] texts, String[] controls )
		{
			ISelector selector = new Selector(	pos, title, style, button_style, title_style, control_style, NextID(), 
												texts, controls );

			mSelectors.Add( selector.GetID(), selector );
			return selector;
		}

		public class Selector : ISelector
		{
			private bool mRenderEnabled;
			private long mID;
			private String mTitle;
			private eStyle mStyle;
			private eStyle mButtonStyle;
			private eStyle mTitleStyle;
			private eStyle mControlStyle;
			private Vector2 mPos;
			private xAABB2 mAABB;

			public Selector(	Vector2 pos, String title, eStyle style, eStyle button_style, eStyle title_style, 
								eStyle control_style, long id, String[] texts, String[] controls )
			{
				mRenderEnabled = true;
				mID = id;
				mPos = pos;
				mTitle = title;
				mStyle = style;
				mButtonStyle = button_style;
				mTitleStyle = title_style;
				mControlStyle = control_style;

				// create a default button to see how big it is vertically
				// size and position border accordingly, factoring in width of largest button including title
				// destroy that button
				// create all the proper buttons in the right spot
				// create title 'button' as disabled button
				XUI xui_inst = XUI.Instance();

				IButton test = xui_inst.CreateRectangularButton( Vector2.Zero, "Test", style );
				xAABB2 button_size = test.GetAABB();
				float button_size_y = button_size.GetSize().Y;
				xui_inst.DestroyButton( test );

				const float k_border_padding_scalar = 0.5f;
				float border_padding = k_border_padding_scalar * button_size_y;

				const float k_spacing_scalar = 0.2f;
				float spacing = k_spacing_scalar * button_size_y;

				// pad out the text strings so the buttons can be wide if the text is small
				int longest = 0;

				for( int i = 0; i < texts.Length; ++i )
				{
					longest = Math.Max( longest, texts[ i ].Length );
				}

				for ( int i = 0; i < controls.Length; ++i )
				{
					longest = Math.Max( longest, controls[ i ].Length );
				}

				for ( int i = 0; i < texts.Length; ++i )
				{
					texts[ i ] = PadButtonText( texts[ i ], longest );
				}

				for ( int i = 0; i < controls.Length; ++i )
				{
					controls[ i ] = PadButtonText( controls[ i ], longest );
				}

				// create buttons and track largest
				IButton[] text_buttons = new IButton[ texts.Length ];
				IButton[] control_buttons = new IButton[ controls.Length ];
				float largest_x = 0;

				for ( int i = 0; i < texts.Length; ++i )
				{
					text_buttons[ i ] = PositionAndCreateButton( pos, border_padding, spacing, button_size_y,
						button_style, i, texts[ i ] );

					float size_x = text_buttons[ i ].GetAABB().GetSize().X;
					largest_x = Math.Max( size_x, largest_x );
				}

				for ( int i = 0; i < controls.Length; ++i )
				{
					control_buttons[ i ] = PositionAndCreateButton( pos, border_padding, spacing, button_size_y,
						control_style, i + texts.Length, controls[ i ] );

					float size_x = control_buttons[ i ].GetAABB().GetSize().X;
					largest_x = Math.Max( size_x, largest_x );
				}

				// create title button (non-functional) and see if it's the largest
				Vector2 title_pos = pos + new Vector2( border_padding, border_padding );
				IButton title_button = xui_inst.CreateRectangularButton( title_pos, title, title_style );
				title_button.SetActive( false );
				largest_x = Math.Max( largest_x, title_button.GetAABB().GetSize().X );

				// calculate aabb
				const float title_padding_scalar = 4.0f;
				float title_padding = border_padding * title_padding_scalar;
				Vector2 title_padding_v = new Vector2( 0, title_padding );
				float full_width = largest_x + 2 * border_padding;

				float full_height = button_size_y * (text_buttons.Length + control_buttons.Length) +
									(text_buttons.Length + control_buttons.Length - 1) * spacing +
									2 * border_padding +
									title_padding;

				mAABB.Set( pos, pos + new Vector2( full_width, full_height ) );

				// translate each button to be centered, and account for title
				for ( int i = 0; i < text_buttons.Length; ++i )
				{
					CenterButton( text_buttons[ i ], largest_x, title_padding );
				}

				for ( int i = 0; i < control_buttons.Length; ++i )
				{
					CenterButton( control_buttons[ i ], largest_x, title_padding );
				}

				CenterButton( title_button, largest_x, 0 );
			}

			private IButton PositionAndCreateButton(	Vector2 pos, float border_padding, float spacing, 
														float button_size_y, eStyle button_style, int button_num, String text )
			{
				Vector2 button_pos = pos;
				button_pos.X += border_padding;
				button_pos.Y += border_padding + (spacing + button_size_y) * button_num;
				return XUI.Instance().CreateRectangularButton( button_pos, text, button_style );
			}
			private String PadButtonText( String text, int longest )
			{
				int length = text.Length;
				int shortfall = longest - length;
				int even_floor_half_shortfall = shortfall / 2;
				String padding = XUtils.GetNSpaces( even_floor_half_shortfall );
				return padding + text + padding;
			}
			private void CenterButton( IButton button, float largest, float title_padding )
			{
				float size_x = button.GetAABB().GetSize().X;
				float shift = (largest - size_x) * 0.5f;
				button.Translate( new Vector2( shift, title_padding ) );
			}
			public void SetRenderEnabled( bool value )
			{
				mRenderEnabled = value;
			}
			long ISelector.GetID()
			{
				return mID;
			}

			void ISelector.Draw( XSimpleDraw simple_draw )
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
