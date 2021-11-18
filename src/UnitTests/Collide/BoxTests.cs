/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collide
{
    using rt.Collide;
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
            Box box = this.OriginCube(1.0f);

            Vec3 rayVal = new Vec3(x, y, z);
            var ray = new Ray(-rayVal, rayVal);

            // Act
            var result = box.TryIntersect(ray);
            var pointOnBox = -ray.Direction * 0.5f;
            var boxNormal = -ray.Direction;

            // Assert
            Assert.IsTrue(result != null);
            Assert.AreEqual(pointOnBox, result.Point);
            Assert.AreEqual(boxNormal, result.Normal);
        }

        [TestMethod]
        public void TestRayPointingParallelOutsideFace()
        {
            // Arrange
            Box box = this.OriginCube(1.0f);
            var ray = new Ray(Vec3.AxisX, Vec3.AxisZ);

            // Act
            var result = box.TryIntersect(ray);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestRayPointingAwayOutsideBoxWithinAllFacesButOne()
        {
            // Arrange
            var box = OriginCube(1.0f);
            var ray = new Ray(Vec3.AxisY, Vec3.AxisY);

            // Act
            var result = box.TryIntersect(ray);

            // Assert
            Assert.AreEqual(null, result);
        }

        private Box OriginCube(float size)
        {
            float halfSize = size / 2.0f;
            return new Box(
                cornerPoint: new Vec3(-halfSize, -halfSize, halfSize),
                lengthVector: Vec3.AxisX * size,
                widthVector: -Vec3.AxisZ * size,
                heightVector: Vec3.AxisY * size,
                material: Helpers.SimpleMaterial(Vec3.One));
        }
    }
}