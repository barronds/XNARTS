using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI : XSingleton< XUI >, XIBroadcaster< XUI.ButtonEvent >
	{
		private XBroadcaster< ButtonEvent >			mBroadcaster_ButtonEvent;
		private XSimpleDraw							mSimpleDraw;
		private static long							sPrevID = 0;
		private Dictionary< eFont, Vector2 >		mFontSizes;
		private XListener< XTouch.SinglePokeData >	mListener_SinglePoke;

		private XUI()
		{
			// private constructor as per singleton
			mBroadcaster_ButtonEvent = new XBroadcaster<ButtonEvent>();
			mListener_SinglePoke = new XListener<XTouch.SinglePokeData>();
			mButtons = new SortedList<long, IButton>();
			mCurrentlyPressed = null;

			// manually stock the font sizes dictionary with emperically determined sizes
			mFontSizes = new Dictionary<eFont, Vector2>();
			mFontSizes.Add( eFont.Consolas16, new Vector2( 12.0f, 19.0f ) );
		}

		public void Init()
		{
			mSimpleDraw = XSimpleDraw.GetInstance( xeSimpleDrawType.ScreenSpace_Transient );
			((XIBroadcaster< XTouch.SinglePokeData >)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_SinglePoke );
		}

		public void Update( GameTime t )
		{
			// things to check for:
			// - currently pressed button might trigger a button up
			// - currently pressed button might get aborted by touch gesture or drift
			// - curretnly pressed button might still be pressed
			// - on press start, a new button may be pressed

			XTouch.SinglePokeData data =    (mListener_SinglePoke.GetNumEvents() > 0)   ?
											mListener_SinglePoke.ReadNext()				:
											null                                        ;

			if ( mCurrentlyPressed != null )
			{
				XUtils.Assert( data != null, "should have hold, end, or abort" );

				if( data.mDetail == XTouch.ePokeDetail.Hold )
				{
					if( mCurrentlyPressed.Contains( data.mCurrentPos ) )
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
				else if( data.mDetail == XTouch.ePokeDetail.End_Abort )
				{
					// touch decided this gesture is no good, un-press
					SendButtonEvent( ButtonEvent.Type.Abort );
				}
				else if( data.mDetail == XTouch.ePokeDetail.End_Normal )
				{
					// this is a pressed button
					SendButtonEvent( ButtonEvent.Type.Up );
				}
				else
				{
					XUtils.Assert( false, "not expecting another state when mCurrentlyPressed is valid" );
				}
			}
			else if( data != null && data.mDetail == XTouch.ePokeDetail.Start )
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

		public void Draw()
		{
			var enumerator = mButtons.GetEnumerator();
			
			while( enumerator.MoveNext() )
			{
				enumerator.Current.Value.Draw( mSimpleDraw );
			}
		}

		private long NextID()
		{
			++sPrevID;
			return sPrevID;
		}
	}
}
