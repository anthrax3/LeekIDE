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

namespace LeekIDE.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            boxie.Text = StartupText ?? "";
        }
        public static string StartupText { get; set; }
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
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            SettingsWindow.FontChanged += async (sender, i) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    boxie.FontSize = i;
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
            var dialog = new SaveFileDialog()
            {
                Filter = "LeekScript Files (*.leek)|*.leek|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() ?? false)
            {
                using (var w = new StreamWriter(dialog.OpenFile()))
                {
                    await w.WriteAsync(boxie.Text);
                }
            }
        }

        private async void CommandBinding_Executed_OpenAsync(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "LeekScript Files (*.leek)|*.leek|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() ?? false)
            {
                using (var r = new StreamReader(dialog.OpenFile()))
                {
                    boxie.Text = await r.ReadToEndAsync();
                }
            }
        }
    }
}
