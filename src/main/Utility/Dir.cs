/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using System;
    using System.IO;

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

        private static readonly string OutputDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        // Need to go up 4 levels to get to rt\
        private static readonly string ProjectRootDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\..\\";

        private static readonly string SceneDirectory = $"{ProjectRootDirectory}scenes\\";
        private static readonly string TestSceneDirectory = $"{SceneDirectory}unit_tests\\";
        private static readonly string OldSceneDirectory = $"{SceneDirectory}unconverted\\";

        public static string Read(string filename)
        {
            string filepath;    // TBD by code below

            //////////////////////////////////////////////////////////////////////////
            /// Parameters adjacent to the EXE
            if (File.Exists($"{OutputDirectory}{filename}"))
            {
                // Next to EXE
                Log.Info($"File \"{filename}\" found in the EXE's path.");
                filepath = $"{OutputDirectory}{filename}";
            }
            //////////////////////////////////////////////////////////////////////////
            /// #todo Paths specified by the config
            /// else if (...)
            //////////////////////////////////////////////////////////////////////////
            /// Project-relative directories, used for development
            else if (File.Exists($"{ProjectRootDirectory}{filename}"))
            {
                // At root of development directory
                // This is where the dev_config would normally live
                Log.Info($"File \"{filename}\" found in the root project path.");
                filepath = $"{ProjectRootDirectory}{filename}";
            }
            else if (File.Exists($"{SceneDirectory}{filename}"))
            {
                // In the scene folder, part of the normal dev setup
                Log.Info($"File \"{filename}\" found in the scene folder path.");
                filepath = $"{SceneDirectory}{filename}";
            }
            else if (File.Exists($"{TestSceneDirectory}{filename}"))
            {
                // In the unit test scene folder, part of the normal dev setup
                Log.Info($"File \"{filename}\" found in the test scene folder path.");
                filepath = $"{TestSceneDirectory}{filename}";
            }
            else if (File.Exists($"{OldSceneDirectory}{filename}"))
            {
                // #todo Remove this once scene conversion is handled by Python scripts
                // In the unconverted scene directory
                Log.Info($"File \"{filename}\" found in the unconverted scene folder path.");
                filepath = $"{OldSceneDirectory}{filename}";
            }
            else
            {
                Log.Error($"File \"{filename}\" not found in any of the expected paths!");
                return string.Empty;
            }

            using StreamReader file = File.OpenText(filepath);
            string fileContents = file.ReadToEnd();
            return fileContents;
        }

        public static string GetSceneFilePath(string sceneFile)
        {
            return $"{SceneDirectory}{sceneFile}";
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