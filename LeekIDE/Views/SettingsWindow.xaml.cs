using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using JetBrains.Annotations;
using LeekIDE.Properties;

namespace LeekIDE.Views
{
    /// <summary>
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Context = new SettingsContext
                      {
                          FontSize = Settings.Default.FontSize
                      };
            DataContext = Context;
        }
        private SettingsContext Context { get; set; }

        public static EventHandler<int> FontChanged;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.FontSize != Context.FontSize)
            {
                FontChanged.Invoke(this,Context.FontSize);
                Settings.Default.FontSize = Context.FontSize;
            }
            Settings.Default.Save();
            Close();
        }
    }
    internal sealed class SettingsContext : INotifyPropertyChanged
    {
        public int FontSize { get; set; } = 15;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
