using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace TrajectoryVerisiIsleme.WinForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CultureInfo culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
