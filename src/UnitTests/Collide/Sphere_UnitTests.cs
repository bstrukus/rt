/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collide
{
    using rt.Present;
    using rt.Collide;
    using rt.Math;

    [TestClass]
    public class Sphere_UnitTests
    {
        [TestMethod]
        [DataRow(0.0f, 0.0f, 1.0f)]
        [DataRow(0.0f, 0.0f, -1.0f)]
        [DataRow(0.0f, 1.0f, 0.0f)]
        [DataRow(0.0f, -1.0f, 0.0f)]
        [DataRow(1.0f, 0.0f, 0.0f)]
        [DataRow(-1.0f, 0.0f, 0.0f)]
        public void TestDirectRayHitInSphereCenterAlongMajorAxis(float x, float y, float z)
        {
            // Arrange
            var sphere = new Sphere(
                this.SimpleTransform(Vec3.Zero, 1.0f),
                this.SimpleMaterial(Vec3.AxisX),
                0.5f);
            Vec3 rayVal = new Vec3(x, y, z);
            var ray = new Ray(-rayVal, rayVal);

            // Act
            var hitInfo = sphere.TryIntersect(ray);
            // Since we're doing simple tests along major axes, the expected hit point result is easily calculated
            var pointOnSphere = -ray.Direction * sphere.Radius;
            var sphereNormal = -ray.Direction;

            // Assert
            Assert.IsTrue(hitInfo != null);
            Assert.AreEqual(pointOnSphere, hitInfo.Point);
            Assert.AreEqual(sphereNormal, hitInfo.Normal);
        }

        private Transform SimpleTransform(Vec3 pos, float scale)
        {
            return new Transform(pos, Quat.Identity, new Vec3(scale, scale, scale));
        }

        private Material SimpleMaterial(Vec3 color)
        {
            return new Material(color);
        }
    }
}