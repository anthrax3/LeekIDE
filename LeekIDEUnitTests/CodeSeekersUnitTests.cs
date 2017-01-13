using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LeekIDE.Autocompletion;
using LeekIDE.Autocompletion.Seekers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeekIDEUnitTests
{
    [TestClass]
    public class CodeSeekersUnitTests
    {
        [TestMethod]
        public void VariableIsDetected()
        {
            var results = new LeekIDE.Autocompletion.Seekers.VariableSeeker().GetResults("var foo = 5; var thing = 8;","f").ToList();
            if (results.Count == 1)
            {
                Assert.AreEqual(results.First().Text, "foo");
            }
            else
            {
                Assert.Fail($"Didn't found 1 result, but {results.Count}");
            }
        }
        [TestMethod]
        public void CodeSnippetIsDetected()
        {
            var seeker = new CodeSnippetSeeker();
            seeker.AddCodeSnippet(new CodeSnippet("foreach","yes"));
            var results = seeker.GetResults("fore").ToList();
            if (results.Any())
            {
                Assert.IsTrue(results.Count == 1 && results.First().Text == "foreach");
            }
            else
            {
                Assert.Fail("Well sadly there is no snippets :'( let's cry");
            }
        }

        [TestMethod]
        public void KeywordIsDetected()
        {
            XshdSyntaxDefinition syntax;
            // TODO: make this clearer
            using (Stream s = Assembly.GetAssembly(typeof(KeywordSeeker)).GetManifestResourceStream("LeekIDE.Syntax.LeekScript.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    syntax = HighlightingLoader.LoadXshd(reader);
                }
            }
            var results = new KeywordSeeker().GetResults("f",syntax.Elements.First(e => e is XshdRuleSet) as XshdRuleSet);
            Assert.IsTrue(results.All(c => c.Text == "for" || c.Text == "function" || c.Text == "false"));
        } 
    }
}
