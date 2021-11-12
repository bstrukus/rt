/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Data
{
    using rt.Data;
    using rt.Utility;

    [TestClass]
    public class DataFactory_UnitTests
    {
        [TestMethod]
        public void LoadInValidSceneWithNoIssues()
        {
            // Arrange
            const string validSceneFile = "jsonTest.json";
            var dataFactory = new DataFactory();
            string sceneFilePath = Dir.GetTestSceneFilePath(validSceneFile);

            // Act
            bool result = dataFactory.Load(sceneFilePath);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LoadInSceneWithMissingDataAndRegisterIt()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void LoadInSpheresAndBoxes()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsTrue(false);
        }
    }
}