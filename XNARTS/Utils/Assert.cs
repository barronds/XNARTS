using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

using XNARTS.XNARTSMath;


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
			if( Math.Abs( test - target ) > tol )
			{
				Breakpoint( msg );
			}
		}

		public static void AssertVal( float test, float target, float tol, string msg = null )
		{
			AssertValTol( target - test, tol, msg );
		}

		public static void AssertVal( Vector2 test, Vector2 target, float tol, string msg = null )
		{
			AssertValTol( (target - test).Length(), tol, msg );
		}

		private static void AssertValTol( float val, float tol, string msg )
		{
			if( Helpers.Abs( val ) > tol )
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
