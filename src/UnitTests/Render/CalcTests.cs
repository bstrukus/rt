/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Render
{
    using rt.Collide;
    using rt.Math;
    using rt.Render;

    [TestClass]
    public class CalcTests
    {
        [TestMethod]
        public void AreAcuteVectorsProducingPositiveDiffuseResults()
        {
            // Arrange
            Vec3 normal = Vec3.AxisY;
            Vec3 lightVector = Vec3.AxisY;

            // Act
            float result = Calc.DiffuseCoefficient(normal, lightVector);

            // Assert
            Assert.IsTrue(result > 0.0f);
        }

        [TestMethod]
        public void AreObtuseVectorsProducingZeroForDiffuse()
        {
            // Arrange
            Vec3 normal = -Vec3.AxisY;
            Vec3 lightVector = Vec3.AxisY;

            // Act
            float result = Calc.DiffuseCoefficient(normal, lightVector);

            // Assert
            Assert.IsTrue(result == 0.0f);
        }

        [TestMethod]
        public void PerpendicularIncidentVectorProducesSimilarRefractedVecttor()
        {
            // Arrange
            Vec3 incidentVector = Vec3.AxisY;
            Vec3 normal = Vec3.AxisY;
            float currRefractIndex = 1.0f;  // air
            float nextRefractIndex = 1.0f;  // made-up value

            // Act
            Vec3 result = Calc.Refract(incidentVector, normal, currRefractIndex, nextRefractIndex);
            Vec3 expected = -Vec3.AxisY;

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DiagonalIncidentVectorProducesMirroredRefractedVector()
        {
            // Arrange
            Vec3 incidentVector = Vec3.One.Normalized();    // Diagonal for all 3 axes
            Vec3 normal = Vec3.AxisY;
            float currRefractIndex = 1.0f;  // Simple values for easy verification
            float nextRefractIndex = 1.0f;
            float relativeRefractionIndex = currRefractIndex / nextRefractIndex;

            // Act
            Vec3 result = Calc.Refract(incidentVector, normal, currRefractIndex, nextRefractIndex);
            Vec3 expected = -Vec3.One.Normalized();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void VerifyRefractedRayStartsInsideBox()
        {
            // Arrange
            var box = Helpers.OriginCube(1.0f);
            var ray = new Ray(Vec3.AxisX, -Vec3.AxisX);
            var hitInfo = box.TryIntersect(ray);

            // Act
            var refractedRay = Calc.RefractedRay(ray, hitInfo, 1.0f, 1.0f);
            var result = Numbers.InRange(refractedRay.Origin.X, 0.4f, 0.5f);

            // Assert
            Assert.IsTrue(result);
        }
    }
}