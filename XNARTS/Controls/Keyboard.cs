using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace XNARTS
{
	public class XKeyInput : XSingleton<XKeyInput>
	{
		public struct KeyDown
		{
			public Keys mKey;
		}
		public struct KeyUp
		{
			public Keys mKey;
		}
		public struct KeyHeld
		{
			public Keys mKey;
		}

		public XBroadcaster< KeyDown >	mBroadcaster_KeyDown;
		public XBroadcaster< KeyUp >	mBroadcaster_KeyUp;
		public XBroadcaster< KeyHeld >	mBroadcaster_KeyHeld;

		private List< Keys > mPrevPressedKeys;

		// private constructor for singleton
		private XKeyInput()
		{
			mBroadcaster_KeyDown = new XBroadcaster<KeyDown>();
			mBroadcaster_KeyUp = new XBroadcaster<KeyUp>();
			mBroadcaster_KeyHeld = new XBroadcaster<KeyHeld>();
			mPrevPressedKeys = new List<Keys>();
		}


		public void Init()
		{
		}


		public void Update()
		{
			var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var curr_pressed = state.GetPressedKeys();

			// get pressed keys
			// check if previously pressed keys are now not pressed - send key up events
			// check if previously pressed keys still pressed - send key held events
			for ( int p = 0; p < mPrevPressedKeys.Count; ++p )
			{
				bool found = false;

				for( int c = 0; c < curr_pressed.Length; ++c )
				{
					if( curr_pressed[ c ] == mPrevPressedKeys[ p ] )
					{
						found = true;
						break;
					}
				}

				if( found )
				{
					KeyHeld msg = new KeyHeld();
					msg.mKey = mPrevPressedKeys[ p ];
					mBroadcaster_KeyHeld.Post( msg );
				}
				else
				{
					KeyUp msg = new KeyUp();
					msg.mKey = mPrevPressedKeys[ p ];
					mBroadcaster_KeyUp.Post( msg );
				}
			}

			// check if previously not pressed keys are now pressed - send key down events
			for( int c = 0; c < curr_pressed.Length; ++c )
			{
				bool found = false;

				for( int p = 0; p < mPrevPressedKeys.Count; ++p )
				{
					if( mPrevPressedKeys[ p ] == curr_pressed[ c ] )
					{
						found = true;
						break;
					}
				}

				if( !found )
				{
					KeyDown msg = new KeyDown();
					msg.mKey = curr_pressed[ c ];
					mBroadcaster_KeyDown.Post( msg );
				}
			}

			// update keys prev pressed
			mPrevPressedKeys.Clear();

			for( int c = 0; c < curr_pressed.Length; ++c )
			{
				mPrevPressedKeys.Add( curr_pressed[ c ] );
			}
		}
	}
}
