using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using XNARTS;


namespace XNARTS
{
    public class XUnitTests
    {
        // toggle this to avoid unit tests
        private static bool sPerformUnitTests = true;


        private static void ExecuteTests()
        {
            xCoord.unitTest();
			xAABB2.UnitTest();
			XEventsUnitTests.UnitTest();
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
