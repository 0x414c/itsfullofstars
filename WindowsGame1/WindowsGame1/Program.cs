using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsGame1 {
#if WINDOWS || XBOX
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args) {
            if (args.Length > 0) {
                switch (args[0]) {
                    case "/s":
                        Start();
                        break;
                }
            }
            else {
                Start();
            }
        }

        static void Start() {
            using (var game = new Game1()) {
                game.Run();
            }
        }
    }
#endif
}
