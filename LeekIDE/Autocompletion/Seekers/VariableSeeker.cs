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
    class VariableSeeker : IGlobalCodeSeeker
    {
        public IEnumerable<ICompletionData> GetResults(TextEditor editor,string word,XshdRuleSet ruleSet = null)
        {
            var data = new List<ICompletionData>();
            foreach (var str in Extracter.ExtractFromString(editor.Text, "var", ";"))
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

                if (trueString.StartsWith(word))
                    data.Add(new VariableCompletion(trueString));
            }
            return data;
        }
    }
}
