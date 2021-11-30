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
        /* #todo Need to have this class handle all file path resolution in the following ways
         * 1. Check to see if a directory was provided for the location of the scenes, if so use that
         * 2. Else, check to see if the config/scene files are adjacent to the EXE, if so use that
         * 3. Else, hardcode the dev paths as they currently exist and use that
         * - The idea is to check to see if a file exists at the location in the specific step instead of just assuming so
         * - Need to move all path-building logic here, remove that logic from ConfigData
         * - For ConfigData, there's only 2 options
         *   1. Check to see if that file is adjacent to the EXE
         *   2. Check to see if that file is in the dev path
         */

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