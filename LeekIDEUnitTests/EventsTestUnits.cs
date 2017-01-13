using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using LeekIDE.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeekIDEUnitTests
{
    [TestClass]
    public class EventsTestUnits
    {
        [TestMethod]
        public void IsParenthesisAdded()
        {
            var myEditor = new LeekBox();
            LeekIDE.Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(myEditor, '(');
            Assert.AreEqual(myEditor.Text,")");
        }
        [TestMethod]
        public void IsCurlyBracketAdded()
        {
            // this tests if the curly bracket is added :)
            var myEditor = new LeekBox();
            LeekIDE.Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(myEditor, '{');
            Assert.IsTrue(myEditor.Text.EndsWith("}"));
        }
        [TestMethod]
        public void IsQuoteAdded()
        {
            // this tests if quote is added WITHOUT looping
            var myEditor = new LeekBox();
            // LeekIDE.Autocompletion.Syntaxic.Symbols.AutoCompleteBrackets(myEditor, '"');
            myEditor.TextArea.PerformTextInput("\"");
            Task.Run(async () =>
            {
                await Task.Delay(1500);
                Assert.Fail("timeout :'(");
            });
            Assert.AreEqual(myEditor.Text, "\"\"");
        }
    }
}
