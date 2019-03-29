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

		public ISelector CreateSelector( Vector2 pos, String title, eStyle style, String[] texts )
		{
			ISelector selector = new Selector( pos, title, style, NextID(), texts );
			mSelectors.Add( selector.GetID(), selector );
			return selector;
		}

		public class Selector : ISelector
		{
			private bool mRenderEnabled;
			private long mID;
			private String mTitle;
			private eStyle mStyle;
			private Vector2 mPos;
			private xAABB2 mAABB;

			public Selector( Vector2 pos, String title, eStyle style, long id, String[] texts )
			{
				mRenderEnabled = true;
				mID = id;
				mPos = pos;
				mTitle = title;
				mStyle = style;

				// create a default button to see how big it is vertically
				// size and position border accordingly
				// destroy that button
				// create all the proper buttons in the right spot
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

				for( int i = 0; i < texts.Length; ++i )
				{
					int length = texts[ i ].Length;
					int shortfall = longest - length;
					int even_floor_half_shortfall = shortfall / 2;
					String padding = XUtils.GetNSpaces( even_floor_half_shortfall );
					texts[ i ] = padding + texts[ i ] + padding;

					// this code would make buttons all the same size but sacrifice perfect centering of text.
					//if( texts[ i ].Length < longest )
					//{
					//	texts[ i ] += " ";
					//}
				}

				// create buttons and track largest
				IButton[] buttons = new IButton[ texts.Length ];
				float largest_x = 0;

				for ( int i = 0; i < texts.Length; ++i )
				{
					Vector2 button_pos = pos;
					button_pos.X += border_padding;
					button_pos.Y += border_padding + (spacing + button_size_y) * i;
					buttons[ i ] = xui_inst.CreateRectangularButton( button_pos, texts[ i ], style );
					float size_x = buttons[ i ].GetAABB().GetSize().X;
					largest_x = Math.Max( size_x, largest_x );
				}

				// calculate aabb
				const float title_padding_scalar = 4.0f;
				float title_padding = border_padding * title_padding_scalar;
				Vector2 title_padding_v = new Vector2( 0, title_padding );
				float full_width = largest_x + 2 * border_padding;

				float full_height = button_size_y * buttons.Length +
									(buttons.Length - 1) * spacing +
									2 * border_padding +
									title_padding;

				mAABB.Set( pos, pos + new Vector2( full_width, full_height ) );

				// translate each button to be centered, and account for title
				for ( int i = 0; i < buttons.Length; ++i )
				{
					float size_x = buttons[ i ].GetAABB().GetSize().X;
					float shift = (largest_x - size_x) * 0.5f;
					buttons[ i ].Translate( new Vector2( shift, title_padding ) );
				}
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
					Color widget_color = XUI.Instance().GetStyle( mStyle ).mBackgroundColor;
					simple_draw.DrawQuad( new Vector3( mAABB.GetMin(), 1 ), new Vector3( mAABB.GetMax(), 1 ), widget_color );
				}
			}
		}
	}

}
