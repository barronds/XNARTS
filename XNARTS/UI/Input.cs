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
			mListener_SinglePoke = new XListener<XTouch.SinglePokeData>();
		}

		private void Init_Input()
		{
			((XIBroadcaster<XTouch.SinglePokeData>)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_SinglePoke );
		}

		private void Update_Input()
		{
			// things to check for:
			// - currently pressed button might trigger a button up
			// - currently pressed button might get aborted by touch gesture or drift
			// - curretnly pressed button might still be pressed
			// - on press start, a new button may be pressed

			XTouch.SinglePokeData data =    (mListener_SinglePoke.GetNumEvents() > 0)   ?
											mListener_SinglePoke.ReadNext()             :
											null                                        ;

			if ( mCurrentlyPressed != null )
			{
				XUtils.Assert( data != null, "should have hold, end, or abort" );

				if ( data.mDetail == XTouch.ePokeDetail.Hold )
				{
					if ( mCurrentlyPressed.Contains( data.mCurrentPos ) )
					{
						// pressed is still pressed
						SendButtonEvent( ButtonEvent.Type.Held );
					}
					else
					{
						// have strayed off button with a hold, un-press
						SendButtonEvent( ButtonEvent.Type.Abort );
					}
				}
				else if ( data.mDetail == XTouch.ePokeDetail.End_Abort )
				{
					// touch decided this gesture is no good, un-press
					SendButtonEvent( ButtonEvent.Type.Abort );
				}
				else if ( data.mDetail == XTouch.ePokeDetail.End_Normal )
				{
					// this is a pressed button
					SendButtonEvent( ButtonEvent.Type.Up );
				}
				else
				{
					XUtils.Assert( false, "not expecting another state when mCurrentlyPressed is valid" );
				}
			}
			else if ( data != null && data.mDetail == XTouch.ePokeDetail.Start )
			{
				// new press, let's see if it hits a button
				var enumerator = mButtons.GetEnumerator();

				while ( enumerator.MoveNext() )
				{
					if ( enumerator.Current.Value.Contains( data.mCurrentPos ) )
					{
						mCurrentlyPressed = enumerator.Current.Value;
						SendButtonEvent( ButtonEvent.Type.Down );
						break;
					}
				}
			}
		}
	}
}
