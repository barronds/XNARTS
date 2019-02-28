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
		public class MultiPokeData
		{
			public xCoord mAvgScreenPos;
		}
		public class MultiDragStart
		{
			public MultiDragStart( Vector2 pos, double separation )
			{
				mData = new MultiDragData( pos, separation );
			}

			public MultiDragData mData;
		}
		public class MultiDragData
		{
			public Vector2  mAvgScreenPos;
			public double   mMaxScreenSeparation;

			public MultiDragData( Vector2 pos, double separation )
			{
				mAvgScreenPos = pos;
				mMaxScreenSeparation = separation;
			}
		}

		// broadcasters
		public XBroadcaster<MultiDragStart> mBroadcaster_MultiDragStart { get; }
		public XBroadcaster<MultiDragData>	mBroadcaster_MultiDrag { get; }

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

		// members for XTouch
		private XStateMachine< eContactChange >							mStateMachine;
		private Microsoft.Xna.Framework.Input.Touch.TouchCollection		mTouches;
		private eContactCount											mPrevContactCount;

		// gesture members, maybe not ne (interrupted, what was i saying here???)

		// data for states
		private Vector2 mMultiPoke_StartPos;
		private double	mMultiPoke_StartMaxSeparation;

		// private constructor for XSingleton
		private XTouch()
		{
			mBroadcaster_MultiDragStart = new XBroadcaster<MultiDragStart>();
			mBroadcaster_MultiDrag = new XBroadcaster<MultiDragData>();
		}

		// state/transition helper functions
		private Vector2 CalcAvgTouchPos()
		{
			XUtils.Assert( mTouches.Count > 0 );
			Vector2 result = Vector2.Zero;

			for ( int i = 0; i < mTouches.Count; ++i )
			{
				result += mTouches[ i ].Position;
			}

			return (1.0f / mTouches.Count) * result;
		}
		private double CalcMaxSeparation()
		{
			XUtils.Assert( mTouches.Count > 0 );
			float max_separation_sqr = 0f;

			for( int i = 0; i < mTouches.Count; ++i )
			{
				for( int j = i + 1; j < mTouches.Count; ++j )
				{
					float separation_sqr = (mTouches[ i ].Position - mTouches[ j ].Position).LengthSquared();
					max_separation_sqr = Math.Max( separation_sqr, max_separation_sqr );
				}
			}

			return Math.Sqrt( max_separation_sqr );
		}
		private void EnterMultiPoke()
		{
			mMultiPoke_StartPos = CalcAvgTouchPos();
			mMultiPoke_StartMaxSeparation = CalcMaxSeparation();
		}
		private void StartMultiDrag()
		{
			var data = new MultiDragStart( CalcAvgTouchPos(), CalcMaxSeparation() );
			mBroadcaster_MultiDragStart.Post( data );
		}


		// state update functions
		private void State_NoContacts()
		{
			//Console.WriteLine( "no contacts" );
		}
		private void State_SinglePoke()
		{
			//Console.WriteLine( "tracking single poke" );
		}
		private void State_SingleDrag()
		{ }
		private void State_MultiPoke()
		{
			//Console.WriteLine( "tracking multi poke" );

			// tuning belongs in config
			Vector2 pos = CalcAvgTouchPos();
			const double kDragMoveDist = 6.0;

			if( (pos - mMultiPoke_StartPos).LengthSquared() > kDragMoveDist * kDragMoveDist )
			{
				mStateMachine.ProcessTrigger( eContactChange.StillToMoving );
				//Console.WriteLine( "dist" );
				return;
			}

			const double kDragSeparationChange = 12.0;
			double separation = CalcMaxSeparation();

			if( Math.Abs( separation - mMultiPoke_StartMaxSeparation ) > kDragSeparationChange )
			{
				mStateMachine.ProcessTrigger( eContactChange.StillToMoving );
				//Console.WriteLine( "spread" );

				return;
			}
		}
		private void State_MultiDrag()
		{
			//Console.WriteLine( "tracking multi drag" );
			var data = new MultiDragData( CalcAvgTouchPos(), CalcMaxSeparation() );
			mBroadcaster_MultiDrag.Post( data );
		}
		private void State_IgnoringContacts()
		{ }
		private void State_Start()
		{ }

		// transition functions
		private void Transition_NoContacts_SinglePoke()
		{ }
		private void Transition_NoContacts_MultiPoke()
		{
			EnterMultiPoke();
		}
		private void Transition_SinglePoke_SingleDrag()
		{ }
		private void Transition_SinglePoke_NoContacts()
		{
			//Console.WriteLine( "poke!?" );
		}
		private void Transition_SinglePoke_MultiPoke()
		{
			EnterMultiPoke();
		}
		private void Transition_SingleDrag_MultiDrag()
		{
			StartMultiDrag();
		}
		private void Transition_MultiPoke_MultiDrag()
		{
			StartMultiDrag();
		}
		private void Transition_SingleDrag_NoContacts()
		{ }
		private void Transition_MultiPoke_NoContacts()
		{
			//Console.WriteLine( "multi poke?!" );
		}
		private void Transition_MultiPoke_SinglePoke()
		{
			//Console.WriteLine( "leaving multi for single poke tracking" );
		}
		private void Transition_Trivial()
		{ }

		// update helpers
		private eContactChange UpdateTouchCount()
		{
			int num_touches = mTouches.Count();
			eContactChange count_change = eContactChange.NoChange;

			eContactCount new_count =   num_touches > 1 ? eContactCount.Some :
										num_touches > 0 ? eContactCount.One :
										eContactCount.Zero;

			if ( mPrevContactCount == eContactCount.Zero )
			{
				if ( new_count == eContactCount.One )
				{
					count_change = eContactChange.ZeroToOne;
				}
				else if ( new_count == eContactCount.Some )
				{
					count_change = eContactChange.ZeroToSome;
				}
			}
			else if ( mPrevContactCount == eContactCount.One )
			{
				if ( new_count == eContactCount.Zero )
				{
					count_change = eContactChange.OneToZero;
				}
				else if ( new_count == eContactCount.Some )
				{
					count_change = eContactChange.OneToSome;
				}
			}
			else if ( mPrevContactCount == eContactCount.Some )
			{
				if ( new_count == eContactCount.Zero )
				{
					count_change = eContactChange.SomeToZero;
				}
				else if ( new_count == eContactCount.One )
				{
					count_change = eContactChange.SomeToOne;
				}
			}
			else if ( mPrevContactCount == eContactCount.Unknown )
			{
				count_change = (new_count == eContactCount.Zero) ?
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
			var count = mTouches.Count();

			if ( count > 0 )
			{
				Console.WriteLine( "touches" );
				Console.WriteLine( count.ToString() );
			}

			for ( int i = 0; i < count; ++i )
			{
				Console.WriteLine( mTouches[ i ].ToString() );
			}
		}

		public void Init()
		{
			mPrevContactCount = eContactCount.Unknown;
			mMultiPoke_StartMaxSeparation = 0d;
			mMultiPoke_StartMaxSeparation = 0d;
			mStateMachine = new XStateMachine<eContactChange>();

			txStateID start =                   mStateMachine.CreateState( State_Start );
			txStateID no_contacts =				mStateMachine.CreateState( State_NoContacts );
			txStateID tracking_single_poke =	mStateMachine.CreateState( State_SinglePoke );
			txStateID tracking_single_drag =    mStateMachine.CreateState( State_SingleDrag );
			txStateID tracking_multi_poke =     mStateMachine.CreateState( State_MultiPoke );
			txStateID tracking_multi_drag =     mStateMachine.CreateState( State_MultiDrag );
			txStateID ignoring_contacts =       mStateMachine.CreateState( State_IgnoringContacts );

			mStateMachine.CreateTransition( no_contacts, tracking_single_poke, eContactChange.ZeroToOne, Transition_NoContacts_SinglePoke );
			mStateMachine.CreateTransition( no_contacts, tracking_multi_poke, eContactChange.ZeroToSome, Transition_NoContacts_MultiPoke );

			mStateMachine.CreateTransition( tracking_single_poke, tracking_single_drag, eContactChange.StillToMoving, Transition_SinglePoke_SingleDrag );
			mStateMachine.CreateTransition( tracking_single_poke, no_contacts, eContactChange.OneToZero, Transition_SinglePoke_NoContacts );
			mStateMachine.CreateTransition( tracking_single_poke, tracking_multi_poke, eContactChange.OneToSome, Transition_SinglePoke_MultiPoke );

			mStateMachine.CreateTransition( tracking_single_drag, tracking_multi_drag, eContactChange.OneToSome, Transition_SingleDrag_MultiDrag );
			mStateMachine.CreateTransition( tracking_single_drag, no_contacts, eContactChange.OneToZero, Transition_SingleDrag_NoContacts );

			mStateMachine.CreateTransition( tracking_multi_poke, no_contacts, eContactChange.SomeToZero, Transition_MultiPoke_NoContacts );
			mStateMachine.CreateTransition( tracking_multi_poke, tracking_multi_drag, eContactChange.StillToMoving, Transition_MultiPoke_MultiDrag );
			mStateMachine.CreateTransition( tracking_multi_poke, tracking_single_poke, eContactChange.SomeToOne, Transition_MultiPoke_SinglePoke );

			mStateMachine.CreateTransition( tracking_multi_drag, no_contacts, eContactChange.SomeToZero, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_drag, ignoring_contacts, eContactChange.SomeToOne, Transition_Trivial );

			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.OneToZero, Transition_Trivial );
			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.SomeToZero, Transition_Trivial );

			mStateMachine.CreateTransition( start, no_contacts, eContactChange.NoInitialContacts, Transition_Trivial );
			mStateMachine.CreateTransition( start, ignoring_contacts, eContactChange.InitialContacts, Transition_Trivial );

			mStateMachine.SetStartingState( start );
			//mStateMachine.Log();
		}
		public void Update( GameTime game_time )
		{
			// cache touches every update in case it's expensive to get
			mTouches = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();

			// gather what is needed to update triggers first
			eContactChange count_change = UpdateTouchCount();
			mStateMachine.ProcessTrigger( count_change );

			// update state machine last, possibly in new state
			mStateMachine.Update();
		}

	}
}
