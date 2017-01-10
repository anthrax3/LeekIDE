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
using Newtonsoft.Json;

namespace LeekIDE.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; // Un
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Properties.Settings.Default.Upgrade();
            SnippetEditor.CodeSnippets =
                JsonSerializer.CreateDefault()
                    .Deserialize<ObservableCollection<CodeSnippet>>(
                        new JsonTextReader(new StringReader(Properties.Settings.Default.json)));
            InitializeComponent();
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("LeekIDE.Syntax.LeekScript.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    Xshd = HighlightingLoader.LoadXshd(reader);
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(Xshd, HighlightingManager.Instance);
                }
            }
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.ContextMenu = new ContextMenu
                                              {
                                                  Items =
                                                  {
                                                      Resources["undoItem"],
                                                      Resources["redoItem"]
                                                  }
                                              };
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($@"You broke me :'(
Message : {(e.Exception).Message}
Stack Trace:
{(e.Exception).StackTrace}","Nooooo",MessageBoxButton.OK,MessageBoxImage.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var r = MessageBox.Show($@"You broke me :'(
Message : {((Exception) e.ExceptionObject).Message}
Stack Trace:
{((Exception) e.ExceptionObject).StackTrace}","OH NO ;(",MessageBoxButton.OK,MessageBoxImage.Error);
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {

            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]) && _completionWindow.CompletionList.CompletionData.Any())
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    if (!(_completionWindow.CompletionList.SelectedItem is CodeSnippetCompletion))
                        _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }
        
        CompletionWindow _completionWindow;
        public void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            Events.WhenTextEntered(textEditor, ref _completionWindow, Xshd, e);
        }

        private XshdSyntaxDefinition Xshd { get; set; }
        
        private void Code_OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new SnippetEditor();
            window.ShowDialog();
        }
    }
}
