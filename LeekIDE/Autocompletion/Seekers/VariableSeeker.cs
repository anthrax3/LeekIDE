using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LeekIDE.Autocompletion.Data;
using LeekIDE.Utilities;

namespace LeekIDE.Autocompletion.Seekers
{
    public class VariableSeeker : IGlobalCodeSeeker
    {
        public IEnumerable<ICompletionData> GetResults(TextEditor editor,string word,XshdRuleSet ruleSet = null)
        {
            return GetResults(editor.Text, word);
        }

        public IEnumerable<ICompletionData> GetResults(string code, string word, XshdRuleSet ruleSet = null)
        {

            var data = new List<ICompletionData>();
            foreach (var str in Extracter.ExtractFromString(code, "var", ";"))
            {
                var trueString = "";
                if (str.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    trueString = str.Substring(0, str.IndexOf("=", StringComparison.Ordinal)).Trim();
                }
                else
                {
                    trueString = str.Trim();
                }
                if (Extracter.ExtractFromString(trueString, "in", ")").Any())
                {
                    trueString = null;
                }
                if (trueString?.StartsWith(word) ?? false)
                    data.Add(new VariableCompletion(trueString));
            }
            return data;
        }
    }
}
