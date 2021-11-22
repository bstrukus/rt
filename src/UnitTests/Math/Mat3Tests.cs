/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Math
{
    using rt.Math;

    [TestClass]
    public class Mat3Tests
    {
        [TestMethod]
        public void CreateIdentityEqualsIdentity()
        {
            // Arrange
            Mat3 result = new Mat3(1.0f, 0.0f, 0.0f,
                                   0.0f, 1.0f, 0.0f,
                                   0.0f, 0.0f, 1.0f);

            // Act
            // No-op

            //Assert
            Assert.AreEqual(Mat3.Identity, result);
        }

        [TestMethod]
        public void SequentialMatrixDeterminantZero()
        {
            // Arrange
            var matrix = new Mat3(1.0f, 2.0f, 3.0f,
                                  4.0f, 5.0f, 6.0f,
                                  7.0f, 8.0f, 9.0f);

            // Act
            var result = matrix.Determinant();

            // Assert
            Assert.AreEqual(0.0f, result);
        }

        [TestMethod]
        [DataRow(3.0f, -2.0f, 5.0f,
                 7.0f, 4.0f, -8.0f,
                 5.0f, -3.0f, -4.0f,
                 -301.0f)]
        [DataRow(9.0f, 3.0f, 5.0f,
                 -6.0f, -9.0f, 7.0f,
                 -1.0f, -8.0f, 1.0f,
                 615.0f)]
        public void ValidDeterminantCorrectlyCalculated(float m00, float m01, float m02,
                                                        float m10, float m11, float m12,
                                                        float m20, float m21, float m22,
                                                        float expectedResult)
        {
            // Arrange
            var matrix = new Mat3(m00, m01, m02, m10, m11, m12, m20, m21, m22);

            // Act
            var result = matrix.Determinant();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(9.0f, 3.0f, 5.0f,  // Input matrix
                 -6.0f, -9.0f, 7.0f,
                 -1.0f, -8.0f, 1.0f,
                 47.0f / 615.0f, -43.0f / 615.0f, 66.0f / 615.0f,  // Expected matrix
                 -1.0f / 615.0f, 14.0f / 615.0f, -93.0f / 615.0f,
                 39.0f / 615.0f, 69.0f / 615.0f, -63.0f / 615.0f)]
        [DataRow(3.0f, -2.0f, 5.0f,
                 7.0f, 4.0f, -8.0f,
                 5.0f, -3.0f, -4.0f,
                 40.0f / 301.0f, 23.0f / 301.0f, 4.0f / 301.0f,
                 12.0f / 301.0f, 37.0f / 301.0f, -59.0f / 301.0f,
                 41.0f / 301.0f, 1.0f / 301.0f, -26.0f / 301.0f)]
        public void InvertibleMatrixCorrectlyComputeInverse(float m00, float m01, float m02,
                                                            float m10, float m11, float m12,
                                                            float m20, float m21, float m22,
                                                            float e00, float e01, float e02,
                                                            float e10, float e11, float e12,
                                                            float e20, float e21, float e22)
        {
            // Arrange
            var matrix = new Mat3(m00, m01, m02, m10, m11, m12, m20, m21, m22);

            // Act
            var result = matrix.Inverted();
            var expected = new Mat3(e00, e01, e02, e10, e11, e12, e20, e21, e22);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(9.0f, 3.0f, 5.0f,  // Input matrix
                 -6.0f, -9.0f, 7.0f,
                 -1.0f, -8.0f, 1.0f)]
        public void DoubleIndexAccessorEqualsSingleIndexAccessor(float m00, float m01, float m02,
                                                                 float m10, float m11, float m12,
                                                                 float m20, float m21, float m22)
        {
            // Arrange
            var result = new Mat3(m00, m01, m02, m10, m11, m12, m20, m21, m22);

            // Act
            // No-op

            // Assert
            Assert.AreEqual(result[0], result[0, 0]);
            Assert.AreEqual(result[1], result[0, 1]);
            Assert.AreEqual(result[2], result[0, 2]);

            Assert.AreEqual(result[3], result[1, 0]);
            Assert.AreEqual(result[4], result[1, 1]);
            Assert.AreEqual(result[5], result[1, 2]);

            Assert.AreEqual(result[6], result[2, 0]);
            Assert.AreEqual(result[7], result[2, 1]);
            Assert.AreEqual(result[8], result[2, 2]);
        }
    }
}