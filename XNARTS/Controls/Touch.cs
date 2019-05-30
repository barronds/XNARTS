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
		public enum ePokeDetail
		{
			Invalid = -1,

			Start,
			Hold,
			End_Normal,
			End_Abort,

			Num,
		}

		public class SinglePokeData
		{
			public SinglePokeData( ePokeDetail detail, Vector2 start_pos, int frame_count )
			{
				mCurrentPosValid = false;
				mStartPos = start_pos;
				mDetail = detail;
				mFrameCount = frame_count;
			}
			public SinglePokeData( ePokeDetail detail, Vector2 start_pos, Vector2 current_pos, int frame_count )
			{
				mCurrentPosValid = true;
				mDetail = detail;
				mStartPos = start_pos;
				mCurrentPos = current_pos;
				mFrameCount = frame_count;
			}

			public void Log()
			{
				Console.WriteLine(	"SinglePokeData " + mDetail.ToString() +
									" start " + mStartPos.ToString() +
									" current " + mCurrentPos.ToString() +
									" curr_valid " + mCurrentPosValid +
									" frames " + mFrameCount );
			}

			public Vector2		mStartPos;
			public Vector2		mCurrentPos;
			public ePokeDetail  mDetail;
			public bool         mCurrentPosValid;
			public int          mFrameCount;
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
		public class MultiDragData
		{
			public Vector2  mAvgScreenPos;
			public double   mMaxScreenSeparation;
			public int      mFrameCount;

			public MultiDragData( Vector2 pos, double separation, int frame_count )
			{
				mAvgScreenPos = pos;
				mMaxScreenSeparation = separation;
				mFrameCount = frame_count;
			}
		}
		public class FiveContacts
		{}

		// broadcasters
		private XBroadcaster<MultiDragData>     mBroadcaster_MultiDrag;
		private XBroadcaster<SinglePokeData>    mBroadcaster_SinglePoke;
		private XBroadcaster< FiveContacts >    mBroadcaster_FiveContacts;

		public XBroadcaster< MultiDragData > GetBroadcaster_MultiDrag()
		{
			return mBroadcaster_MultiDrag;
		}
		public XBroadcaster< SinglePokeData > GetBroadcaster_SinglePoke()
		{
			return mBroadcaster_SinglePoke;
		}
		public XBroadcaster< FiveContacts > GetBroadcaster_FiveContacts()
		{
			return mBroadcaster_FiveContacts;
		}

		private enum eContactChange
		{
			// initial state
			InitialContacts,
			NoInitialContacts,

			// number of contacts
			NoChange,
			ZeroToOne,
			ZeroToTwo,
			ZeroToMany,
			OneToTwo,
			OneToMany,
			OneToZero,
			TwoToOne,
			TwoToMany,
			TwoToZero,
			ManyToOne,
			ManyToTwo,
			ManyToZero,
			AnyToFive,
			FiveToZero,
			FiveToAnyPositive,

			// contact motion
			StillToMoving,

			// processed transitions
			SingleDragToMultiDrag,
			SingleDragToMultiDragTooSlow
		}
		private enum eContactCount
		{
			Unknown,
			Zero,
			One,
			Two,
			Five,
			Many // actually 3, 4 or more than 5
		}

		// members for XTouch
		private XStateMachine< eContactChange >							mStateMachine;
		private Microsoft.Xna.Framework.Input.Touch.TouchCollection		mTouches;
		private eContactCount											mContactCount;
		private GameTime                                                mCurrentGameTime;

		// gesture members, maybe not ne (interrupted, what was i saying here???)

		// data for states
		private double		mSingleDragStartTimeSec;
		private Vector2     mSinglePoke_StartPos;
		private int			mSinglePoke_FrameCount;
		private Vector2		mMultiPoke_StartPos;
		private double		mMultiPoke_StartMaxSeparation;
		private int			mMultiDrag_FrameCount;

		// private constructor for XSingleton
		private XTouch()
		{
			mBroadcaster_MultiDrag = new XBroadcaster<MultiDragData>();
			mBroadcaster_SinglePoke = new XBroadcaster<SinglePokeData>();
			mBroadcaster_FiveContacts = new XBroadcaster<FiveContacts>();
		}

		// state/transition helper functions
		private eContactChange UpdateTouchCount()
		{
			int num_touches = mTouches.Count();
			eContactCount prev_contact_count = mContactCount;
			eContactChange count_change = eContactChange.NoChange;

			eContactCount new_count =   num_touches == 5 ? eContactCount.Five :
										num_touches > 2 ? eContactCount.Many :
										num_touches > 1 ? eContactCount.Two :
										num_touches > 0 ? eContactCount.One :
										eContactCount.Zero;

			if ( prev_contact_count == eContactCount.Zero )
			{
				count_change = new_count == eContactCount.One ? eContactChange.ZeroToOne :
								new_count == eContactCount.Two ? eContactChange.ZeroToTwo :
								new_count == eContactCount.Many ? eContactChange.ZeroToMany :
								new_count == eContactCount.Five ? eContactChange.AnyToFive :
								eContactChange.NoChange;
			}
			else if ( prev_contact_count == eContactCount.One )
			{
				count_change = new_count == eContactCount.Zero ? eContactChange.OneToZero :
								new_count == eContactCount.Two ? eContactChange.OneToTwo :
								new_count == eContactCount.Many ? eContactChange.OneToMany :
								new_count == eContactCount.Five ? eContactChange.AnyToFive :
								eContactChange.NoChange;
			}
			else if ( prev_contact_count == eContactCount.Two )
			{
				count_change = new_count == eContactCount.Zero ? eContactChange.TwoToZero :
								new_count == eContactCount.One ? eContactChange.TwoToOne :
								new_count == eContactCount.Many ? eContactChange.TwoToMany :
								new_count == eContactCount.Five ? eContactChange.AnyToFive :
								eContactChange.NoChange;
			}
			else if ( prev_contact_count == eContactCount.Many )
			{
				count_change = new_count == eContactCount.Zero ? eContactChange.ManyToZero :
								new_count == eContactCount.One ? eContactChange.ManyToOne :
								new_count == eContactCount.Two ? eContactChange.ManyToTwo :
								new_count == eContactCount.Five ? eContactChange.AnyToFive :
								eContactChange.NoChange;
			}
			else if( prev_contact_count == eContactCount.Five )
			{
				count_change =	new_count == eContactCount.Five ? eContactChange.NoChange : 
								new_count == eContactCount.Zero ? eContactChange.FiveToZero :
								eContactChange.FiveToAnyPositive;
			}
			else if ( prev_contact_count == eContactCount.Unknown )
			{
				count_change = (new_count == eContactCount.Zero) ?
								eContactChange.NoInitialContacts :
								eContactChange.InitialContacts;
			}
			else
			{
				XUtils.Assert( false );
			}

			mContactCount = new_count;
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
		private Vector2 GetSingleTouchPos()
		{
			XUtils.Assert( mTouches.Count == 1 );
			return mTouches[ 0 ].Position;
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
		private void EnterSinglePoke()
		{
			mSinglePoke_StartPos = GetSingleTouchPos();
			mSinglePoke_FrameCount = 0;
		}
		private void ExitSinglePoke( ePokeDetail detail, bool current_pos_valid )
		{
			SinglePokeData data =	current_pos_valid																				? 
									new SinglePokeData( detail, mSinglePoke_StartPos, GetSingleTouchPos(), mSinglePoke_FrameCount )	:
									new SinglePokeData( detail, mSinglePoke_StartPos, mSinglePoke_FrameCount )						;

			mBroadcaster_SinglePoke.Post( data );
		}
		private void EnterSingleDrag()
		{
			mSingleDragStartTimeSec = mCurrentGameTime.TotalGameTime.TotalSeconds;
		}
		private void EnterMultiPoke()
		{
			mMultiPoke_StartPos = CalcAvgTouchPos();
			mMultiPoke_StartMaxSeparation = CalcMaxSeparation();
		}
		private void StartMultiDrag()
		{
			mMultiDrag_FrameCount = 0;
		}

		// state update functions
		private eContactChange State_NoContacts()
		{
			//Console.WriteLine( "no contacts" );
			return eContactChange.NoChange;
		}
		private eContactChange State_SinglePoke()
		{
			double k_drift_thresh_sqr = XMath.Sqr( 45d );
			Vector2 current_pos = GetSingleTouchPos();
			Vector2 drift = current_pos - mSinglePoke_StartPos;

			if( drift.LengthSquared() > k_drift_thresh_sqr )
			{
				return eContactChange.StillToMoving;
			}

			ePokeDetail detail = mSinglePoke_FrameCount == 0 ? ePokeDetail.Start : ePokeDetail.Hold;
			SinglePokeData data = new SinglePokeData( detail, mSinglePoke_StartPos, current_pos, mSinglePoke_FrameCount );
			mBroadcaster_SinglePoke.Post( data );
			++mSinglePoke_FrameCount;

			return eContactChange.NoChange;
		}
		private eContactChange State_SingleDrag()
		{
			// we can go quickly to multidrag but if in single drag deliberately, then no
			const double k_thresh_seconds = 0.1;

			if( mContactCount == eContactCount.Two )
			{
				double elapsed = mCurrentGameTime.TotalGameTime.TotalSeconds - mSingleDragStartTimeSec;

				eContactChange change = (elapsed > k_thresh_seconds)				?
										eContactChange.SingleDragToMultiDragTooSlow :
										eContactChange.SingleDragToMultiDrag        ;

				return change;
			}

			return eContactChange.NoChange;
		}
		private eContactChange State_MultiPoke()
		{
			//Console.WriteLine( "tracking multi poke" );

			// tuning belongs in config
			Vector2 pos = CalcAvgTouchPos();
			const double kDragMoveDist = 6.0;

			if( (pos - mMultiPoke_StartPos).LengthSquared() > kDragMoveDist * kDragMoveDist )
			{
				return eContactChange.StillToMoving;
			}

			const double kDragSeparationChange = 12.0;
			double separation = CalcMaxSeparation();

			if( Math.Abs( separation - mMultiPoke_StartMaxSeparation ) > kDragSeparationChange )
			{
				return eContactChange.StillToMoving;
			}

			return eContactChange.NoChange;
		}
		private eContactChange State_MultiDrag()
		{
			var data = new MultiDragData( CalcAvgTouchPos(), CalcMaxSeparation(), mMultiDrag_FrameCount );
			mBroadcaster_MultiDrag.Post( data );
			++mMultiDrag_FrameCount;
			return eContactChange.NoChange;
		}
		private eContactChange State_IgnoringContacts()
		{
			return eContactChange.NoChange;
		}
		private eContactChange State_Start()
		{
			return eContactChange.NoChange;
		}
		private eContactChange State_5Contacts()
		{
			return eContactChange.NoChange;
		}

		// transition functions
		private void Transition_NoContacts_SinglePoke()
		{
			EnterSinglePoke();
		}
		private void Transition_NoContacts_MultiPoke()
		{
			EnterMultiPoke();
		}
		private void Transition_SinglePoke_SingleDrag()
		{
			ExitSinglePoke( ePokeDetail.End_Abort, true );
			EnterSingleDrag();
		}
		private void Transition_SinglePoke_IgnoringContacts()
		{
			ExitSinglePoke( ePokeDetail.End_Abort, false );
		}
		private void Transition_SinglePoke_NoContacts()
		{
			ExitSinglePoke( ePokeDetail.End_Normal, false );
		}
		private void Transition_SinglePoke_MultiPoke()
		{
			ExitSinglePoke( ePokeDetail.End_Abort, false );
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
		}
		private void Transition_5Contacts_Any()
		{
			mBroadcaster_FiveContacts.Post( new FiveContacts() );
		}
		private void Transition_Trivial()
		{ }

		public void Init()
		{
			mCurrentGameTime = new GameTime();
			mContactCount = eContactCount.Unknown;
			mMultiPoke_StartMaxSeparation = 0d;
			mMultiPoke_StartMaxSeparation = 0d;
			mStateMachine = new XStateMachine<eContactChange>( eContactChange.NoChange );

			txStateID start =                   mStateMachine.CreateState( State_Start );
			txStateID no_contacts =				mStateMachine.CreateState( State_NoContacts );
			txStateID tracking_single_poke =	mStateMachine.CreateState( State_SinglePoke );
			txStateID tracking_single_drag =    mStateMachine.CreateState( State_SingleDrag );
			txStateID tracking_multi_poke =     mStateMachine.CreateState( State_MultiPoke );
			txStateID tracking_multi_drag =     mStateMachine.CreateState( State_MultiDrag );
			txStateID ignoring_contacts =       mStateMachine.CreateState( State_IgnoringContacts );
			txStateID tracking_5_contacts =     mStateMachine.CreateState( State_5Contacts );

			mStateMachine.CreateTransition( no_contacts, tracking_single_poke, eContactChange.ZeroToOne, Transition_NoContacts_SinglePoke );
			mStateMachine.CreateTransition( no_contacts, tracking_multi_poke, eContactChange.ZeroToTwo, Transition_NoContacts_MultiPoke );
			mStateMachine.CreateTransition( no_contacts, ignoring_contacts, eContactChange.ZeroToMany, Transition_Trivial );
			mStateMachine.CreateTransition( no_contacts, tracking_5_contacts, eContactChange.AnyToFive, Transition_Trivial );

			mStateMachine.CreateTransition( tracking_single_poke, tracking_single_drag, eContactChange.StillToMoving, Transition_SinglePoke_SingleDrag );
			mStateMachine.CreateTransition( tracking_single_poke, no_contacts, eContactChange.OneToZero, Transition_SinglePoke_NoContacts );
			mStateMachine.CreateTransition( tracking_single_poke, tracking_multi_poke, eContactChange.OneToTwo, Transition_SinglePoke_MultiPoke );
			mStateMachine.CreateTransition( tracking_single_poke, ignoring_contacts, eContactChange.OneToMany, Transition_SinglePoke_IgnoringContacts );
			mStateMachine.CreateTransition( tracking_single_poke, tracking_5_contacts, eContactChange.AnyToFive, Transition_Trivial );

			mStateMachine.CreateTransition( tracking_single_drag, tracking_multi_drag, eContactChange.SingleDragToMultiDrag, Transition_SingleDrag_MultiDrag );
			mStateMachine.CreateTransition( tracking_single_drag, no_contacts, eContactChange.OneToZero, Transition_SingleDrag_NoContacts );
			mStateMachine.CreateTransition( tracking_single_drag, ignoring_contacts, eContactChange.OneToMany, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_single_drag, ignoring_contacts, eContactChange.SingleDragToMultiDragTooSlow, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_single_drag, tracking_5_contacts, eContactChange.AnyToFive, Transition_Trivial );

			mStateMachine.CreateTransition( tracking_multi_poke, no_contacts, eContactChange.TwoToZero, Transition_MultiPoke_NoContacts );
			mStateMachine.CreateTransition( tracking_multi_poke, tracking_multi_drag, eContactChange.StillToMoving, Transition_MultiPoke_MultiDrag );
			mStateMachine.CreateTransition( tracking_multi_poke, ignoring_contacts, eContactChange.TwoToOne, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_poke, ignoring_contacts, eContactChange.TwoToMany, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_poke, tracking_5_contacts, eContactChange.AnyToFive, Transition_Trivial );

			mStateMachine.CreateTransition( tracking_multi_drag, no_contacts, eContactChange.TwoToZero, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_drag, ignoring_contacts, eContactChange.TwoToOne, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_drag, ignoring_contacts, eContactChange.TwoToMany, Transition_Trivial );
			mStateMachine.CreateTransition( tracking_multi_drag, tracking_5_contacts, eContactChange.AnyToFive, Transition_Trivial );

			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.OneToZero, Transition_Trivial );
			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.TwoToZero, Transition_Trivial );
			mStateMachine.CreateTransition( ignoring_contacts, no_contacts, eContactChange.ManyToZero, Transition_Trivial );
			mStateMachine.CreateTransition( ignoring_contacts, tracking_5_contacts, eContactChange.AnyToFive, Transition_Trivial );

			mStateMachine.CreateTransition( start, no_contacts, eContactChange.NoInitialContacts, Transition_Trivial );
			mStateMachine.CreateTransition( start, ignoring_contacts, eContactChange.InitialContacts, Transition_Trivial );

			mStateMachine.CreateTransition( tracking_5_contacts, no_contacts, eContactChange.FiveToZero, Transition_5Contacts_Any );
			mStateMachine.CreateTransition( tracking_5_contacts, ignoring_contacts, eContactChange.FiveToAnyPositive, Transition_5Contacts_Any );

			mStateMachine.SetStartingState( start );
		}

		public void Update( GameTime game_time )
		{
			mCurrentGameTime = game_time;

			// cache touches every update in case it's expensive to get
			mTouches = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();

			// gather what is needed to update triggers first
			eContactChange count_change = UpdateTouchCount();

			// update state machine last, possibly in new state
			mStateMachine.Update( count_change );
		}
	}
}
