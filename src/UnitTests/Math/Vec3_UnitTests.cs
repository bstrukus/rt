/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt.Math;

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
            Vec3 allZero = Vec3.Zero;

            // Act
            bool result = allZero.X == 0.0f &&
                          allZero.Y == 0.0f &&
                          allZero.Z == 0.0f;

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AccessXValueAsFive()
        {
            // Arrange
            Vec3 fiveX = new Vec3(5.0f, 0.0f, 0.0f);

            // Act
            // no-op

            // Assert
            Assert.AreEqual(5.0f, fiveX.X, Epsilon);
        }

        #region Cross Product

        [TestMethod]
        public void CrossProductOfXAndYAxes()
        {
            // Arrange
            Vec3 xAxis = new Vec3(1.0f, 0.0f, 0.0f);
            Vec3 yAxis = new Vec3(0.0f, 1.0f, 0.0f);

            // Act
            Vec3 zAxis = Vec3.Cross(xAxis, yAxis);

            // Assert
            bool result = zAxis.X == 0.0f &&
                          zAxis.Y == 0.0f &&
                          zAxis.Z == 1.0f;
            Assert.AreEqual(Vec3.AxisZ, zAxis);
        }

        [TestMethod]
        public void CrossProductOfZAndXAxes()
        {
            // Arrange
            Vec3 xAxis = new Vec3(1.0f, 0.0f, 0.0f);
            Vec3 zAxis = new Vec3(0.0f, 0.0f, 1.0f);

            // Act
            Vec3 yAxis = Vec3.Cross(zAxis, xAxis);

            // Assert
            Assert.AreEqual(Vec3.AxisY, yAxis);
        }

        [TestMethod]
        public void CrossProductOfYAndZAxes()
        {
            // Arrange
            Vec3 yAxis = new Vec3(0.0f, 1.0f, 0.0f);
            Vec3 zAxis = new Vec3(0.0f, 0.0f, 1.0f);

            // Act
            Vec3 xAxis = Vec3.Cross(yAxis, zAxis);

            // Assert
            Assert.AreEqual(xAxis, Vec3.AxisX);
        }

        #endregion Cross Product

        #region Dot Product

        [TestMethod]
        public void DotProductOfTheSameVector()
        {
            // Arrange
            Vec3 xAxis = new Vec3(1.0f, 0.0f, 0.0f);

            // Act
            float result = Vec3.Dot(xAxis, xAxis);

            // Assert
            Assert.AreEqual(result, 1.0f);
        }

        [TestMethod]
        public void DotProductOfPerpendicularVectorsShouldBeZero()
        {
            // Arrange
            Vec3 xAxis = Vec3.AxisX;
            Vec3 yAxis = Vec3.AxisY;

            // Act
            float result = Vec3.Dot(xAxis, yAxis);

            // Assert
            Assert.AreEqual(result, 0.0f);
        }

        #endregion Dot Product
    }
}