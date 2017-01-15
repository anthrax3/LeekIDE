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
        public IEnumerable<ICompletionData> GetResults(TextEditor editor, string word, XshdRuleSet ruleSet = null, int offset = 0)
        {
            return GetResults(editor.Text.Substring(0, editor.CaretOffset), word,offset: offset);
        }

        public IEnumerable<ICompletionData> GetResults(string code, string word, XshdRuleSet ruleSet = null, int offset = 0)
        {
            var data = new List<ICompletionData>();
            word = word.ToLower();
            foreach (var item in TextUtils.ExtractFromStringTuple(code, "var[\\s]", "\\s|;"))
            {
                var str = item.str.Trim();
                if (str.StartsWith(word))
                {
                    var foreachMargin = TextUtils.GetPreviousWord(code, item.index, 2).Contains("for") ? 1 : 0;
                    if (TextUtils.GetBracketLevel(code,item.index) + foreachMargin <= TextUtils.GetBracketLevel(code,code.Length - 1))
                    data.Add(new VariableCompletion(str));
                }
            }
            foreach (var item in TextUtils.ExtractFromStringTuple(code,"global[\\s]","\\s|;"))
            {
                var str = item.str.Trim();
                if (str.StartsWith(word))
                {
                    if (TextUtils.GetBracketLevel(code, item.index) <= TextUtils.GetBracketLevel(code, code.Length - 1))
                        data.Add(new GlobalVariableCompletion(str));
                }
            }
            return data;
        }
    }
}
