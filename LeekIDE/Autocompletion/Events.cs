using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LeekIDE.Autocompletion.Seekers;

namespace LeekIDE.Autocompletion
{
    internal static class Events
    {
        public static void WhenTextEntered(TextEditor edit, ref CompletionWindow complete, XshdSyntaxDefinition syntax, TextCompositionEventArgs e)
        {

            complete = new CompletionWindow(edit.TextArea);
            IList<ICompletionData> data = complete.CompletionList.CompletionData;
            List<char> wordsChar = new List<char>();
            var start = TextUtilities.GetNextCaretPosition(edit.Document, edit.CaretOffset,
                                                           LogicalDirection.Backward, CaretPositioningMode.WordStart);

            var end = 0;
            Debug.WriteLine($"ok so : the start is {start:x8} and da max value is {Int32.MaxValue:x8}");
            if (start != -1)
            {
                end = edit.CaretOffset;
                var current = start;
                while (current < end)
                {
                    if (current == Int32.MaxValue)
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
    }
}