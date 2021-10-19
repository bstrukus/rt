/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Math
{
    using rt.Math;
    using rt.Collide;

    [TestClass]
    public class Ray_UnitTests
    {
        [TestMethod]
        public void VerifyNonNormalizedConstructorNormalizes()
        {
            // Arrange
            var origin = Vec3.Zero;
            var direction = Vec3.AxisX * 2.0f;

            // Act
            var ray = new Ray(origin, direction);

            // Assert
            Assert.IsTrue(ray.Direction.Length() == 1.0f);
        }

        [TestMethod]
        public void VerifyNormalizedConstructorNormalized()
        {
            // Arrange
            var origin = Vec3.Zero;
            var direction = Vec3.AxisX;

            // Act
            var ray = new Ray(origin, direction, true);

            // Assert
            Assert.IsTrue(ray.Direction.Length() == 1.0f);
        }
    }
}