/*
 * Copyright Ben Strukus
 */

using System;
using System.IO;

namespace rt
{
    internal class Program
    {
        private const string ScenePath = "..\\..\\scenes\\";

        private const string SceneFile = "jsonTest.json";

        private static void Main(string[] args)
        {
            Console.WriteLine("PROGRAM START");

            LoadScene(ScenePath + SceneFile);

            //             PixelBuffer pb = new PixelBuffer(256, 256);
            //             pb.Fill();
            //             pb.Save("test.bmp");
            Console.WriteLine("PROGRAM END");
        }

        private static void LoadScene(string sceneFile)
        {
            using StreamReader file = File.OpenText(sceneFile);
            string fileContents = file.ReadToEnd();
            Console.WriteLine($"FILE: {fileContents}\r\n");

            var sceneData = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.SceneData>(fileContents);
            if (!sceneData.Validate())
            {
                Console.WriteLine("Error parsing JSON");
                return;
            }

            sceneData.PrintData();
        }
    }
}