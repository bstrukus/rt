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
            Box box = new Box(
                cornerPoint: new Vec3(-0.5f, -0.5f, 0.5f),
                lengthVector: Vec3.AxisX,
                widthVector: -Vec3.AxisZ,
                heightVector: Vec3.AxisY,
                material: Helpers.SimpleMaterial(Vec3.One));

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
    }
}