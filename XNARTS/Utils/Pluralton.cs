using System;
using System.Reflection;
using System.Collections.Generic;


namespace XNARTS
{
	// XPluralton< Key, T > usage:
	// inherit like this: public class Foo : XPluralton< Key, T >, where T is the class and Key is a suitable key, like an enum or string.
	// then make a public constructor in T, which must also have no other public constructors.
	// the constructor needs to take two params, the key and an int, which should be passed back through to pluralton.  
	// this is for debugging mostly, and an attempt at some pluralton enforcement (ie, don't construct an instance without pluralton).
	// eg:
	// class Foo : XPluralton< eFooType, Foo >
	// {
	//	   public Foo( eFooType pluralton_key, int pluralton_only_constructor ) : base( pluralton_key, pluralton_only_constructor ) {}
	// }
	//
	// Initialize() must be called exactly once before calls to CreateInstance() or Instance().
	// at some point, call CreateInstance() and anytime afterwards Instance().
	// T will probably need an Init() function to replace the work of a constructor, to be called shortly or right after CreateInstance()
	// the usage of sInitLock, the constructors check, and Activator came from the almighty internet. 

	public class XPluralton< Key, T > where T : class
	{
		private static SortedDictionary< Key, T >	sInstances;
		private static object						sInitLock = new object();
		private static int                          s_DO_NOT_USE_THIS_VALUE_TO_CONSTRUCT_AN_INSTANCE_OF_T_OUTSIDE_OF_PLURALTON = 0xB00B135;
		private Key                                 mPluraltonKey;

		public XPluralton( Key pluralton_key, int pluralton_only_constructor )
		{
			mPluraltonKey = pluralton_key;
			
			if( pluralton_only_constructor != s_DO_NOT_USE_THIS_VALUE_TO_CONSTRUCT_AN_INSTANCE_OF_T_OUTSIDE_OF_PLURALTON )
			{
				Type t = typeof( T );
				throw new InvalidOperationException( String.Format( "Use Pluralton::CreateInstance to make instances of {0}", t.Name ) );
			}
		}

		public Key GetPluraltonKey()
		{
			return mPluraltonKey;
		}

		public static void Initialize()
		{
			Type t = typeof( T );
			Type k = typeof( Key );

			// Ensure there are no public constructors...
			ConstructorInfo[] ctors = t.GetConstructors();

			if (	ctors.Length != 1 || 
					ctors[ 0 ].GetParameters().Length != 2 || 
					ctors[ 0 ].GetParameters()[ 0 ].ParameterType != k ||
					ctors[ 0 ].GetParameters()[ 1 ].ParameterType != typeof( int ) )
			{
				String msg = String.Format( "{0} must have only 1 public constructor, taking 2 parameters of type {1}, int", t.Name, k.Name );
				throw new InvalidOperationException( msg );
			}
			
			XUtils.Assert( sInstances == null );
			sInstances = new SortedDictionary<Key, T>();
		}

		public static T Instance( Key key )
		{
			// Technically, the sInstances object isn't thread safe, as you can be
			// adding an object to it in CreateInstance while reading from it
			// here.  To be truly safe, you should be locking the same object
			// (sInitLock), at which point it isn't really an initialization lock.
			// Alternately, if the create is very sporadic, the reads are fast, and
			// don't want to lock on read, you could lock up write, update a clone,
			// then copy the clone over to the Pluralton object.
			XUtils.Assert( sInstances != null, "must initialize XPluralton" );
			T value;
			XUtils.Assert( sInstances.TryGetValue( key, out value ), "instance doesn't exist" );
			return value;
		}

		public static T CreateInstance( Key key )
		{
			XUtils.Assert( sInstances != null, "must initialize XPluralton" );

			lock( sInitLock )
			{
				if( sInstances.ContainsKey( key ) )
				{
					XUtils.Assert( false, "instance already exists for this key" );
					return Instance( key );
				}
				
				// Create an instance via the private constructor
				Type t = typeof( T );
				Object[] args = { key, s_DO_NOT_USE_THIS_VALUE_TO_CONSTRUCT_AN_INSTANCE_OF_T_OUTSIDE_OF_PLURALTON };
				T instance = (T)Activator.CreateInstance( t, args );
				sInstances.Add( key, instance );
				return instance;
			}
		}

		public static T GetInstance( Key key )
		{
			lock ( sInitLock )
			{
				XUtils.Assert( sInstances != null, "must initialize XPluralton" );
				T value;
				if ( sInstances.TryGetValue( key, out value ) == false )
				{
					value = XPluralton<Key, T>.CreateInstanceNoLock( key );
				}
				return value;
			}
		}

		private static T CreateInstanceNoLock( Key key )
		{
			XUtils.Assert( sInstances != null, "must initialize XPluralton" );
			// Create an instance via the private constructor
			Type t = typeof( T );
			T instance = (T)Activator.CreateInstance( t, true );
			sInstances.Add( key, instance );
			return instance;
		}

	}
}
