using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// TODO: make txStateID more usable for clients, like operator== etc
	public struct txStateID : IComparable< txStateID >
	{
		private ulong mID;
		private static ulong sNext = 2;

		public static txStateID kNone = new txStateID( 0 );


		public int CompareTo( txStateID value )
		{
			return (mID < value.mID) ? -1 : (mID > value.mID) ? 1 : 0;
		}


		public void Init()
		{
			if ( mID > 0 )
			{
				XUtils.Assert( false, "txStateID only initializes once" );
			}

			mID = sNext;
			++sNext;
		}


		public bool Equals( txStateID rhs )
		{
			return mID == rhs.mID;
		}


		private txStateID( ulong id )
		{
			mID = id;
		}


		public override string ToString()
		{
			return "ID = " + mID.ToString();
		}
	}


	public class XStateMachine< Trigger >
	{
		// holds the state for one instance of a state machine.
		// holds the context object associated with the callbacks.
		private XState mCurrentState;
		private bool mLocked;
		private SortedDictionary< txStateID, XState > mStates;

		public delegate void Callback();


		public XStateMachine()
		{
			mCurrentState = null;
			mLocked = false;
			mStates = new SortedDictionary<txStateID, XState>();
		}


		public void SetStartingState( txStateID starting_state )
		{
			XUtils.Assert( mCurrentState == null );
			XUtils.Assert( !starting_state.Equals( txStateID.kNone ) );
			mCurrentState = GetState( starting_state );
		}


		public class XState
		{
			public txStateID mStateID;
			private Callback mUpdateCallback;
			private SortedDictionary< Trigger, XTransition > mTransitions;


			public XState( Callback callback )
			{
				mStateID.Init();
				mUpdateCallback = callback;
				mTransitions = new SortedDictionary<Trigger, XTransition>();
			}


			public void Add( Trigger trigger, XState to_state, Callback callback )
			{
				XUtils.Assert( !mTransitions.ContainsKey( trigger ) );
				XTransition transition = new XTransition( to_state, callback );
				mTransitions.Add( trigger, transition );
			}


			public void Remove( Trigger trigger )
			{
				XUtils.Assert( mTransitions.ContainsKey( trigger ) );
				mTransitions.Remove( trigger );
			}


			public void RemoveTransitionsTo( txStateID state_id )
			{
				List< Trigger > removals = new List< Trigger >();

				for( int i = 0; i < mTransitions.Count; ++i )
				{
					var transition_i = mTransitions.ElementAt( i );

					if( transition_i.Value.GetToStateID().Equals( state_id ) )
					{
						removals.Add( transition_i.Key );
					}
				}

				foreach( Trigger removal in removals )
				{
					mTransitions.Remove( removal );
				}
			}


			public void Update()
			{
				mUpdateCallback?.Invoke();
			}


			public bool ProcessTrigger( Trigger trigger, out XState to_state )
			{
				XTransition transition;

				if( mTransitions.TryGetValue( trigger, out transition ) )
				{
					to_state = transition.Trigger();
					return true;
				}

				to_state = null;
				return false;
			}


			public override string ToString()
			{
				string result = "State: " + mStateID.ToString() + ", Update Callback: " + (mUpdateCallback != null ? mUpdateCallback.Method.ToString() : "null") + "\n";

				foreach( var trigger_transition_pair in mTransitions )
				{
					result += "\t" + trigger_transition_pair.Key.ToString() + " -> state " + trigger_transition_pair.Value.ToString() + "\n";
				}

				return result;
			}
		}


		public class XTransition
		{
			private XState mToState;
			private Callback mTransitionCallback;


			public XTransition( XState to_state, Callback callback )
			{
				mToState = to_state;
				mTransitionCallback = callback;
			}


			public txStateID GetToStateID()
			{
				return mToState.mStateID;
			}


			public XState Trigger()
			{
				mTransitionCallback?.Invoke();
				return mToState;
			}


			public override string ToString()
			{
				return mToState.mStateID.ToString() + ", Transition Callback: " + (mTransitionCallback != null ? mTransitionCallback.Method.ToString() : "null");
			}
		}


		public txStateID CreateState( Callback callback )
		{
			XUtils.Assert( !mLocked, "Modifying state machine while in use not allowed." );
			XState state = new XState( callback );
			mStates.Add( state.mStateID, state );
			return state.mStateID;
		}


		public void RemoveState( txStateID state_id, txStateID new_state_if_current )
		{
			XUtils.Assert( !mLocked, "Modifying state machine while in use not allowed." );

			// if that was the current state, prepare to switch states as indicated.
			// remove all transitions to that state from other states.
			// remove state.
			XState next_state = null;

			if( mCurrentState.mStateID.Equals( state_id ) )
			{
				XUtils.Assert( !new_state_if_current.Equals( state_id ) );

				if( new_state_if_current.Equals( txStateID.kNone ) )
				{
					XUtils.Assert( mStates.Count == 1, "can only switch to no current state if removing the last state" );
					mCurrentState = null;
				}
				else
				{
					bool found_next = mStates.TryGetValue( new_state_if_current, out next_state );
					XUtils.Assert( found_next );
				}
			}

			for ( int i = 0; i < mStates.Count; ++i )
			{
				var state_i = mStates.ElementAt( i );
				state_i.Value.RemoveTransitionsTo( state_id );
			}

			if( next_state != null )
			{
				mCurrentState = next_state;
			}

			XState state;
			bool found = mStates.TryGetValue( state_id, out state );
			XUtils.Assert( found );
			mStates.Remove( state_id );
		}


		// TODO: templatize this to improve what is passed in for trigger, cast it to int
		public void CreateTransition( txStateID from, txStateID to, Trigger trigger, Callback callback )
		{
			XUtils.Assert( !mLocked, "Modifying state machine while in use not allowed." );

			// make sure 'from' already exists
			// make sure 'from' state doesn't already have that trigger (Add() will do that)
			XState from_state, to_state;
			bool found_from = mStates.TryGetValue( from, out from_state );
			bool found_to = mStates.TryGetValue( to, out to_state );
			XUtils.Assert( found_from && found_to );

			from_state.Add( trigger, to_state, callback );
		}


		public void RemoveTransition( txStateID from, Trigger trigger )
		{
			XUtils.Assert( !mLocked, "Modifying state machine while in use not allowed." );

			XState from_state;
			bool found_from = mStates.TryGetValue( from, out from_state );
			XUtils.Assert( found_from );
			from_state.Remove( trigger );
		}


		public void Update()
		{
			mLocked = true;

			if( mCurrentState != null )
			{
				mCurrentState.Update();
			}

			mLocked = false;
		}


		public void ProcessTrigger( Trigger trigger )
		{
			mLocked = true;
			XState change;

			if( mCurrentState != null )
			{
				bool transition = mCurrentState.ProcessTrigger( trigger, out change );

				if ( transition )
				{
					mCurrentState = change;
				}
			}

			mLocked = false;
		}


		private XState GetState( txStateID state_id )
		{
			XState state;
			bool found = mStates.TryGetValue( state_id, out state );
			XUtils.Assert( found );
			return state;
		}


		public override string ToString()
		{
			string result = "State Machine Current State: " + (mCurrentState != null ? mCurrentState.mStateID.ToString() : "null") + "\nStates:\n";

			foreach( var stateID_state_pair in mStates )
			{
				result += stateID_state_pair.Value.ToString() + "\n";
			}

			return result;
		}


		public void Log( string msg = null )
		{
			Console.WriteLine( (msg != null ? msg + ": " : "") + ToString() );
		}
	}


	public class XStateMachineUnitTest
	{
		private static XStateMachineUnitTest sUnitTest;


		public XStateMachineUnitTest()
		{ }


		enum eTriggers
		{
			Jump,
			Poke
		}


		public void Cb1()
		{
			//Console.WriteLine( "Cb1" );
		}


		public void Cb2()
		{
			//Console.WriteLine( "Cb2" );
		}


		public static void Cb3()
		{
			//Console.WriteLine( "Cb3" );
		}

		public static void UnitTest()
		{
			sUnitTest = new XStateMachineUnitTest();
			var sm = new XStateMachine< eTriggers >();

			//sm.Log( "empty" );

			txStateID s1 = sm.CreateState( null );
			sm.SetStartingState( s1 );
			sm.Update();

			sm.CreateTransition( s1, s1, eTriggers.Jump, sUnitTest.Cb1 );
			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Poke );
			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Poke );

			sm.CreateTransition( s1, s1, eTriggers.Poke, sUnitTest.Cb2 );
			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Poke );

			//Console.WriteLine( "test removal" );
			sm.RemoveTransition( s1, eTriggers.Jump );

			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Poke );

			// sm.RemoveTransition( s1, eTriggers.Jump );  // correctly asserts missing trigger
			sm.RemoveTransition( s1, eTriggers.Poke );

			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Poke );

			sm.RemoveState( s1, txStateID.kNone );

			sm.Update();
			sm.ProcessTrigger( eTriggers.Poke );

			// sm.SetStartingState( txStateID.kNone );  // correctly asserts illegal starting state

			// sm.CreateTransition( s1, txStateID.kNone, sUnitTest.Cb2, eTriggers.Jump );  // asserts correctly s1 not found
			// sm.CreateTransition( txStateID.kNone, txStateID.kNone, sUnitTest.Cb2, eTriggers.Jump );  // asserts correctly kNone not found

			//Console.WriteLine( "testing non trivial transitions and static callbacks" );

			txStateID s2 = sm.CreateState( Cb3 );
			txStateID s3 = sm.CreateState( sUnitTest.Cb2 );
			sm.SetStartingState( s2 );

			sm.CreateTransition( s2, s3, eTriggers.Jump, sUnitTest.Cb1 );
			sm.CreateTransition( s3, s2, eTriggers.Poke, sUnitTest.Cb1 );

			sm.Update();
			sm.ProcessTrigger( eTriggers.Jump );
			sm.Update();
			sm.ProcessTrigger( eTriggers.Poke );
			sm.Update();
			sm.ProcessTrigger( eTriggers.Jump );
			sm.ProcessTrigger( eTriggers.Poke );
			sm.Update();

			//sm.Log();

			//Console.WriteLine( "testing non trivial remove state" );

			// sm.RemoveState( s2, s2 );  // correctly asserts can't name same state
			sm.RemoveState( s2, s3 );

			//sm.Log();

			//Console.WriteLine( "testing non trivial remove state, not the current" );

			txStateID s4 = sm.CreateState( sUnitTest.Cb1 );
			sm.CreateTransition( s3, s4, eTriggers.Poke, sUnitTest.Cb1 );
			sm.CreateTransition( s4, s3, eTriggers.Poke, sUnitTest.Cb1 );

			//sm.Log();

			sm.RemoveState( s4, s3 );

			//sm.Log();
		}
	}
}