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

            // Act
            bool result = dataFactory.LoadScene(sceneFile);

            // Assert
            Assert.IsTrue(result);
        }
    }
}