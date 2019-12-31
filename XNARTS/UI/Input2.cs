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

			if ( mCurrentlyPressed != null )
			{
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
				for( int i = 0; i < mActiveButtons.Count; ++i )
				{
					if( mActiveButtons[ i ].Contains( data.mCurrentPos ) )
					{
						mCurrentlyPressed = mActiveButtons[ i ];
						SendButtonDownEvent();
						break;
					}
				}
			}
		}

		public void AddActiveButton( Button b )
		{
			XUtils.Assert( mActiveButtons.Find( Widget.CompareWidgets( b ) ) == null );
			mActiveButtons.Add( b );
		}

		public void RemoveActiveButton( Button b )
		{
			XUtils.Assert( mActiveButtons.Remove( b ) );
		}

	}
}
