using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LeekIDE
{
    /// <summary>
    /// Logique d'interaction pour SnippetEditor.xaml
    /// </summary>
    public partial class SnippetEditor : Window
    {
        public SnippetEditor()
        {
            InitializeComponent();
            this.Closed += SnippetEditor_Closed;
            DataContext = this;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("LeekIDE.LeekScript.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    Xshd = HighlightingLoader.LoadXshd(reader);
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(Xshd, HighlightingManager.Instance);
                }
            }
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextChanged += TextEditor_TextChanged;
        }

        private void SnippetEditor_Closed(object sender, EventArgs e)
        {
            using (var memory = new MemoryStream())
            {
                var result = JsonConvert.SerializeObject(CodeSnippets);
                Properties.Settings.Default.json = result;
                Properties.Settings.Default.Save();
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            if (Current != null)
            {
                Current.Code = textEditor.Text;
            }
        }

        public static ObservableCollection<CodeSnippet> CodeSnippets { get; set; } = new ObservableCollection<CodeSnippet>();
        private CompletionWindow comp;
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            MainWindow.WhenTextEntered(textEditor, ref comp, Xshd,e);
        }

        public XshdSyntaxDefinition Xshd { get; set; }
        private CodeSnippet Current { get; set; } = null;
        private void Selector_OnSelected(object sender, RoutedEventArgs e)
        {
            DeleteButton.IsEnabled = true;
            var item = View.SelectedItem as CodeSnippet;
            Current = item;
            textEditor.Text = item.Code;
            textEditor.IsEnabled = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CodeSnippets.Add(new CodeSnippet("newSnippet", ""));
        }

        private void WhenDeleting(object sender, RoutedEventArgs e)
        {
            View.SelectionChanged -= Selector_OnSelected;
            var bye = View.SelectedItem as CodeSnippet;
            var resoult =
                MessageBox.Show($"Are you SURE you want to delete this POOR snippet ({bye.ShortenedCalling}) ?",
                                "Deletion confirmation for noobs", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resoult == MessageBoxResult.Yes)
            CodeSnippets.Remove(bye);
            View.SelectionChanged += Selector_OnSelected;
        }
    }
}
