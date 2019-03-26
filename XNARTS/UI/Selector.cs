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
			void Draw();
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

		private void Draw_Selector()
		{
			var enumerator = mSelectors.GetEnumerator();

			while( enumerator.MoveNext() )
			{
				enumerator.Current.Value.Draw();
			}
		}

		public class Selector : ISelector
		{
			private bool mRenderEnabled;
			private long mID;

			public Selector( Vector2 pos, String title, eStyle style, long id, String[] texts )
			{
				mRenderEnabled = true;
				mID = id;

				// create a default button to see how big it is vertically
				// size and position border accordingly
				// destroy that button
				// create all the proper buttons in the right spot
				XUI xui_inst = XUI.Instance();

				IButton test = xui_inst.CreateRectangularButton( Vector2.Zero, "Test", style );
				xAABB2 button_size = test.GetAABB();
				float button_size_y = button_size.GetSize().Y;
				xui_inst.DestroyButton( test );

				const float k_border_padding_scalar = 1.0f;
				float border_padding = k_border_padding_scalar * button_size_y;

				const float k_spacing_scalar = 0.2f;
				float spacing = k_spacing_scalar * button_size_y;

				IButton[] buttons = new IButton[ texts.Length ];

				for( int i = 0; i < texts.Length; ++i )
				{
					Vector2 button_pos = pos;
					button_pos.Y += 2 * border_padding + (spacing + button_size_y) * i;
					buttons[ i ] = xui_inst.CreateRectangularButton( button_pos, texts[ i ], style );
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

			void ISelector.Draw()
			{
				if( mRenderEnabled )
				{
					// draw the title and background, buttons will draw themselves
				}
			}
		}
	}

}
