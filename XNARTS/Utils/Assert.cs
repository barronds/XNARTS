using System;
using System.Diagnostics;


namespace XNARTS
{
	public partial class Utils
	{
		public static void Assert( bool b, string msg = null )
		{
			if( !b )
			{
				Breakpoint( msg );
			}
		}

		public static void AssertVal( double test, double target, double tol, string msg = null )
		{
			if( System.Math.Abs( test - target ) > tol )
			{
				Breakpoint( msg );
			}
		}

		private static void Breakpoint( string msg )
		{
			// breakpoint here
			Debug.Assert( false, msg );
		}
	}
}
