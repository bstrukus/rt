/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Present
{
    using rt.Collide;
    using rt.Math;
    using rt.Present;
    using System.Collections.Generic;

    [TestClass]
    public class SceneTests
    {
        [TestMethod]
        public void CanCorrectlyReturnCloserHitPointBetweenTwoConsecutiveSpheres()
        {
            // Arrange
            var closeSphere = new Sphere(
                Helpers.SimpleTransform(Vec3.AxisX, 1.0f),
                Helpers.SimpleMaterial(Vec3.AxisX),
                0.5f);
            var farSphere = new Sphere(
                Helpers.SimpleTransform(Vec3.AxisX * 2.0f, 1.0f),
                Helpers.SimpleMaterial(Vec3.AxisX),
                0.5f);
            var hittables = new List<IHittable> { closeSphere, farSphere };
            var scene = new Scene(hittables, null);
            var ray = new Ray(Vec3.Zero, Vec3.AxisX);

            // Act
            var hitInfo = scene.Project(ray);

            // Assert
            Assert.AreEqual(0.5f, hitInfo.Distance);
        }
    }
}