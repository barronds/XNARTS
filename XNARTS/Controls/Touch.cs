using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


// detect one finger pokes, 2 finger zoom-drags, ...


namespace XNARTS
{
	public class XTouch : XSingleton< XTouch >
	{
		public enum xeGestureType
		{
			None,
			SinglePoke,
			SingleHold,
			SingleDrag,
			MultiDrag
		}


		public struct xSinglePokeData
		{
			public xCoord mScreenPos;
		}


		public struct xSingleHoldData
		{
			public xCoord mScreenPos;
		}


		public struct xSingleDragData
		{
			public xCoord mLastScreenPos;
			public xCoord mCurrentScreenPos;
		}


		public struct xMultiDragData
		{
			public xCoord		mLastAverageScreenPos;
			public xCoord		mCurrentAverageScreenPos;
			public float		mLastMaxScreenSeparation;
			public float		mCurrentMaxScreenSeparation;
		}


		private xSinglePokeData		mSinglePokeData;
		private xSingleHoldData		mSingleHoldData;
		private xSingleDragData		mSingleDragData;
		private xMultiDragData      mMultiDragData;
		

		// private constructor for XSingleton
		private XTouch()
		{}


		public void Init()
		{
		}


		public void Update( GameTime game_time )
		{
			// when we go from no contacts to one, start tracking a poke.  if it changes to a drag, start tracking that instead.  
			// if a second or successive finger joins in in time, track as a multidrag.  if at anytime we go from
			// no touches to two or more touches, start tracking a multidrag right away.
			// we should use a state machine for this...  going to build one now.


			Microsoft.Xna.Framework.Input.Touch.TouchCollection touches = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var count = touches.Count();
			if( count > 0 )
			{
				Console.WriteLine( "touches" );
				Console.WriteLine( count.ToString() );
			}

			for ( int i = 0; i < count; ++i )
			{
				Console.WriteLine( touches[ i ].ToString() );
			}
		}

	}
}
