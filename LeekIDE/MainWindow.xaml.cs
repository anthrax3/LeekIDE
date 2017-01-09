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
using System.Windows.Media.Imaging;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;

namespace LeekIDE
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Properties.Settings.Default.Upgrade();
            SnippetEditor.CodeSnippets =
                JsonSerializer.CreateDefault()
                    .Deserialize<ObservableCollection<CodeSnippet>>(
                        new JsonTextReader(new StringReader(Properties.Settings.Default.json)));
            InitializeComponent();
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("LeekIDE.LeekScript.xshd"))
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
        private static List<string> ExtractFromString(
    string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    try
                    {
                        matched.Add(text.Substring(indexStart + startString.Length,
                                                   indexEnd - indexStart - startString.Length));
                        text = text.Substring(indexEnd + endString.Length);
                    }
                    catch
                    {
                        break;
                    }
                }
                else
                    exit = true;
            }
            return matched;
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
            else
            {
                Debug.WriteLine("ono");
                wordsChar.Add('²');
                wordsChar.Add('é');
                start = 0;
            }
            Debug.WriteLine("gat past");
            switch (e.Text)
            {
                case "{":
                    edit.Text += "}";
                    edit.CaretOffset++;
                    break;
                case "(":
                    edit.Text += ")";
                    edit.CaretOffset++;
                    break;
                case "\"":
                    edit.Text += "\"";
                    edit.CaretOffset++;
                    break;
                default:
                    break;
            }
            foreach (var word in (syntax
                .Elements
                .First(el => el is XshdRuleSet) as XshdRuleSet)
                .Elements
                .Where(ele => ele is XshdKeywords)
                .Cast<XshdKeywords>()
                .First()
                .Words)
            {
                if (word.StartsWith(new string(wordsChar.ToArray())))
                {
                    data.Add(new KeywordCompletion(word));
                }
            }
            foreach (var codeSnippet in SnippetEditor.CodeSnippets)
            {
                if (codeSnippet.ShortenedCalling.StartsWith(new string(wordsChar.ToArray())))
                {
                    data.Add(new CodeSnippetCompletion(codeSnippet));
                }
            }
            foreach (var str in ExtractFromString(edit.Text,"var",";"))
            {
                var trueString = "";
                if (str.IndexOf("=") != -1)
                {
                    trueString = str.Substring(0, str.IndexOf("=")).Trim();
                }
                else
                {
                    trueString = str.Trim();
                }
                if (trueString.StartsWith(new string(wordsChar.ToArray())))
                data.Add(new VariableCompletion(trueString));
                else
                    Debug.WriteLine($"In fact no because the TRU string is {trueString} and da words are... {new string(wordsChar.ToArray())}");
            }
            if (data.Any())
            {
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
        }
        private XshdSyntaxDefinition Xshd { get; set; }
        public class KeywordCompletion : ICompletionData
        {
            public KeywordCompletion(string text)
            {
                this.Text = text;
                Image = CurrentBitmap;
            }
            private BitmapImage CurrentBitmap { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Resources/keyword.png"));
            public System.Windows.Media.ImageSource Image { get; }
            public string Text { get; private set; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content
            {
                get { return this.Text; }
            }

            public object Description => $"{Text} keyword";

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }

            public double Priority { get; } = 2;
        }
        public class CodeSnippetCompletion : ICompletionData
        {
            public CodeSnippetCompletion(string shortened, string code)
            {
                this.Text = shortened;
                EntireCode = code;
                Image = CurrentBitmap;
            }

            public CodeSnippetCompletion(CodeSnippet snippet)
            {
                Text = snippet.ShortenedCalling;
                Image = CurrentBitmap;
                EntireCode = snippet.Code;
            }
            private BitmapImage CurrentBitmap { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Resources/snippet.png"));
            public System.Windows.Media.ImageSource Image { get; }
            public string Text { get; private set; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content
            {
                get { return this.Text; }
            }
            public string EntireCode { get; set; }
            public object Description => $"A code snippet for {Text}";

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, EntireCode);
            }

            public double Priority { get; } = 1.5;
        }
        public class VariableCompletion : ICompletionData
        {
            public VariableCompletion(string text)
            {
                this.Text = text;
            }
            private BitmapImage CurrentBitmap { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Resources/snippet.png"));
            public System.Windows.Media.ImageSource Image { get; }
            public string Text { get; private set; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content
            {
                get { return this.Text; }
            }
            
            public object Description => $"{Text} variable";

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, Text);
            }

            public double Priority { get; } = 3;
        }
        private void Code_OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new SnippetEditor();
            window.ShowDialog();
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class CodeSnippet
    {
        [JsonConstructor]
        public CodeSnippet(string shortenedcalling, string code)
        {
            ShortenedCalling = shortenedcalling;
            Code = code;
        }
        public string ShortenedCalling { get; set; }
        public string Code { get; set; }
    }
}
