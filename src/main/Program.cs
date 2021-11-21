/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt
{
    using Utility;

    internal class Program
    {
        // #todo Read this in as a command line parameter
        // #todo Read in config file specifying width, output file name, scene file
        private const string SceneFile = "ptest1.json";

        private static void Main(string[] args)
        {
            Log.Info("PROGRAM START");

            if (args.Length == 0)
            {
                LoadScene(Utility.Dir.GetSceneFilePath(SceneFile));
            }
            else if (args.Length == 1)
            {
                ConvertFile(args[0]);
            }

            Log.Info("PROGRAM END");
        }

        private static void LoadScene(string sceneFile)
        {
            rt.Data.DataFactory dataFactory = new rt.Data.DataFactory();
            dataFactory.Load(sceneFile);

            var scene = dataFactory.CreateScene();
            var camera = dataFactory.CreateCamera();
            var image = dataFactory.CreateImage();

            var runner = new rt.Execute.Runner(scene, camera, image);
            runner.Execute();
        }

        private static void ConvertFile(string filename)
        {
            rt.Utility.SceneConverter.Convert(filename);
        }
    }
}