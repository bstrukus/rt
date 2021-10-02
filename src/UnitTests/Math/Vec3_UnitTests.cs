// #copyright_placeholder

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Math
{
    [TestClass]
    public class Vec3_UnitTests
    {
        private const float Epsilon = 0.00001f;

        [TestMethod]
        public void AllZeroConstructorContainsZeroses()
        {
            // Arrange
            Vec3 allZero = Vec3.Zero();

            // Act
            bool result = allZero.x == 0.0f &&
                          allZero.y == 0.0f &&
                          allZero.z == 0.0f;

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AccessXValueAsFive()
        {
            Vec3 fiveX = new Vec3(5.0f, 0.0f, 0.0f);

            // Act

            // Assert
            Assert.AreEqual(5.0f, fiveX.x, Epsilon);
        }
    }
}