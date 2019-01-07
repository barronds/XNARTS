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

		private enum eContactChange
		{
			ZeroToOne,
			ZeroToSome,
			OneToSome,
			OneToZero,
			SomeToOne,
			SomeToZero,
			StillToMoving,
		}

		private XStateMachine< eContactChange > mStateMachine;
		

		// private constructor for XSingleton
		private XTouch()
		{}


		private void State_NoContacts()
		{ }


		private void State_TrackingSinglePoke()
		{ }


		private void State_TrackingSingleDrag()
		{ }


		private void State_TrackingMultiPoke()
		{ }


		private void State_TrackingMultiDrag()
		{ }


		private void State_IgnoringContacts()
		{ }


		private void Transition_NoContacts_TrackingSinglePoke()
		{ }


		private void Transition_NoContacts_TrackingMultiPoke()
		{ }


		private void Transition_TrackingSinglePoke_TrackingSingleDrag()
		{ }


		private void Transition_TrackingSinglePoke_NoContacts()
		{ }


		private void Transition_TrackingSinglePoke_TrackingMultiPoke()
		{ }


		private void Transition_TrackingSingleDrag_TrackingMultiDrag()
		{ }


		public void Init()
		{
			mStateMachine = new XStateMachine<eContactChange>();

			txStateID no_contacts =				mStateMachine.CreateState( State_NoContacts );
			txStateID tracking_single_poke =	mStateMachine.CreateState( State_TrackingSinglePoke );
			txStateID tracking_single_drag =    mStateMachine.CreateState( State_TrackingSingleDrag );
			txStateID tracking_multi_poke =     mStateMachine.CreateState( State_TrackingMultiPoke );
			txStateID tracking_multi_drag =     mStateMachine.CreateState( State_TrackingMultiDrag );
			txStateID ignoring_contacts =       mStateMachine.CreateState( State_IgnoringContacts );

			mStateMachine.CreateTransition( no_contacts, tracking_single_poke, eContactChange.ZeroToOne, Transition_NoContacts_TrackingSinglePoke );
			mStateMachine.CreateTransition( no_contacts, tracking_multi_poke, eContactChange.ZeroToSome, Transition_NoContacts_TrackingMultiPoke );

			mStateMachine.CreateTransition( tracking_single_poke, tracking_single_drag, eContactChange.StillToMoving, Transition_TrackingSinglePoke_TrackingSingleDrag );
			mStateMachine.CreateTransition( tracking_single_poke, no_contacts, eContactChange.OneToZero, Transition_TrackingSinglePoke_NoContacts );
			mStateMachine.CreateTransition( tracking_single_poke, tracking_multi_poke, eContactChange.OneToSome, Transition_TrackingSinglePoke_TrackingMultiPoke );

			mStateMachine.CreateTransition( tracking_single_drag, tracking_multi_drag, eContactChange.OneToSome, Transition_TrackingSingleDrag_TrackingMultiDrag );
			// Continue ...

			mStateMachine.Log();
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
