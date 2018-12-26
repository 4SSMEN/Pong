using System;
using System.Windows.Forms;

namespace Pong
{
#if WINDOWS || XBOX
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Settings settings = new Settings())
            {
                Application.Run(settings);
            }
        }
    }
#endif
}

