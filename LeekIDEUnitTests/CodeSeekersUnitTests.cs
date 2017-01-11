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
    }
}
