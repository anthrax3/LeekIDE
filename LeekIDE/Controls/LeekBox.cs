using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using JetBrains.Annotations;
using LeekIDE.Autocompletion.Seekers;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace LeekIDE.Controls
{
    public class LeekBox : TextEditor, INotifyPropertyChanged
    {
        public LeekBox()
        {
            using (Stream s = Assembly.GetAssembly(typeof(LeekBox)).GetManifestResourceStream("LeekIDE.Syntax.LeekScript.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    this.Xshd = HighlightingLoader.LoadXshd(reader);
                    this.SyntaxHighlighting = HighlightingLoader.Load(Xshd, HighlightingManager.Instance);
                }
            }
            this.TextArea.TextEntered += TextArea_TextEntered;
            TextArea.TextEntering += TextArea_TextEntering;
            this.ShowLineNumbers = true;
            FontFamily = new FontFamily("Consolas");
            FontSize = GlobalFontsize;
        }

        public static int GlobalFontsize { get; set; } = 15;
        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // todo: migrate things
        }

        private XshdSyntaxDefinition Xshd { get; set; }

        private CompletionWindow complete;
        private bool InsertString { get; set; } = true;
        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            complete = new CompletionWindow(TextArea);
            IList<ICompletionData> data = complete.CompletionList.CompletionData;
            List<char> wordsChar = new List<char>();
            if (InsertString || e.Text != "\"")
            {
                InsertString = false;
                Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(this, e.Text.First());
            }
            else
            {
                InsertString = true;
            }
            var start = TextUtilities.GetNextCaretPosition(Document, CaretOffset,
                                                           LogicalDirection.Backward, CaretPositioningMode.WordStart);

            var end = 0;
            if (start != -1)
            {
                end = CaretOffset;
                var current = start;
                while (current < end)
                {
                    if (current == -1)
                        break;
                    try
                    {
                        wordsChar.Add(Document.GetCharAt(current));
                    }
                    catch
                    {
                        break;
                    }
                    current++;
                }
            }

            //foreach (var x1 in edit.Document.GetLineByOffset(edit.CaretOffset))
            //{

            //}
            //for (int i = 0; i < Extracter.GetBracketLevel(edit.Text,edit.CaretOffset); i++)
            //{

            //}
            string s = new string(wordsChar.ToArray());
            var syntaxic = this.Xshd.Elements.Where(ele => ele is XshdRuleSet).Cast<XshdRuleSet>().FirstOrDefault();
            foreach (var completionData in new VariableSeeker().GetResults(this, s))
            {
                data.Add(completionData);
            }
            foreach (var completionData in new KeywordSeeker().GetResults(s, syntaxic))
            {
                data.Add(completionData);
            }
            foreach (var completionData in new CodeSnippetSeeker().GetResults(s))
            {
                data.Add(completionData);
            }
            foreach (var completionData in new FunctionCodeSeeker().GetResults(this, s))
            {
                data.Add(completionData);
            }
            data = data.OrderByDescending(c => c.Priority).ToList();
            if (!data.Any()) return;
            var off = end - start;
            if (off < 0)
            {
                off = 0;
            }
            try
            {
                complete.Show();
            }
            catch (InvalidOperationException)
            {
                // The code is ran from a test unit.
            }
            complete.StartOffset -= off;
            complete.CompletionList.SelectedItem =
                complete.CompletionList.CompletionData.FirstOrDefault();
        
    }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
