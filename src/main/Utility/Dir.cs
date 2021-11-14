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
        // App dir: "rt\OUTPUT\rt\<target_config>\<netcoreapp_ver>\"
        // Need to go up 2 levels to get to rt\
        static private string SceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\";

        // Test dir: "rt\OUTPUT\unit_tests\<target_config>\<netcoreapp_ver>\"
        // Need to go up 5 levels to get to rt\
        static private string TestSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\unit_tests\\";

        static private string OldSceneDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\scenes\\unconverted\\";

        static public string GetSceneFilePath(string sceneFile)
        {
            return $"{SceneDirectory}{sceneFile}";
        }

        static public string GetTestSceneFilePath(string sceneFile)
        {
            return $"{TestSceneDirectory}{sceneFile}";
        }

        static public string GetOldSceneFilePath(string oldSceneFile)
        {
            return $"{OldSceneDirectory}{oldSceneFile}";
        }
    }
}