using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeekIDEUnitTests
{
    [TestClass]
    public class EventsTestUnits
    {
        [TestMethod]
        public void IsParenthesisAdded()
        {
            var myEditor = new TextEditor();
            LeekIDE.Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(myEditor, '(');
            Assert.AreEqual(myEditor.Text,")");
        }
        [TestMethod]
        public void IsCurlyBracketAdded()
        {
            // this tests if the curly bracket is added :)
            var myEditor = new TextEditor();
            LeekIDE.Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(myEditor, '{');
            Assert.AreEqual(myEditor.Text, "}");
        }
        [TestMethod]
        public void IsQuoteAdded()
        {
            // this tests if quote is added WITHOUT looping
            var myEditor = new TextEditor();
            LeekIDE.Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(myEditor, '"');
            Task.Run(async () =>
            {
                await Task.Delay(1500);
                Assert.Fail("timeout :'(");
            });
            Assert.AreEqual(myEditor.Text, "\"");
        }
    }
}
