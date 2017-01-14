using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LeekIDE
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Any() && (args.FirstOrDefault()?.EndsWith(".leek") ?? false))
            {
                using (var r = new StreamReader(args[0]))
                {
                    LeekIDE.Views.MainWindow.StartupText = r.ReadToEnd();
                }
            }
        }
    }
}
