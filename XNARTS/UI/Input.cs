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
		private XListener< XTouch.SinglePokeData >  mListener_SinglePoke;

		private void Constructor_Input()
		{
			mListener_SinglePoke = new XListener<XTouch.SinglePokeData>( 1, eEventQueueFullBehaviour.Assert, "XUIinputsinglepoke" );
		}

		private void Init_Input()
		{
			((XIBroadcaster<XTouch.SinglePokeData>)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_SinglePoke );
		}

		private void Update_Input()
		{
			Update_Input_Buttons();
			Update_Input_Selector();
		}

		private void Update_Input_Buttons()
		{
			// things to check for:
			// - currently pressed button might trigger a button up
			// - currently pressed button might get aborted by touch gesture or drift
			// - curretnly pressed button might still be pressed
			// - on press start, a new button may be pressed
			XTouch.SinglePokeData data = mListener_SinglePoke.GetEnumerator().MoveNext();

			if ( mCurrentlyPressed != null )
			{
				// first check if button disabled before considering input
				if ( !mCurrentlyPressed.IsActive() )
				{
					SendButtonAbortEvent();
					return;
				}

				XUtils.Assert( data != null, "should have hold, end, or abort" );

				if ( data.mDetail == XTouch.ePokeDetail.Hold )
				{
					if ( mCurrentlyPressed.Contains( data.mCurrentPos ) )
					{
						// pressed is still pressed
						SendButtonHeldEvent();
						return;
					}

					// have strayed off button with a hold, un-press
					SendButtonAbortEvent();
					return;
				}

				if ( data.mDetail == XTouch.ePokeDetail.End_Abort )
				{
					// touch decided this gesture is no good, un-press
					SendButtonAbortEvent();
					return;
				}

				if ( data.mDetail == XTouch.ePokeDetail.End_Normal )
				{
					// this is a pressed button
					SendButtonUpEvent();
					return;
				}

				XUtils.Assert( false, "not expecting another state when mCurrentlyPressed is valid" );
				return;
			}

			if ( data != null && data.mDetail == XTouch.ePokeDetail.Start )
			{
				// new press, let's see if it hits a button
				var enumerator = mButtons.GetEnumerator();

				while ( enumerator.MoveNext() )
				{
					if ( enumerator.Current.Value.Contains( data.mCurrentPos ) && enumerator.Current.Value.IsActive() )
					{
						mCurrentlyPressed = enumerator.Current.Value;
						SendButtonDownEvent();
						break;
					}
				}
			}
		}

		void Update_Input_Selector()
		{
			// go through all selectors and check all button and controls against button_up input
			var enumerator = mListener_ButtonUpEvent.GetEnumerator();
			ButtonUpEvent button_up = null;
			int selected_index = -1;

			while( (button_up = enumerator.MoveNext()) != null )
			{
				var s = mSelectors.GetEnumerator();

				while( s.MoveNext() )
				{
					if( (selected_index = s.Current.Value.CheckSelections( button_up.mID )) > -1 )
					{
						SelectorSelectionEvent e = new SelectorSelectionEvent( s.Current.Value.GetID(), selected_index );
						mBroadcaster_SelectorSelectionEvent.Post( e );
					}
					else if( (selected_index = s.Current.Value.CheckControls( button_up.mID )) > -1 )
					{
						SelectorControlEvent e = new SelectorControlEvent( s.Current.Value.GetID(), selected_index );
						mBroadcaster_SelectorControlEvent.Post( e );
					}
				}
			}
			
		}
	}
}
