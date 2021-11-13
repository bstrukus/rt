/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using System;

    /// <summary>
    /// Helper class to handle directory resolving logic.
    /// </summary>
    internal static class Dir
    {
        // App dir: "rt\OUTPUT\netcoreapp3.0\"
        // Need to go up 2 levels to get to rt\
        static private string SceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\scenes\\";

        // Test dir: "rt\src\UnitTests\bin\Debug\netcoreapp3.1\"
        // Need to go up 5 levels to get to rt\
        static private string TestSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\..\\scenes\\unit_tests\\";

        static public string GetSceneFilePath(string sceneFile)
        {
            return $"{SceneDirectory}{sceneFile}";
        }

        static public string GetTestSceneFilePath(string sceneFile)
        {
            return $"{TestSceneDirectory}{sceneFile}";
        }
    }
}