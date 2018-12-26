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
		private enum xeInputTrackingType
		{
			None,
			OneFinger,
			TwoFingers
		}


		public enum xeInputResultType
		{
			None,
			SinglePoke,
			SingleHold,
			SingleDrag,
			DoubleDragZoom
		}


		// different gesture data will be available to be polled every frame and marked valid or not.
		// at the start or end of a gesture, an event will occur and this data will be included.

		public struct xSinglePokeData
		{
			public bool		mValid;
			public int		mId;
			public xCoord	mScreenPos;
		}


		public struct xSingleHoldData
		{
			public bool			mValid;
			public int			mId;
			public GameTime		mStartTime;
			public xCoord		mScreenPos;
		}


		public struct xSingleDragData
		{
			public bool			mValid;
			public int			mId;
			public GameTime		mStartTime;
			public xCoord		mStartScreenPos;
			public xCoord		mLastScreenPos;
			public xCoord		mCurrentScreenPos;
		}


		public struct xDoubleDragZoomData
		{
			public bool			mValid;
			public int			mId;
			public GameTime		mStartTime;
			public xCoord		mStartAverageScreenPos;
			public xCoord		mLastAverageScreenPos;
			public xCoord		mCurrentAverageScreenPos;
			public float		mLastScreenSeparation;
			public float		mCurrentScreenSeparation;
		}


		// these can generally be live together.
		// eg., single drag would be active at the start of a single touch but might become also a single hold or single poke
		private xSinglePokeData			mSinglePokeData;
		private xSingleHoldData			mSingleHoldData;
		private xSingleDragData			mSingleDragData;
		private xDoubleDragZoomData		mDoubleDragZoomData;
		
		// private constructor for XSingleton
		private XTouch()
		{}


		public void Init()
		{
		}


		public void Update( GameTime game_time )
		{
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


		public xSinglePokeData GetSinglePokeData()
		{
			return mSinglePokeData;
		}
	}
}
