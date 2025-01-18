using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace DummyClientCore
{
    internal static class Program
    {
        const int ThreadCount = 4;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm form = new MainForm();
            DummyManager manager = new DummyManager(8);
            MainController controller = new MainController(form, manager);

            Application.Run(form);
        }

    }
}
