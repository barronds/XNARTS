using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using XNARTS.RTSMath;

namespace XNARTS.UnitTests
{
    public class Framework
    {
        // toggle this to avoid unit tests
        private static bool sPerformUnitTests = true;

        public static void Assert( bool b, string msg = null )
        {
            if ( !b )
            {
                Breakpoint( msg );
            }
        }

        public static void AssertVal( double test, double target, double tol, string msg = null )
        {
            if ( Math.Abs( test - target ) > tol )
            {
                Breakpoint( msg );
            }
        }

        private static void Breakpoint( string msg )
        {
            // breakpoint here
            Debug.Assert( false, msg );
        }

        private static void ExecuteTests()
        {
            tCoord.unitTest();
        }

        public static void RunUnitTests()
        {
            if ( !sPerformUnitTests )
            {
                return;
            }

            Stopwatch stopWatch = new Stopwatch();

            Console.WriteLine( "Running RTS Unit Tests..." );
            stopWatch.Start();
            ExecuteTests();
            stopWatch.Stop();
            Console.Write( "RTS Unit Tests Complete: " );

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine( elapsedTime );
        }
    }
}
