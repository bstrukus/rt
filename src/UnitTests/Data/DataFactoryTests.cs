/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Data
{
    using rt.Data;
    using rt.Utility;

    [TestClass]
    public class DataFactoryTests
    {
        [TestMethod]
        [DataRow("validSceneNoIssues.json")]
        public void LoadInValidSceneWithNoIssues(string sceneFile)
        {
            // Arrange
            var dataFactory = new DataFactory();
            string sceneFilePath = Dir.GetTestSceneFilePath(sceneFile);

            // Act
            bool result = dataFactory.Load(sceneFilePath);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("missingImage.json")]
        public void LoadInSceneWithMissingDataAndRegisterIt(string sceneFile)
        {
            // Arrange
            var dataFactory = new DataFactory();
            string sceneFilePath = Dir.GetTestSceneFilePath(sceneFile);

            // Act
            bool result = dataFactory.Load(sceneFilePath);

            // Assert
            Assert.IsFalse(result);
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