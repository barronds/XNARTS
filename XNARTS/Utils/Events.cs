﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// XListener, XBroadcaster pattern:
	// broadcaster class has one or more XBroadcaster<T> for each T.
	// listener class has one or more XListener<T> objects as members, like mail boxes.
	// listener objects subscribe and unsuscribe as needed using XBroadcaster<T>.
	// XBroadcaster<T> objects will have to be publicly available to XListener<T>.

	public enum eEventQueueFullBehaviour
	{
		Invalid = -1,
		Assert,
		Ignore,
		IgnoreOldest
	};


	public class XListener< EventData > where EventData : class
	{
		private Queue< EventData >			mEvents;
		private eEventQueueFullBehaviour	mFullBehaviour;
		private int                         mMaxCapacity;
		private String                      mDebugName;

		public XListener( int max_capacity, eEventQueueFullBehaviour full_behaviour, String debug_name )
		{
			// initial_capacity is just a hint as the queue can scale.
			// for most systems, one event will be broadcast for each consumption so 1 is often enough
			// for initial and max capacity.
			const int initial_capacity = 1;
			mFullBehaviour = full_behaviour;
			mMaxCapacity = max_capacity;
			mDebugName = debug_name;
			mEvents = new Queue<EventData>( initial_capacity );
		}

		// called by broadcaster
		public void Post( EventData e )
		{
			if( mEvents.Count == mMaxCapacity )
			{
				if( mFullBehaviour == eEventQueueFullBehaviour.Assert )
				{
					XUtils.Assert( false, "listener queue full" );
				}
				else if( mFullBehaviour == eEventQueueFullBehaviour.IgnoreOldest )
				{
					mEvents.Dequeue();
					mEvents.Enqueue( e );
				}

				// deliberate fall through for Ignore
			}
			else
			{
				mEvents.Enqueue( e );
			}
		}

		public class Enumerator
		{
			private Queue< EventData >	mEvents;
			private EventData			mCurrent;

			public Enumerator( Queue< EventData > events )
			{
				mEvents = events;
				mCurrent = null;
			}
			public bool MoveNext()
			{
				if( mEvents.Count > 0 )
				{
					mCurrent = mEvents.Dequeue();
					return true;
				}
				else
				{
					mCurrent = null;
					return false;
				}
			}
			public EventData GetCurrent()
			{
				return mCurrent;
			}
		}

		public Enumerator CreateEnumerator()
		{
			return new Enumerator( mEvents );
		}
		public EventData GetMaxOne()
		{
			if( mEvents.Count == 0 )
			{
				return null;
			}
			else if( mEvents.Count == 1 )
			{
				return mEvents.Dequeue();
			}
			else
			{
				// incorrect assumption about this listener
				XUtils.Assert( false );
				return null;
			}
		}

		// not of much use other than debugging or enforcing certain timing behaviour
		public int GetNumEvents()
		{
			return mEvents.Count;
		}
	}


	public class XBroadcaster< EventData > where EventData : class
	{
		private List< XListener< EventData > > mListeners;

		public XBroadcaster()
		{
			const int initial_capacity = 1;
			mListeners = new List< XListener< EventData >>( initial_capacity );
		}

		public void Subscribe( XListener< EventData > listener )
		{
			// TODO: maybe add returns number added, so avoid 'contains'
			XUtils.Assert( !mListeners.Contains( listener ), "this listener already subscribed" );
			mListeners.Add( listener );
		}
		public void Unsubscribe( XListener<EventData> listener )
		{
			// TODO: maybe remove returns number removed, so avoid 'contains'
			XUtils.Assert( mListeners.Contains( listener ), "this listener is not subscribed" );
			mListeners.Remove( listener );
		}

		// hopefully not many if any systems need to rely on this
		public bool IsSubscribed( XListener<EventData> listener )
		{
			return mListeners.Contains( listener );
		}
		public void Post( EventData e )
		{
			// send to each listener
			int num_listeners = mListeners.Count();

			for( int i = 0; i < num_listeners; ++i )
			{
				mListeners[ i ].Post( e );
			}
		}
	}


	public class XEventsUnitTests
	{
		public class tEvent1
		{
			public int a;
		}


		public class tEvent2
		{
			public float b;
		}


		public class tEvent3
		{
			public char c;
		}

		public class TestListener
		{
			public XListener< tEvent1 > mMailbox1;
			public XListener< tEvent2 > mMailbox2;
			public XListener< tEvent3 > mMailbox3;

			public TestListener()
			{
				mMailbox1 = new XListener<tEvent1>( 1, eEventQueueFullBehaviour.Assert, "testmailbox1" );
				mMailbox2 = new XListener<tEvent2>( 2, eEventQueueFullBehaviour.IgnoreOldest, "testmailbox2" );
				mMailbox3 = new XListener<tEvent3>( 1, eEventQueueFullBehaviour.Ignore, "testmailbox3" );
			}
		}


		public class TestBroadcaster
		{
			public XBroadcaster< tEvent1 > mEvent1;
			public XBroadcaster< tEvent2 > mEvent2;
			public XBroadcaster< tEvent3 > mEvent3;


			public TestBroadcaster()
			{
				mEvent1 = new XBroadcaster<tEvent1>();
				mEvent2 = new XBroadcaster<tEvent2>();
				mEvent3 = new XBroadcaster<tEvent3>();
			}
		}


		public static void UnitTest()
		{
			TestListener listener = new TestListener();
			TestBroadcaster broadcaster = new TestBroadcaster();

			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 0 );
			//listener_1.mMailbox1.ReadNext(); // correctly hit assert for reading no events

			var event1 = new tEvent1();
			event1.a = 2;

			XUtils.Assert( broadcaster.mEvent1.IsSubscribed( listener.mMailbox1 ) == false );
			broadcaster.mEvent1.Subscribe( listener.mMailbox1 );
			XUtils.Assert( broadcaster.mEvent1.IsSubscribed( listener.mMailbox1 ) == true );

			broadcaster.mEvent1.Post( event1 );
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 1 );
			var enumerator_1 = listener.mMailbox1.CreateEnumerator();
			enumerator_1.MoveNext();
			tEvent1 event_read = enumerator_1.GetCurrent();
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 0 );
			XUtils.Assert( event_read.a == 2 );

			// listener.mMailbox1.ReadNext(); // correctly asserts for reading no events after having one
			broadcaster.mEvent1.Post( event1 );
			// broadcaster.mEvent1.Post( event1 ); // correctly asserts for event queue full

			var event3 = new tEvent3();
			event3.c = 'a';

			broadcaster.mEvent3.Subscribe( listener.mMailbox3 );
			broadcaster.mEvent3.Post( event3 );
			var event4 = new tEvent3();
			event4.c = 'b';
			broadcaster.mEvent3.Post( event4 );
			broadcaster.mEvent3.Post( event4 );
			XUtils.Assert( listener.mMailbox3.GetNumEvents() == 1 );
			var enumerator_3 = listener.mMailbox3.CreateEnumerator();
			enumerator_3.MoveNext();
			XUtils.Assert( enumerator_3.GetCurrent().c == 'a' );

			var event2 = new tEvent2();
			var event6 = new tEvent2();
			var event7 = new tEvent2();
			event2.b = 1.0f;

			broadcaster.mEvent2.Subscribe( listener.mMailbox2 );
			broadcaster.mEvent2.Post( event2 );
			event6.b = 2.0f;
			broadcaster.mEvent2.Post( event6 );
			event7.b = 3.0f;
			broadcaster.mEvent2.Post( event7 );
			XUtils.Assert( listener.mMailbox2.GetNumEvents() == 2 );
			var enumerator2 = listener.mMailbox2.CreateEnumerator();
			enumerator2.MoveNext();
			XUtils.Assert( enumerator2.GetCurrent().b == 2.0f );
			enumerator2.MoveNext();
			XUtils.Assert( enumerator2.GetCurrent().b == 3.0f );

			TestListener listener2 = new TestListener();
			broadcaster.mEvent1.Subscribe( listener2.mMailbox1 );
			var e1 = listener.mMailbox1.CreateEnumerator();
			e1.MoveNext();
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 0 );
			XUtils.Assert( broadcaster.mEvent1.IsSubscribed( listener.mMailbox1 ) );
			broadcaster.mEvent1.Post( event1 );
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 1 );
			XUtils.Assert( listener2.mMailbox1.GetNumEvents() == 1 );

		}
	}
}
