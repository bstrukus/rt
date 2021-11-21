/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests.Collide
{
    using rt.Math;
    using rt.Collide.Shapes;
    using rt.Collide;

    [TestClass]
    public class PolygonTests
    {
        [TestMethod]
        [DataRow(0.0f, 0.0f, 1.0f)]
        [DataRow(0.0f, 0.0f, -1.0f)]
        [DataRow(0.0f, 1.0f, 0.0f)]
        [DataRow(0.0f, -1.0f, 0.0f)]
        [DataRow(1.0f, 0.0f, 0.0f)]
        [DataRow(-1.0f, 0.0f, 0.0f)]
        public void TestAxisAlignedRayDirectHitPolygon(float x, float y, float z)
        {
            // Arrange
            var triangle = this.CreateOriginTriangle(1.0f);

            Vec3 rayVal = new Vec3(x, y, z);
            var ray = new Ray(-rayVal, rayVal);

            // Act
            var result = triangle.TryIntersect(ray);

            // Assert
            Vec3 expectedPoint = Vec3.Zero;
            Vec3 expectedNormal = Vec3.One.Normalized();

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPoint, result.Point);
            Assert.AreEqual(expectedNormal, result.Normal);
        }

        [TestMethod]
        [DataRow(1.0f)]
        [DataRow(-1.0f)]
        public void TestRayWithinTriangleBoundsButPointingAway(float sign)
        {
            // Arrange
            var triangle = this.CreateOriginTriangle(1.0f);

            Vec3 rayVal = Vec3.One * sign;
            var ray = new Ray(rayVal, rayVal);

            // Act
            var result = triangle.TryIntersect(ray);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestRayAgainstOriginAlignedQuad(int axis)
        {
            // Arrange

            // Act

            // Assert
        }

        private Polygon CreateOriginTriangle(float size)
        {
            // Create triangle
            List<Vec3> vertices = new List<Vec3>()
            {
                Vec3.AxisX * size,
                Vec3.AxisY * size,
                Vec3.AxisZ * size,
            };

            // Obtain triangle center
            Vec3 triangleCenter = (vertices[0] + vertices[1] + vertices[2]) / 3.0f;

            // Shift triangle so center is at origin
            vertices[0] -= triangleCenter;
            vertices[1] -= triangleCenter;
            vertices[2] -= triangleCenter;

            return new Polygon(vertices, Helpers.SimpleMaterial(Vec3.One));
        }
    }
}