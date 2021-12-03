/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collide
{
    using rt.Collide;
    using rt.Collide.Shapes;
    using rt.Math;

    [TestClass]
    public class BoxTests
    {
        [TestMethod]
        [DataRow(0.0f, 0.0f, 1.0f)]
        [DataRow(0.0f, 0.0f, -1.0f)]
        [DataRow(0.0f, 1.0f, 0.0f)]
        [DataRow(0.0f, -1.0f, 0.0f)]
        [DataRow(1.0f, 0.0f, 0.0f)]
        [DataRow(-1.0f, 0.0f, 0.0f)]
        public void TestDirectRayHitInBoxCenterAlongMajorAxis(float x, float y, float z)
        {
            // Arrange
            Box box = Helpers.OriginCube(1.0f);

            Vec3 rayVal = new Vec3(x, y, z);
            var ray = new Ray(-rayVal, rayVal);

            // Act
            var result = box.TryIntersect(ray);
            var pointOnBox = -ray.Direction * 0.5f;
            var boxNormal = -ray.Direction;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pointOnBox, result.Point);
            Assert.AreEqual(boxNormal, result.Normal);
        }

        [TestMethod]
        public void TestRayPointingParallelOutsideFace()
        {
            // Arrange
            Box box = Helpers.OriginCube(1.0f);
            var ray = new Ray(Vec3.AxisX, Vec3.AxisZ);

            // Act
            var result = box.TryIntersect(ray);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestRayPointingAwayOutsideBoxWithinAllFacesButOne()
        {
            // Arrange
            var box = Helpers.OriginCube(1.0f);
            var ray = new Ray(Vec3.AxisY, Vec3.AxisY);

            // Act
            var result = box.TryIntersect(ray);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RayInsideBoxIntersectsOppositeSide()
        {
            // Arrange
            var box = Helpers.OriginCube(1.0f);
            var ray = new Ray(Vec3.AxisX * 0.25f, -Vec3.AxisX);

            // Act
            var result = box.TryIntersect(ray);
            var expected = new Vec3[]
            {
                -Vec3.AxisX * 0.5f, // Opposite side of the box
                -Vec3.AxisX         // Box face normal
            };

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected[0], result.Point);
            Assert.AreEqual(expected[1], result.Normal);
        }
    }
}