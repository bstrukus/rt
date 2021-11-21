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
    public class EllipsoidTests
    {
        [TestMethod]
        [DataRow(0.0f, 0.0f, 1.0f)]
        [DataRow(0.0f, 0.0f, -1.0f)]
        [DataRow(0.0f, 1.0f, 0.0f)]
        [DataRow(0.0f, -1.0f, 0.0f)]
        [DataRow(1.0f, 0.0f, 0.0f)]
        [DataRow(-1.0f, 0.0f, 0.0f)]
        public void TestDirectRayHitInEllipsoidCenterAlongMajorAxis(float x, float y, float z)
        {
            // Arrange
            var ellipsoidScale = new Vec3(0.25f, 0.5f, 0.75f);
            var ellipsoid = this.CreateOriginAxisAlignedEllipsoid(ellipsoidScale);
            Vec3 rayVal = new Vec3(x, y, z);
            var ray = new Ray(-rayVal, rayVal);

            // Act
            var result = ellipsoid.TryIntersect(ray);
            // Since we're doing simple tests along major axes, the expected hit point result is easily calculated
            var expectedPoint = Vec3.Multiply(-ray.Direction, ellipsoidScale);
            var expectedNormal = -ray.Direction;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPoint, result.Point);
            Assert.AreEqual(expectedNormal, result.Normal);
        }

        private Ellipsoid CreateOriginAxisAlignedEllipsoid(Vec3 scale)
        {
            return new Ellipsoid(
                center: Vec3.Zero,
                uAxis: Vec3.AxisX * scale.X,
                vAxis: Vec3.AxisY * scale.Y,
                wAxis: Vec3.AxisZ * scale.Z,
                Helpers.DefaultMaterial);
        }
    }
}