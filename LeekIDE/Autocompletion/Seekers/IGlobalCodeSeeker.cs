using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace LeekIDE.Autocompletion.Seekers
{
    public interface IGlobalCodeSeeker
    {
        IEnumerable<ICompletionData> GetResults(TextEditor editor,string word,XshdRuleSet ruleSet = null);
        IEnumerable<ICompletionData> GetResults(string code, string word, XshdRuleSet ruleSet = null);
    }
}