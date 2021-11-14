﻿/*
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
        // App dir: "rt\OUTPUT\rt\<target_config>\<netcoreapp_ver>\"
        // Need to go up 2 levels to get to rt\
        private static string SceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\";

        // Test dir: "rt\OUTPUT\unit_tests\<target_config>\<netcoreapp_ver>\"
        // Need to go up 5 levels to get to rt\
        private static string TestSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\unit_tests\\";

        private static string OldSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\unconverted\\";

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
    }
}