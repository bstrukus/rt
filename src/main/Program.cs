/*
 * Copyright Ben Strukus
 */

using main;
using Newtonsoft.Json;
using System;
using System.IO;

namespace rt
{
    /*
    public class Vec3
    {
        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x { get; }
        public float y { get; }
        public float z { get; }
    }

    public class Image
    {
        [JsonProperty("stepFactor")]
        public int StepFactor { get; set; }

        [JsonProperty("fileExtension")]
        public string FileExtension { get; set; }
    }

    public class Orientation
    {
        [JsonProperty("x-axis")]
        public Vec3 xAxis { get; set; }

        [JsonProperty("y-axis")]
        public Vec3 yAxis { get; set; }

        [JsonProperty("z-axis")]
        public Vec3 zAxis { get; set; }
    }

    public class Transform
    {
        [JsonProperty("position")]
        public Vec3 position { get; set; }

        [JsonProperty("orientation")]
        public Orientation orientation { get; set; }

        [JsonProperty("scale")]
        public Vec3 scale { get; set; }
    }

    public class Camera
    {
        [JsonProperty("transform")]
        public Transform transform { get; set; }
    }

    //     public IShape
    //         {
    public class Scene
    {
        // Image
        [JsonProperty("image")]
        public Image Image { get; set; }

        // Camera
        //[JsonProperty("camera")]

        // Shapes
        [JsonProperty("shapes")]
        public Shape[] shapes { get; set; }

        // Lights
    }
    */

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

            var sceneData = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonObjects.SceneData>(fileContents);
            if (!sceneData.Validate())
            {
                Console.WriteLine("Error parsing JSON");
                return;
            }

            sceneData.PrintData();
        }
    }
}