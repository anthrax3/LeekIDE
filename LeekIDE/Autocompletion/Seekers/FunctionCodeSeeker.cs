using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LeekIDE.Autocompletion.Data;

namespace LeekIDE.Autocompletion.Seekers
{
    public class FunctionCodeSeeker : IGlobalCodeSeeker
    {
        public IEnumerable<ICompletionData> GetResults(TextEditor editor, string word, XshdRuleSet ruleSet = null,int offset = 0)
        {
            return GetResults(editor.Text.Substring(0,editor.CaretOffset), word);
        }

        public IEnumerable<ICompletionData> GetResults(string code, string word, XshdRuleSet ruleSet = null,int offset = 0)
        {
            var funcs = Utilities.Extracter.ExtractFromString(code, "function", "{")
                .Select(s => new FunctionCompletion(s.Trim()));
            foreach (var functionCompletion in funcs)
            {
                if (functionCompletion.Text.StartsWith(word))
                    yield return functionCompletion;                
            }
        }
    }
}