/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt
{
    using Utility;

    internal class Program
    {
        // #todo Read this in as a command line parameter
        private const string SceneFile = "simpleSphere.json";

        private static void Main(string[] args)
        {
            Log.Info("PROGRAM START");

            LoadScene(Utility.Dir.GetSceneFilePath(SceneFile));

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
    }
}