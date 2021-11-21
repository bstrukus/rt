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
        [DataRow(0)]    // Normal == x-axis
        [DataRow(1)]    // Normal == y-axis
        [DataRow(2)]    // Normal == z-axis
        public void TestRayAgainstOriginAlignedQuad(int axis)
        {
            // Arrange
            Vec3 planeNormal = this.IntToAxis(axis);
            var axisAlignedQuad = this.CreateAxisAlignedQuad(planeNormal);
            var ray = new Ray(planeNormal, -planeNormal);

            // Act
            var result = axisAlignedQuad.TryIntersect(ray);

            // Assert
            Vec3 expectedPoint = Vec3.Zero;
            Vec3 expectedNormal = planeNormal;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPoint, result.Point);
            Assert.AreEqual(expectedNormal, result.Normal);
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

        private Polygon CreateAxisAlignedQuad(Vec3 axisNormal)
        {
            // #todo Handle inverted plane normal
            // Default value is the positive z-axis as the plane normal
            Vec3 uAxis = Vec3.AxisY;
            Vec3 vAxis = Vec3.AxisX;
            if (axisNormal == Vec3.AxisX)
            {
                uAxis = Vec3.AxisZ;
                vAxis = Vec3.AxisY;
            }
            else if (axisNormal == Vec3.AxisY)
            {
                uAxis = Vec3.AxisX;
                vAxis = Vec3.AxisZ;
            }

            const float Size = 0.5f;
            uAxis *= Size;
            vAxis *= Size;

            List<Vec3> vertices = new List<Vec3>
            {
                // ++ +- -- -+
                uAxis + vAxis,
                uAxis - vAxis,
                -uAxis - vAxis,
                -uAxis + vAxis
            };

            return new Polygon(vertices, Helpers.DefaultMaterial);
        }

        private Vec3 IntToAxis(int axis)
        {
            Assert.IsTrue(Numbers.InRange(axis, 0, 2));
            if (axis == 0)
            {
                return Vec3.AxisX;
            }
            else if (axis == 1)
            {
                return Vec3.AxisY;
            }
            else if (axis == 2)
            {
                return Vec3.AxisZ;
            }
            return Vec3.Zero;
        }
    }
}