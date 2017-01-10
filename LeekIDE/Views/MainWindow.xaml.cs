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
            WhenTextEntered(textEditor, ref _completionWindow, Xshd, e);
        }

        public static void WhenTextEntered(TextEditor edit, ref CompletionWindow complete, XshdSyntaxDefinition syntax, TextCompositionEventArgs e)
        {

            complete = new CompletionWindow(edit.TextArea);
            IList<ICompletionData> data = complete.CompletionList.CompletionData;
            List<char> wordsChar = new List<char>();
            var start = TextUtilities.GetNextCaretPosition(edit.Document, edit.CaretOffset,
                                                           LogicalDirection.Backward, CaretPositioningMode.WordStart);

            var end = 0;
            Debug.WriteLine($"ok so : the start is {start:x8} and da max value is {int.MaxValue:x8}");
            if (start != -1)
            {
                end = edit.CaretOffset;
                var current = start;
                while (current < end)
                {
                    if (current == int.MaxValue)
                        break;
                    try
                    {
                        wordsChar.Add(edit.Document.GetCharAt(current));
                    }
                    catch
                    {
                        break;
                    }
                    current++;
                }
            }
            switch (e.Text)
            {
                case "{":
                    edit.TextArea.PerformTextInput("}");
                    edit.CaretOffset--;
                    break;
                case "(":
                    edit.TextArea.PerformTextInput(")");
                    edit.CaretOffset--;
                    break;
                case "\"":
                    edit.TextArea.PerformTextInput("\"");
                    edit.CaretOffset--;
                    break;
                default:
                    break;
            }
            string s = new string(wordsChar.ToArray());
            var syntaxic = syntax.Elements.Where(ele => ele is XshdRuleSet).Cast<XshdRuleSet>().FirstOrDefault();
            foreach (var completionData in new VariableSeeker().GetResults(edit,s))
            {
                data.Add(completionData);
            }
            foreach (var completionData in new KeywordSeeker().GetResults(s,syntaxic))
            {
                data.Add(completionData);
            }
            foreach (var completionData in new CodeSnippetSeeker().GetResults(s))
            {
                data.Add(completionData);
            }

            if (!data.Any()) return;
            var off = end - start;
            if (off < 0)
            {
                off = 0;
            }
            complete.Show();
            complete.StartOffset -= off;
            complete.CompletionList.SelectedItem =
                complete.CompletionList.CompletionData.FirstOrDefault();
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
