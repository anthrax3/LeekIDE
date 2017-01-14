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
            //foreach (var item in Extracter.ExtractFromString(code,new[] { "for(var","for (var" },new[] { " in " }))
            //{
            //    if (item.StartsWith(word))
            //    {
            //        data.Add(new VariableCompletion(Extracter.ExtractFromString(code, "var", "in").FirstOrDefault().Trim()));
            //    }
            //}
            //foreach (var str in Extracter.ExtractFromString(code, "var", ";").Where(s => !Extracter.ExtractFromString(s," in ","var").Any()))
            //{
            //    var trueString = ""; 
            //    if (str.IndexOf("=", StringComparison.Ordinal) != -1)
            //    {
            //        trueString = str.Substring(0, str.IndexOf("=", StringComparison.Ordinal)).Trim();
            //    }
            //    else
            //    {
            //        trueString = str.Trim();
            //    }
            //    if (trueString?.StartsWith(word) ?? false)
            //        data.Add(new VariableCompletion(trueString));
            //}
            foreach (var item in Extracter.ExtractFromStringTuple(code, "var[\\s]", "\\s"))
            {
                var str = item.str.Trim();
                if (str.StartsWith(word))
                {
                    var foreachMargin = Extracter.GetPreviousWord(code, item.index, 2).Contains("for") ? 1 : 0;
                    if (Extracter.GetBracketLevel(code,item.index) + foreachMargin <= Extracter.GetBracketLevel(code,code.Length - 1))
                    data.Add(new VariableCompletion(str));
                }
            }
            return data;
        }
    }
}
