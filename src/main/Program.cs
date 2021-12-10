/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt
{
    using Utility;
    using Execute;
    using System.Collections.Generic;

    internal class Program
    {
        private static void Main(string[] args)
        {
            LogStart("PROGRAM");

            if (args.Length != 1)
            {
                Log.Error("INCORRECT PARAMETERS");
            }

            LogStart("LOAD");
            var sceneRenderers = Load(args[0]);
            LogEnd("LOAD");

            if (sceneRenderers == null || sceneRenderers.Count == 0)
            {
                return;
            }

            foreach (var renderJob in sceneRenderers)
            {
                LogStart($"RENDERING: {renderJob.Name}");

                renderJob.Execute();

                LogEnd("RENDERING");
            }

            LogEnd("PROGRAM");
        }

        private static List<Runner> Load(string configFile)
        {
            LogStart("CONFIG");
            Data.DataFactory dataFactory = new Data.DataFactory();

            var configData = dataFactory.CreateConfig(configFile);
            if (configData == null)
            {
                Log.Error("Failure when loading config file");
            }
            LogEnd("CONFIG");
            //
            //             if (configData.IsSceneConversion)
            //             {
            //                 LogStart("CONVERSION");
            //                 // #todo Use the ConfigData to get this file information in the SceneConverter, maybe pass in the ConfigData?
            //                 SceneConverter.Convert(configData.SceneFile);
            //                 LogEnd("CONVERSION");
            //                 return null;
            //             }
            //             else
            {
                List<Runner> scenes = new List<Runner>(configData.SceneFiles.Count);
                foreach (var sceneFile in configData.SceneFiles)
                {
                    LogStart($"LOAD SCENE: {sceneFile}");
                    dataFactory.LoadScene(sceneFile);

                    var scene = dataFactory.CreateScene();
                    var camera = dataFactory.CreateCamera();
                    var image = dataFactory.CreateImage(configData, sceneFile);
                    scenes.Add(new Runner(scene, camera, image));

                    LogEnd("LOAD SCENE");
                }
                return scenes;
            }
        }

        private static void LogStart(string state)
        {
            Log.Info($"{state} START");
        }

        private static void LogEnd(string state)
        {
            Log.Info($"{state} END");
        }
    }
}