using System;
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


	public class XListener< EventData >
	{
		private Queue< EventData >			mEvents;
		private eEventQueueFullBehaviour	mFullBehaviour;
		private int                         mMaxCapacity;

		public XListener( int max_capacity = 1, eEventQueueFullBehaviour full_behaviour = eEventQueueFullBehaviour.Assert )
		{
			// initial_capacity is just a hint as the queue can scale.
			// for most systems, one event will be broadcast for each consumption so 1 is often enough
			// for initial and max capacity.
			const int initial_capacity = 1;
			mFullBehaviour = full_behaviour;
			mMaxCapacity = max_capacity;
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

		// owner of this mailbox calls this then iterates through using ReadNext().
		// TODO: change this to an iterator?
		public int GetNumEvents()
		{
			return mEvents.Count;
		}
		public EventData ReadNext()
		{
			XUtils.Assert( GetNumEvents() > 0, "trying to read no events" );
			return mEvents.Dequeue();
		}
	}


	public interface XIBroadcaster< EventData >
	{
		XBroadcaster<EventData> GetBroadcaster();
	}


	public class XBroadcaster< EventData >
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
		public struct tEvent1
		{
			public int a;
		}


		public struct tEvent2
		{
			public float b;
		}


		public struct tEvent3
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
				mMailbox1 = new XListener<tEvent1>();
				mMailbox2 = new XListener<tEvent2>( 2, eEventQueueFullBehaviour.IgnoreOldest );
				mMailbox3 = new XListener<tEvent3>( 1, eEventQueueFullBehaviour.Ignore );
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
			tEvent1 event_read = listener.mMailbox1.ReadNext();
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 0 );
			XUtils.Assert( event_read.a == 2 );

			// listener.mMailbox1.ReadNext(); // correctly asserts for reading no events after having one
			broadcaster.mEvent1.Post( event1 );
			// broadcaster.mEvent1.Post( event1 ); // correctly asserts for event queue full

			var event3 = new tEvent3();
			event3.c = 'a';

			broadcaster.mEvent3.Subscribe( listener.mMailbox3 );
			broadcaster.mEvent3.Post( event3 );
			event3.c = 'b';
			broadcaster.mEvent3.Post( event3 );
			broadcaster.mEvent3.Post( event3 );
			XUtils.Assert( listener.mMailbox3.GetNumEvents() == 1 );
			XUtils.Assert( listener.mMailbox3.ReadNext().c == 'a' );

			var event2 = new tEvent2();
			event2.b = 1.0f;

			broadcaster.mEvent2.Subscribe( listener.mMailbox2 );
			broadcaster.mEvent2.Post( event2 );
			event2.b = 2.0f;
			broadcaster.mEvent2.Post( event2 );
			event2.b = 3.0f;
			broadcaster.mEvent2.Post( event2 );
			XUtils.Assert( listener.mMailbox2.GetNumEvents() == 2 );
			XUtils.Assert( listener.mMailbox2.ReadNext().b == 2.0f );
			XUtils.Assert( listener.mMailbox2.ReadNext().b == 3.0f );

			TestListener listener2 = new TestListener();
			broadcaster.mEvent1.Subscribe( listener2.mMailbox1 );
			listener.mMailbox1.ReadNext();
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 0 );
			XUtils.Assert( broadcaster.mEvent1.IsSubscribed( listener.mMailbox1 ) );
			broadcaster.mEvent1.Post( event1 );
			XUtils.Assert( listener.mMailbox1.GetNumEvents() == 1 );
			XUtils.Assert( listener2.mMailbox1.GetNumEvents() == 1 );

		}
	}
}
