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
using LeekIDE.Utilities;
using Microsoft.Win32;

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
        public string FilePath { get; set; }
        public async static Task<LeekBox> CreateBoxFromFileAsync(string path)
        {
            var box = new LeekBox();
            try
            {
                using (var r = new StreamReader(path))
                {
                    box.Text = await r.ReadToEndAsync();
                    box.FilePath = path;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Couldn't open file.");
            }
            return box;
        }
        /// <summary>
        /// Saaves the current content to the a file.
        /// </summary>
        /// <param name="saveAs"></param>
        /// <returns>A task that you don't care</returns>
        public async Task<string> SaveToFileAsync(bool saveAs = false)
        {
            if (FilePath == null || saveAs)
            {
                var dialog = new SaveFileDialog()
                {
                    Filter = "LeekScript Files (*.leek)|*.leek|All files (*.*)|*.*"                            
                };
                if (dialog.ShowDialog() ?? false)
                {
                    FilePath = dialog.FileName;
                    using (var w = new StreamWriter(dialog.OpenFile()))
                    {
                        await w.WriteAsync(Text);
                    }
                    return dialog.SafeFileName;
                }
            }
            else
            {
                try
                {
                    using (var w = new StreamWriter(FilePath))
                    {
                        await w.WriteAsync(Text);
                    }
                    return FilePath.Remove(0, FilePath.LastIndexOfAny(new[] { '\\', '/' }) + 1);
                } catch
                {
                    FilePath = null;
                    return await SaveToFileAsync(true);
                }
            }
            return null;
        }
        private CompletionWindow complete;
        private bool InsertString { get; set; } = true;
        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            complete = new CompletionWindow(TextArea);
            IList<ICompletionData> data = complete.CompletionList.CompletionData;
            if (InsertString || e.Text != "\"")
            {
                InsertString = false;
                Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(this, e.Text.First());
            }
            else
            {
                InsertString = true;
            }
            //foreach (var x1 in edit.Document.GetLineByOffset(edit.CaretOffset))
            //{

            //}
            //for (int i = 0; i < Extracter.GetBracketLevel(edit.Text,edit.CaretOffset); i++)
            //{

            //}
            string s = TextUtils.GetPreviousWord(Text, CaretOffset);
            var syntaxic = this.Xshd.Elements.Where(ele => ele is XshdRuleSet).Cast<XshdRuleSet>().FirstOrDefault();
            if (!TextUtils.ShouldIBreak(s) || s == "")
            {
                foreach (var completionData in new VariableSeeker().GetResults(this, s, offset: CaretOffset))
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
            }
            data = data.OrderByDescending(c => c.Priority).ToList();
            if (!data.Any()) return;
            var off = s.Length;
            if (off < 0)
            {
                off = 0;
            }
            try
            {
                complete.Show();
                complete.MinWidth = 130;
                complete.Width = complete.CompletionList.CompletionData.Max(cd => cd.Text.Length * complete.FontSize);
            }
            catch (InvalidOperationException)
            {
                // The code is ran from a test unit.
            }
            complete.StartOffset -= off;
            complete.CompletionList.SelectedItem =
                complete.CompletionList.CompletionData.FirstOrDefault();

        }

        private void GetPreviousWord(List<char> wordsChar, out int start, out int end)
        {
            start = TextUtilities.GetNextCaretPosition(Document, CaretOffset,
                                                                       LogicalDirection.Backward, CaretPositioningMode.WordStart);
            end = 0;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
