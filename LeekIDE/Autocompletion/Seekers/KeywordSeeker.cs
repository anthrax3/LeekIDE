using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using JetBrains.Annotations;
using LeekIDE.Autocompletion.Data;

namespace LeekIDE.Autocompletion.Seekers
{
    public class KeywordSeeker : ICodeSeeker
    {

        /// <summary>
        /// Gets the keyword corresponding.
        /// </summary>
        /// <param name="word">The word to check</param>
        /// <param name="ruleSet">The NECESSARY DAMMIT ruleset</param>
        /// <returns>An IEnumarable of completion data</returns>
        public IEnumerable<ICompletionData> GetResults(string word,[ItemNotNull] XshdRuleSet ruleSet = null)
        {
            if (ruleSet == null)
                throw new NullReferenceException(); // In case the syntax is null
            // Else just continue :)
            var keywords = ruleSet.Elements.FirstOrDefault(e => e is XshdKeywords) as XshdKeywords;
            return from keyword
                in keywords?.Words
                where keyword.StartsWith(word)
                select new KeywordCompletion(keyword);
        }
    }
}