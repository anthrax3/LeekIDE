using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace LeekIDE.Autocompletion.Seekers
{
    public interface ICodeSeeker
    {
        IEnumerable<ICompletionData> GetResults(string word,XshdRuleSet ruleSet = null);        
    }
}