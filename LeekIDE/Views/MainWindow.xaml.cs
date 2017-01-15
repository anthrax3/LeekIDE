using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LeekIDE.Autocompletion;
using LeekIDE.Autocompletion.Data;
using LeekIDE.Autocompletion.Seekers;
using LeekIDE.Properties;
using Newtonsoft.Json;
using Microsoft.Win32;
using LeekIDE.Controls;

namespace LeekIDE.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LeekBox GetCurrent()
        {
            return ((Tab.SelectedItem ?? Tab.Items[0]) as TabItem).Content as LeekBox;
        }
        private TabItem GetCurrentTab()
        {
            return (Tab.SelectedItem ?? Tab.Items[0]) as TabItem;
        }
        public MainWindow()
        {
            Closed += MainWindow_Closed;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; // Un
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            SnippetEditor.CodeSnippets =
                JsonSerializer.CreateDefault()
                    .Deserialize<ObservableCollection<CodeSnippet>>(
                        new JsonTextReader(new StringReader(Properties.Settings.Default.json)));

            InitializeComponent();
            GetCurrent().Text = StartupText ?? "";
            GetCurrentTab().Header = StartupFileName ?? "Unnamed.leek";
            GetCurrent().FilePath = StartupFilePath;
        }
        private bool CanClose => Tab.Items.Count > 1;
        public static string StartupText { get; set; }
        public static string StartupFilePath { get; set; }
        public static string StartupFileName { get; set; }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }
        public static RoutedCommand SaveCommand = new RoutedCommand
        {
            InputGestures = { new KeyGesture(Key.S, ModifierKeys.Control) }
        };
        public static RoutedCommand OpenCommand = new RoutedCommand
        {
            InputGestures = { new KeyGesture(Key.O, ModifierKeys.Control) }
        };
        public static RoutedCommand SaveAsCommand = new RoutedCommand
        {
            InputGestures = { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Alt) }
        };
        public static RoutedCommand CloseTabCommand = new RoutedCommand
        {
            InputGestures = { new KeyGesture(Key.W, ModifierKeys.Control) }
        };
        public static RoutedCommand OpenTabCommand = new RoutedCommand
        {
            InputGestures = { new KeyGesture(Key.N, ModifierKeys.Control) }
        };
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            SettingsWindow.FontChanged += async (sender, i) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var item in Tab.Items.Cast<TabItem>())
                    {
                        ((LeekBox)item.Content).FontSize = i;
                    }
                });
            };
            if (!Settings.Default.UpgradeNeeded) return;
            Settings.Default.Upgrade();
            Settings.Default.UpgradeNeeded = false;
            Settings.Default.Save();
            new ChangeLog().ShowDialog();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($@"You broke me :'(
Message : {(e.Exception).Message}
Stack Trace:
{(e.Exception).StackTrace}", "Nooooo", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var r = MessageBox.Show($@"You broke me :'(
Message : {((Exception)e.ExceptionObject).Message}
Stack Trace:
{((Exception)e.ExceptionObject).StackTrace}", "OH NO ;(", MessageBoxButton.OK, MessageBoxImage.Error);
        }



        private XshdSyntaxDefinition Xshd { get; set; }


        private void SnippetTriggered(object sender, RoutedEventArgs e)
        {
            new SnippetEditor().ShowDialog();
        }

        private void SettingsTriggered(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void ChangeLogTriggered(object sender, RoutedEventArgs e)
        {
            new ChangeLog(true).ShowDialog();
        }

        private async void CommandBinding_ExecutedAsync(object sender, ExecutedRoutedEventArgs e)
        {
            GetCurrentTab().Header = await GetCurrent().SaveToFileAsync(false);
        }

        private async void CommandBinding_Executed_OpenAsync(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "LeekScript Files (*.leek)|*.leek|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() ?? false)
            {
                var item = await LeekBox.CreateBoxFromFileAsync(dialog.FileName);
                var index = Tab.Items.Add(new TabItem
                {
                    Header = dialog.SafeFileName,
                    Content = item,
                    Style = Resources["tab"] as Style
                });
            }
        }

        private async void CommandBinding_SaveAsAsync(object sender, ExecutedRoutedEventArgs e)
        {
            GetCurrentTab().Header = await GetCurrent().SaveToFileAsync(true);
        }

        private void CommandBinding_ExecutedClose(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanClose)
            {
                Tab.Items.RemoveAt(Tab.SelectedIndex);
            }
        }

        private void CommandBinding_ExecutedNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            Tab.Items.Add(new TabItem
            {
                Content = new LeekBox(),
                Header = "Unnamed" + Tab.Items.Count + ".leek",
                Style = Resources["tab"] as Style
            });
        }

        private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tabItem = sender as TabItem;
            var box = tabItem.Content as LeekBox;
            var dialog = new InputPrompt("Please insert the new name for this tab (and only the tab) :")
            {
                Text = (sender as TabItem).Header as string
            };
            dialog.ShowDialog();
            tabItem.Header = dialog.Text + ".leek";
        }
    }
}
