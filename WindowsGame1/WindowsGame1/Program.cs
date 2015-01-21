using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;


namespace WindowsGame1 {
#if WINDOWS || XBOX
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args) {
            if (args.Length > 0) {
                // Arguments description:
                // /S - Test in Explorer
                // /s - for start / Preview in Control Panel
                // none - for Configure in Explorer
                // /p <handle> for preview in Control Panel
                // /c:<handle> for Settings in Control Panel
                switch (args[0]) {
                    case "/s":
                        Start();
                        break;
                    case "/S":
                        Start();
                        break;
                }
            } else {
                DialogResult result = MessageBox.Show(
                    "itsfullostars -- Classic Windows 'Starfield' screensaver recreated.\n\nPress OK to run demonstration.",
                    "About",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information
                );
                if (result == DialogResult.OK) {
                    Start();
                } else {
                    return;
                }
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
