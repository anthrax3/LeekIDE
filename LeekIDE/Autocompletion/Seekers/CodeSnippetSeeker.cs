using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Snippets;
using LeekIDE.Autocompletion.Data;
using LeekIDE.Views;

namespace LeekIDE.Autocompletion.Seekers
{
    public class CodeSnippetSeeker : ICodeSeeker
    {
        public IEnumerable<ICompletionData> GetResults(string word, XshdRuleSet ruleSet = null)
        {
            return from codeSnippet 
                   in SnippetEditor.CodeSnippets
                   where codeSnippet.ShortenedCalling.StartsWith(word)
                   select new CodeSnippetCompletion(codeSnippet);
        }
        // Test thingies
        public void AddCodeSnippet(CodeSnippet snippet)
        {
            SnippetEditor.CodeSnippets.Add(snippet);
        }
    }
}