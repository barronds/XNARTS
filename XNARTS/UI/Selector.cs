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

		public ISelector CreateSelector( String title, eStyle style, Vector2 location, String[] texts )
		{
			return null;
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

			public Selector( long id )
			{
				mRenderEnabled = true;
				mID = id;
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

				}
			}
		}
	}

}
