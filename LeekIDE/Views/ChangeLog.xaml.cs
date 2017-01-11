using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeekIDE.Views
{
    /// <summary>
    /// Logique d'interaction pour ChangeLog.xaml
    /// </summary>
    public partial class ChangeLog : Window
    {
        public ChangeLog(bool isNormal = false)
        {           
            InitializeComponent();
            if (!isNormal)
            {
                VerRun.Text = $"Welcome to the version {Assembly.GetEntryAssembly().GetName().Version}";
            }
            else
            {
                VerRun.Text = $"The current version is : {Assembly.GetEntryAssembly().GetName().Version}";
                TitleRun.Text = "Changelog";
            }
        }

        private void CloseThisPlease(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
