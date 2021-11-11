/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Data
{
    using rt.Data;

    [TestClass]
    public class DataFactory_UnitTests
    {
        private const string TestScenePath = "..\\..\\..\\..\\..\\scenes\\unit_tests\\";
        private const string SceneFile = "jsonTest.json";

        [TestMethod]
        public void LoadInValidSceneWithNoIssues()
        {
            // Arrange
            var dataFactory = new DataFactory();
            string sceneFilePath = TestScenePath + SceneFile;

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