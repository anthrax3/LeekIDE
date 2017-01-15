using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logique d'interaction pour InputPrompt.xaml
    /// </summary>
    public partial class InputPrompt : Window
    {
        public InputPrompt(string title)
        {
            DataContext = this;
            InitializeComponent();
            TitleLabel.Content = title;
        }
        public string Text { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public EventHandler Cancelled;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Cancelled?.Invoke(this, null);
            Close();
        }
    }
}
