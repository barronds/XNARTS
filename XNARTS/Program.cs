using System;

namespace XNARTS
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XUnitTests.RunUnitTests();

            using ( var game = new Game1())
                game.Run();
        }
    }
#endif
}
