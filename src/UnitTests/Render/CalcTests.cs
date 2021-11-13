/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Render
{
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
    }
}
