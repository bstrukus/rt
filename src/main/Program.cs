/*
 * Copyright Ben Strukus
 */

using System;
using System.IO;

namespace rt
{
    using Utility;

    internal class Program
    {
        private const string ScenePath = "..\\..\\scenes\\";

        private const string SceneFile = "simpleSphere.json";

        private static void Main(string[] args)
        {
            Log.Info("PROGRAM START");

            LoadScene(ScenePath + SceneFile);

            //             PixelBuffer pb = new PixelBuffer(256, 256);
            //             pb.Fill();
            //             pb.Save("test.bmp");
            Log.Info("PROGRAM END");
        }

        private static void LoadScene(string sceneFile)
        {
            rt.Data.DataFactory dataFactory = new rt.Data.DataFactory();
            dataFactory.Load(sceneFile);

            var scene = dataFactory.CreateScene();
            var camera = dataFactory.CreateCamera();

            var runner = new rt.Execute.Runner(camera, scene);
            runner.Execute();

            // Error checking is done on the SceneData side, should be fine to
            // build out the actual Scene in the constructor
            //             rt.Present.Scene scene = new rt.Present.Scene(sceneData);
            //
            //             rt.Execute.Runner runner = new rt.Execute.Runner(null, scene);
        }
    }
}