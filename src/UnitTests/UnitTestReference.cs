/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTestReference
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true, "Should never be false");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void TestMethod2(int value)
        {
            var result = value == -1 ||
                         value == 0 ||
                         value == 1;

            Assert.IsTrue(result, $"{value} is not expected");
        }
    }
}