using System;
using System.Reflection;


namespace XNARTS
{
	public class Singleton<T> where T : class
	{
		private static T		sInstance;
		private static object	sInitLock = new object();


		public static T Instance()
		{
			if( sInstance == null )
			{
				Utils.Assert( false, "instance doesn't exist" );
			}				

			return sInstance;
		}


		public static T InstantiateSingleton()
		{
			lock( sInitLock )
			{
				if( sInstance != null )
				{
					Utils.Assert( false, "instance already exists" );
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
