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
		//private XBroadcaster< ButtonUpEvent >       mBroadcaster_ButtonUpEvent;
		//private XBroadcaster< ButtonDownEvent >     mBroadcaster_ButtonDownEvent;
		//private XBroadcaster< ButtonHeldEvent >     mBroadcaster_ButtonHeldEvent;
		//private XBroadcaster< ButtonAbortEvent >    mBroadcaster_ButtonAbortEvent;
		private XListener< XTouch.SinglePokeData >  mListener_SinglePoke;
		private List< Button >						mButtons;
		private Button								mCurrentlyPressed;


		private void Constructor_Input()
		{
			mListener_SinglePoke = new XListener<XTouch.SinglePokeData>( 1, eEventQueueFullBehaviour.Assert, "XUIinputsinglepoke" );
		}

		private void Init_Input()
		{
			XBulletinBoard.Instance().mBroadcaster_SinglePoke.Subscribe( mListener_SinglePoke );
		}

		private void Update_Input()
		{
			Update_Input_Buttons();
		}

		private void Update_Input_Buttons()
		{
			// things to check for:
			// - currently pressed button might trigger a button up
			// - currently pressed button might get aborted by touch gesture or drift
			// - curretnly pressed button might still be pressed
			// - on press start, a new button may be pressed
			var single_poke_enumerator = mListener_SinglePoke.CreateEnumerator();
			single_poke_enumerator.MoveNext();
			var data = single_poke_enumerator.GetCurrent();

			if ( _mCurrentlyPressed != null )
			{
				// first check if button disabled before considering input
				if ( !_mCurrentlyPressed.IsActive() )
				{
					SendButtonAbortEvent();
					return;
				}

				XUtils.Assert( data != null, "should have hold, end, or abort" );

				if ( data.mDetail == XTouch.ePokeDetail.Hold )
				{
					if ( _mCurrentlyPressed.Contains( data.mCurrentPos ) )
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
				var enumerator = _mButtons.GetEnumerator();

				while ( enumerator.MoveNext() )
				{
					if ( enumerator.Current.Value.Contains( data.mCurrentPos ) && enumerator.Current.Value.IsActive() )
					{
						_mCurrentlyPressed = enumerator.Current.Value;
						SendButtonDownEvent();
						break;
					}
				}
			}
		}

		void Update_Input_Selector()
		{
			// go through all selectors and check all buttons against button_up input
			ButtonUpEvent button_up = mListener_ButtonUpEvent.GetMaxOne();
			int selected_index = -1;

			if ( button_up != null )
			{
				var s = mSelectors.GetEnumerator();

				while ( s.MoveNext() )
				{
					if ( (selected_index = s.Current.Value.CheckSelections( button_up.mID )) > -1 )
					{
						SelectorSelectionEvent e = new SelectorSelectionEvent( s.Current.Value.GetID(), selected_index );
						mBroadcaster_SelectorSelectionEvent.Post( e );
					}
				}
			}
		}
	}
}
