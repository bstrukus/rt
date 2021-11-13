/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Render
{
    using rt.Math;
    using rt.Render;

    [TestClass]
    public class ProjectionPlaneTests
    {
        [TestMethod]
        public void GetCenterOfProjectionPlane()
        {
            // Arrange
            var projectionPlane = new ProjectionPlane(Vec3.Zero, Vec3.AxisX, Vec3.AxisY);

            // Act
            var planeCenter = projectionPlane.GetPointOnPlane(0.0f, 0.0f);

            // Assert
            Assert.AreEqual(Vec3.Zero, planeCenter);
        }

        [TestMethod]
        [DataRow(-1.0f, 1.0f)]  // Top left
        [DataRow(-1.0f, -1.0f)] // Bottom left
        [DataRow(1.0f, 1.0f)]   // Top right
        [DataRow(1.0f, -1.0f)]  // Bottom right
        public void GetCornersOfProjectionPlane(float hVal, float vVal)
        {
            // Arrange
            float hScale = 3.0f;    // Testing non-uniform projection plane
            float vScale = 2.0f;    // Testing non-uniform projection plane
            var projectionPlane = new ProjectionPlane(Vec3.AxisZ, hScale * Vec3.AxisX, vScale * Vec3.AxisY);

            // Act
            var planePoint = projectionPlane.GetPointOnPlane(hVal, vVal);

            // Assert
            float xTestVal = hScale * hVal;
            float yTestVal = vScale * vVal;
            Vec3 expected = new Vec3(xTestVal, yTestVal, 0.0f);

            Assert.AreEqual(expected, planePoint);
        }
    }
}