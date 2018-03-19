using System;
using System.Reflection;


namespace XNARTS
{
	// XSingleton<T> usage:
	// inherit like this: public class Foo : XSingleton< T >
	// then make a private empty constructor in T.  T may not have any public constructors.
	// at some point, call CreateInstance() and anytime afterwards Instance()
	// T will probably need an Init() function to replace the work of a constructor, to be called shortly or right after CreateInstance()
	// the usage of sInitLock, the constructors check, and Activator came from the almighty internet. 

	public class XSingleton<T> where T : class
	{
		private static T		sInstance;
		private static object	sInitLock = new object();


		public static T Instance()
		{
			if( sInstance == null )
			{
				XUtils.Assert( false, "instance doesn't exist" );
			}				

			return sInstance;
		}


		public static T CreateInstance()
		{
			lock( sInitLock )
			{
				if( sInstance != null )
				{
					XUtils.Assert( false, "instance already exists" );
					return sInstance;
				}

				Type t = typeof( T );

				// Ensure there are no public constructors...
				ConstructorInfo[] ctors = t.GetConstructors();
				if( ctors.Length > 0 )
				{
					throw new InvalidOperationException( String.Format( "{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", t.Name));
				}

				// Create an instance via the private constructor
				sInstance = (T)Activator.CreateInstance( t, true );
				return sInstance;
			}
		}
	}
}
