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
            var args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments?.ActivationData?.Any() ?? false ? 
                       AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0] 
                       : null;
            try
            {
                if (args != null)
                    if (args.EndsWith(".leek"))
                    {
                        Uri uri = new Uri(args);
                        args = uri.LocalPath;
                        
                        using (var r = new StreamReader(args))
                        {
                            LeekIDE.Views.MainWindow.StartupText = r.ReadToEnd();
                            Views.MainWindow.StartupFilePath = uri.LocalPath;
                            Views.MainWindow.StartupFileName = uri.LocalPath.Split('\\').Last();
                        }
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured : {ex.Message}");
            }
        }
        
    }
}
