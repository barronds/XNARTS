using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public struct txStateID
	{
		private ulong mID;
		private static ulong sNext = 1;


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


		public override String ToString()
		{
			return "ID = " + mID.ToString();
		}
	}


	public struct txTrigger
	{
		private int mTrigger;


		public txTrigger( int trigger )
		{
			mTrigger = trigger;
		}


		public bool Equals( txTrigger rhs )
		{
			return mTrigger == rhs.mTrigger;
		}


		public override String ToString()
		{
			return "Trigger = " + mTrigger.ToString();
		}
	}


	public class XStateMachine
	{
		// holds the state for one instance of a state machine.
		// holds the context object associated with the callbacks.
		private XState mCurrentState;
		private SortedDictionary< txStateID, XState > mStates;

		public delegate void Callback();


		public XStateMachine()
		{
			mCurrentState = null;
		}


		public void SetStartingState( txStateID starting_state )
		{
			XUtils.Assert( mCurrentState == null );
			mCurrentState = GetState( starting_state );
		}


		public class XState
		{
			public txStateID mStateID;
			private Callback mUpdateCallback;
			private SortedDictionary< txTrigger, XTransition > mTransitions;


			public XState( Callback callback )
			{
				mStateID.Init();
				mUpdateCallback = callback;
				mTransitions = new SortedDictionary<txTrigger, XTransition>();
			}


			public void Add( txTrigger trigger, XState to_state, Callback callback )
			{
				XUtils.Assert( !mTransitions.ContainsKey( trigger ) );
				XTransition transition = new XTransition( to_state, callback );
				mTransitions.Add( trigger, transition );
			}


			public void Remove( txTrigger trigger )
			{
				XUtils.Assert( mTransitions.ContainsKey( trigger ) );
				mTransitions.Remove( trigger );
			}


			public void RemoveTransitionsTo( txStateID state_id )
			{
				List< txTrigger > removals = new List< txTrigger >();

				for( int i = 0; i < mTransitions.Count; ++i )
				{
					var transition_i = mTransitions.ElementAt( i );

					if( transition_i.Value.GetToStateID().Equals( state_id ) )
					{
						removals.Add( transition_i.Key );
					}
				}

				foreach( txTrigger removal in removals )
				{
					mTransitions.Remove( removal );
				}
			}


			public void Update()
			{
				mUpdateCallback();
			}


			public bool ProcessTrigger( txTrigger trigger, out XState to_state )
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


			public override String ToString()
			{
				String result = "State: " + mStateID.ToString() + ", Callback: " + mUpdateCallback.ToString() + "\n";

				foreach(  var trigger_transition_pair in mTransitions )
				{
					result += trigger_transition_pair.Key.ToString() + " -> " + trigger_transition_pair.Value.ToString() + "\n";
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
				mTransitionCallback();
				return mToState;
			}


			public override String ToString()
			{
				return mToState.ToString() + ", Callback: " + mTransitionCallback.ToString();
			}
		}


		public txStateID CreateState( Callback callback )
		{
			XState state = new XState( callback );
			return state.mStateID;
		}


		public void RemoveState( txStateID state_id, txStateID new_state_if_current )
		{
			// if that was the current state, prepare to switch states as indicated.
			// remove all transitions to that state from other states.
			// remove state.
			XState next_state = null;

			if( mCurrentState.mStateID.Equals( state_id ) )
			{
				XUtils.Assert( !new_state_if_current.Equals( state_id ) );
				bool found_next = mStates.TryGetValue( new_state_if_current, out next_state );
				XUtils.Assert( found_next );
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
		public void CreateTransition( txStateID from, txStateID to, Callback callback, txTrigger trigger )
		{
			// make sure 'from' already exists
			// make sure 'from' state doesn't already have that trigger (Add() will do that)
			XState from_state, to_state;
			bool found_from = mStates.TryGetValue( from, out from_state );
			bool found_to = mStates.TryGetValue( to, out to_state );
			XUtils.Assert( found_from && found_to );
			from_state.Add( trigger, to_state, callback );
		}


		public void RemoveTransition( txStateID from, txTrigger trigger )
		{
			XState from_state;
			bool found_from = mStates.TryGetValue( from, out from_state );
			XUtils.Assert( found_from );
			from_state.Remove( trigger );
		}


		public void Update()
		{
			mCurrentState.Update();
		}


		public void ProcessTrigger( txTrigger trigger )
		{
			XState change;
			bool transition = mCurrentState.ProcessTrigger( trigger, out change );

			if( transition )
			{
				mCurrentState = change;
			}
		}


		private XState GetState( txStateID state_id )
		{
			XState state;
			bool found = mStates.TryGetValue( state_id, out state );
			XUtils.Assert( found );
			return state;
		}


		public override String ToString()
		{
			String result = "State Machine States:\n";

			foreach( var stateID_state_pair in mStates )
			{
				result += stateID_state_pair.Key.ToString() + ": " + stateID_state_pair.Value.ToString() + "\n";
			}

			return result;
		}
	}


}