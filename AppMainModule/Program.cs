using System;
using System.Threading;
using System.Windows.Forms;

namespace AppMainModule
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        /// Заборона подвійного запуску програми
        private static Mutex dInstance;
        private static readonly string dAppName = "Assessment_Polianski";//Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        [STAThread]
        static void Main()
        {
            dInstance = new Mutex(true, dAppName, out bool vCreateNewApp);


            if (vCreateNewApp)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //-----------------------------------------------------------
                _ = new ApplicationController();
                //ApplicationController vApplicationController = new ApplicationController();
                // Application.Run(new Form1()); // Переносим в AppMainModule-> Views -> FormMain -> Run();
                //-----------------------------------------------------------
            }
            else
                MessageBox.Show("Програма вже запущена!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
