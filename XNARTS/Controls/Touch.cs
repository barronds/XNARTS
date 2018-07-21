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


		public struct xDoubleDragZoomData
		{
			public xCoord mLastAverageScreenPos;
			public xCoord mCurrentAverageScreenPos;
			public float mLastScreenSeparation;
			public float mCurrentScreenSeparation;
		}


		xeInputResultType mInputResultType;
		xSinglePokeData mSinglePokeData;
		xSingleHoldData mSingleHoldData;
		xSingleDragData mSingleDragData;
		xDoubleDragZoomData mDoubleDragZoomData;

		
		// private constructor for XSingleton
		private XTouch()
		{}


		public void Init()
		{
			mInputResultType = xeInputResultType.None;
		}


		public void Update( GameTime game_time )
		{

		}


		// temporary until there are events
		public xeInputResultType GetInput()
		{
			return xeInputResultType.None;
		}


		public xSinglePokeData GetSinglePokeData()
		{
			// TODO: assert that the type is correct and return something smart
			XUtils.Assert( false );
			return new xSinglePokeData();
		}
	}
}
