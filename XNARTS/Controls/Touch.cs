using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace XNARTS
{

	public class XTouch : XSingleton< XTouch >
	{
		public enum xeGestureType
		{
			None,
			SingleTap,
			SingleHold,
			SingleDrag,
			MultiTap,
			MultiHold,
			MultiDrag
		}


		public class SingleTapData
		{
			public xCoord mScreenPos;
		}


		public class SingleHoldData
		{
			public xCoord mScreenPos;
		}


		public class SingleDragData
		{
			public List< xCoord > mPoints;
		}


		public class MultiTapData
		{
			public xCoord mAvgScreenPos;
		}


		public class MultiHoldData
		{
			public xCoord mAvgScreenPos;
		}


		public class tMultiDragDatum
		{
			public xCoord	mAvgScreenPos;
			public float    mMaxScreenSeparation;
		}


		public class MultiDragData
		{
			public List< tMultiDragDatum > mPoints;
		}


		private SingleTapData		mSingleTapData;
		private SingleHoldData		mSingleHoldData;
		private SingleDragData		mSingleDragData;
		private MultiTapData		mMultiTapData;
		private MultiHoldData		mMultiHoldData;
		private MultiDragData		mMultiDragData;


		private enum eContactChange
		{
			// initial state
			InitialContacts,
			NoInitialContacts,

			// number of contacts
			NoChange,
			ZeroToOne,
			ZeroToSome,
			OneToSome,
			OneToZero,
			SomeToOne,
			SomeToZero,

			// contact motion
			StillToMoving
		}

		private enum eContactCount
		{
			Unknown,
			Zero,
			One,
			Some
		}

		private XStateMachine< eContactChange > mStateMachine;
		private eContactCount					mPrevContactCount;
		

		// private constructor for XSingleton
		private XTouch()
		{}


		private void State_NoContacts()
		{
			Console.WriteLine( "no contacts" );
		}
		private void State_TrackingSinglePoke()
		{
			Console.WriteLine( "tracking single poke" );
		}
		private void State_TrackingSingleDrag()
		{ }
		private void State_TrackingMultiPoke()
		{
			Console.WriteLine( "tracking multi poke" );
		}
		private void State_TrackingMultiDrag()
		{ }
		private void State_IgnoringContacts()
		{ }
		private void State_Start()
		{ }
		private void Transition_NoContacts_TrackingSinglePoke()
		{ }
		private void Transition_NoContacts_TrackingMultiPoke()
		{ }
		private void Transition_TrackingSinglePoke_TrackingSingleDrag()
		{ }
		private void Transition_TrackingSinglePoke_NoContacts()
		{
			Console.WriteLine( "poke!?" );
		}
		private void Transition_TrackingSinglePoke_TrackingMultiPoke()
		{ }
		private void Transition_TrackingSingleDrag_TrackingMultiDrag()
		{ }
		private void Transition_TrackingSingleDrag_NoContacts()
		{ }
		private void Transition_TrackingMultiPoke_NoContacts()
		{
			Console.WriteLine( "multi poke?!" );
		}
		private void Transition_TrackingMultiPoke_TrackingSinglePoke()
		{
			Console.WriteLine( "leaving multi for single poke tracking" );
		}
		private void Transition_Trivial()
		{ }


		public void Init()
		{
			mPrevContactCount = eContactCount.Unknown;
			mStateMachine = new XStateMachine<eContactChange>();

			txStateID start =                   mStateMachine.CreateState( State_Start );
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
			mStateMachine.CreateTransition( tracking_single_drag, no_contacts, eContactChange.OneToZero, Transition_TrackingSingleDrag_NoContacts );

			mStateMachine.CreateTransition( tracking_multi_poke, no_contacts, eContactChange.SomeToZero, Transition_TrackingMultiPoke_NoContacts );
			// trivial transitions below here, add interesting code later
			mStateMachine.CreateTransition( tracking_multi_poke, tracking_multi_drag, eContactChange.StillToMoving, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_poke, tracking_single_poke, eContactChange.SomeToOne, Transition_TrackingMultiPoke_TrackingSinglePoke );

			mStateMachine.CreateTransition( tracking_multi_drag, no_contacts, eContactChange.SomeToZero, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_drag, ignoring_contacts, eContactChange.SomeToOne, Transition_Trivial );

			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.OneToZero, Transition_Trivial );
			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.SomeToZero, Transition_Trivial );

			mStateMachine.CreateTransition( start, no_contacts, eContactChange.NoInitialContacts, Transition_Trivial );
			mStateMachine.CreateTransition( start, ignoring_contacts, eContactChange.InitialContacts, Transition_Trivial );

			mStateMachine.SetStartingState( start );
			mStateMachine.Log();
		}


		private eContactChange UpdateTouchCount( Microsoft.Xna.Framework.Input.Touch.TouchCollection touches )
		{
			int num_touches = touches.Count();
			eContactChange count_change = eContactChange.NoChange;

			eContactCount new_count =	num_touches > 1 ? eContactCount.Some : 
										num_touches > 0 ? eContactCount.One :
										eContactCount.Zero;
			
			if( mPrevContactCount == eContactCount.Zero )
			{
				if( new_count == eContactCount.One )
				{
					count_change = eContactChange.ZeroToOne;
				}
				else if( new_count == eContactCount.Some )
				{
					count_change = eContactChange.ZeroToSome;
				}
			}
			else if( mPrevContactCount == eContactCount.One )
			{
				if( new_count == eContactCount.Zero )
				{
					count_change = eContactChange.OneToZero;
				}
				else if( new_count == eContactCount.Some )
				{
					count_change = eContactChange.OneToSome;
				}
			}
			else if( mPrevContactCount == eContactCount.Some )
			{
				if( new_count == eContactCount.Zero )
				{
					count_change = eContactChange.SomeToZero;
				}
				else if( new_count == eContactCount.One )
				{
					count_change = eContactChange.SomeToOne;
				}
			}
			else if( mPrevContactCount == eContactCount.Unknown )
			{
				count_change =	(new_count == eContactCount.Zero) ? 
								eContactChange.NoInitialContacts : 
								eContactChange.InitialContacts;
			}
			else
			{
				XUtils.Assert( false );
			}

			mPrevContactCount = new_count;
			return count_change;
		}


		private void LogTouches()
		{
			Microsoft.Xna.Framework.Input.Touch.TouchCollection touches = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var count = touches.Count();

			if ( count > 0 )
			{
				Console.WriteLine( "touches" );
				Console.WriteLine( count.ToString() );
			}

			for ( int i = 0; i < count; ++i )
			{
				Console.WriteLine( touches[ i ].ToString() );
			}
		}


		public void Update( GameTime game_time )
		{
			Microsoft.Xna.Framework.Input.Touch.TouchCollection touches = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			eContactChange count_change = UpdateTouchCount( touches );
			mStateMachine.ProcessTrigger( count_change );
			mStateMachine.Update();
		}

	}
}
