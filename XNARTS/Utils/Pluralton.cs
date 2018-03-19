using System;
using System.Reflection;
using System.Collections.Generic;


namespace XNARTS
{
	// XPluralton<T> usage:
	// inherit like this: public class Foo : XSingleton< T >
	// then make a private empty constructor in Foo.
	// at some point, call InstantiateSingleton() and anytime afterwards Instance()
	// Foo will probably need an Init() function to replace the work of a constructor, to be called shortly or right after InstantiateSingleton()
	// the usage of sInitLock, the constructors check, and Activator came from the almighty internet. 

	public class XPluralton< Key, T > where T : class
	{
		private static SortedDictionary< Key, T >	sInstances;
		private static object						sInitLock = new object();


		public static void Initialize()
		{
			XUtils.Assert( sInstances == null );
			sInstances = new SortedDictionary< Key, T >();
		}


		public static T Instance( Key key )
		{
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
				
				Type t = typeof( T );

				// Ensure there are no public constructors...
				ConstructorInfo[] ctors = t.GetConstructors();
				if( ctors.Length > 0 )
				{
					throw new InvalidOperationException( String.Format( "{0} has at least one accesible ctor making it impossible to enforce pluralton behaviour", t.Name));
				}

				// Create an instance via the private constructor
				T instance = (T)Activator.CreateInstance( t, true );
				sInstances.Add( key, instance );
				return instance;
			}
		}
	}
}
