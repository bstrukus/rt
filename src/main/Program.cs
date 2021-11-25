/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt
{
    using Utility;
    using Execute;

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
            var runner = Load(args[0]);
            LogEnd("LOAD");

            if (runner == null)
            {
                return;
            }

            LogStart("RENDERING");
            runner.Execute();
            LogEnd("RENDERING");

            LogEnd("PROGRAM");
        }

        private static Runner Load(string configFile)
        {
            LogStart("CONFIG");
            Data.DataFactory dataFactory = new Data.DataFactory();
            var configData = dataFactory.CreateConfig(configFile);
            if (configData == null)
            {
                Log.Error("Failure when loading config file");
            }
            LogEnd("CONFIG");

            if (configData.IsSceneConversion)
            {
                LogStart("CONVERSION");
                // #todo Use the ConfigData to get this file information in the SceneConverter, maybe pass in the ConfigData?
                SceneConverter.Convert(configData.SceneFile);
                LogEnd("CONVERSION");
                return null;
            }
            else if (configData.IsSceneRenderable)
            {
                LogStart("LOAD SCENE");
                dataFactory.LoadScene(configData.GetSceneFullFilePath());

                var scene = dataFactory.CreateScene();
                var camera = dataFactory.CreateCamera();
                var image = dataFactory.CreateImage();
                LogEnd("LOAD SCENE");

                return new Runner(scene, camera, image);
            }
            else
            {
                Log.Error("Scene was neither converted nor rendered, unknown error");
                return null;
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