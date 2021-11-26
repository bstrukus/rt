/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using System;

    /// <summary>
    /// Helper class to handle directory resolving logic.
    /// </summary>
    public static class Dir
    {
        private static string OutputDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        private static string ConfigDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\";

        // App dir: "rt\OUTPUT\rt\<target_config>\<netcoreapp_ver>\"
        // Need to go up 4 levels to get to rt\
        private static string SceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\";

        // Test dir: "rt\OUTPUT\unit_tests\<target_config>\<netcoreapp_ver>\"
        // Need to go up 4 levels to get to rt\
        private static string TestSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\unit_tests\\";

        private static string OldSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\unconverted\\";

        public static string GetConfigFilePath(string configFile)
        {
            return $"{ConfigDirectory}{configFile}";
        }

        public static string GetSceneFilePath(string sceneFile)
        {
            return $"{SceneDirectory}{sceneFile}";
        }

        public static string GetTestSceneFilePath(string sceneFile)
        {
            return $"{TestSceneDirectory}{sceneFile}";
        }

        public static string GetOldSceneFilePath(string oldSceneFile)
        {
            return $"{OldSceneDirectory}{oldSceneFile}";
        }

        public static string GetOutputFilePath(string outputFile)
        {
            return $"{OutputDirectory}{outputFile}";
        }
    }
}