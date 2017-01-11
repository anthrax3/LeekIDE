using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeekIDEUnitTests
{
    [TestClass]
    public class CodeSeekersUnitTests
    {
        [TestMethod]
        public void VariableIsDetected()
        {
            var results = new LeekIDE.Autocompletion.Seekers.VariableSeeker().GetResults("var foo = 5; var thing = 8;","f");
            if (results.Count() == 1)
            {
                Assert.AreEqual(results.First().Text, "foo");
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
