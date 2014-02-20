using System;
using System.Windows.Forms;

namespace WindowsGame1 {
#if WINDOWS || XBOX
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args) {
            using (var game = new Game1()) {
                game.Run();
            }
        }
    }
#endif
}