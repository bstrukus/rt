/*
 * Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonObjects
{
    public class SceneData
    {
        [JsonProperty("image")]
        public ImageData Image { get; set; }

        [JsonProperty("shapes")]
        public ShapeData Shapes { get; set; }

        [JsonProperty("lights")]
        public LightData Lights { get; set; }

        public bool Validate()
        {
            if (this.Image == null)
            {
                return false;
            }
            return true;
        }

        public void PrintData()
        {
            int indentation = 3;
            Console.WriteLine("SCENE");
            this.Image.PrintData(indentation);
            this.Shapes.PrintData(indentation);
            this.Lights.PrintData(indentation);
        }
    }

    public class ImageData
    {
        [JsonProperty("stepFactor")]
        public int StepFactor { get; set; }

        [JsonProperty("format")]
        public string FileFormat { get; set; }

        public void PrintData(int spaceCount = 0)
        {
            string spaces = new string(' ', spaceCount);
            Console.WriteLine($"{spaces}IMAGE");
            Console.WriteLine($"{spaces} - StepFactor: {this.StepFactor}");
            Console.WriteLine($"{spaces} - FileFormat: {this.FileFormat}");
        }
    }

    public class TransformData
    {
        [JsonProperty("position")]
        public List<float> Position { get; set; }

        [JsonProperty("orientation")]
        public List<float> Orientation { get; set; }

        [JsonProperty("scale")]
        public List<float> Scale { get; set; }

        public void PrintData(int spaceCount = 0)
        {
            string spaces = new string(' ', spaceCount);

            Console.WriteLine($"{spaces}TRANSFORM");
            Console.WriteLine($"{spaces} - Position: [{this.Position[0]}, {this.Position[1]}, {this.Position[2]}]");
            Console.WriteLine($"{spaces} - Orientation: [{this.Orientation[0]}, {this.Orientation[1]}, {this.Orientation[2]}, {this.Orientation[3]}]");
            Console.WriteLine($"{spaces} - Scale: [{this.Scale[0]}, {this.Scale[1]}, {this.Scale[2]}]");
        }
    }

    public class ShapeData
    {
        [JsonProperty("spheres")]
        public List<SphereData> Spheres { get; set; }

        [JsonProperty("boxes")]
        public List<BoxData> Boxes { get; set; }

        public void PrintData(int spaceCount = 0)
        {
            string spaces = new string(' ', spaceCount);

            Console.WriteLine($"{spaces}SHAPES");

            int indentation = 3 + spaceCount;
            foreach (var sphere in this.Spheres)
            {
                sphere.PrintData(indentation);
            }
            foreach (var box in this.Boxes)
            {
                box.PrintData(indentation);
            }
        }
    }

    public class SphereData
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("radius")]
        public float Radius { get; set; }

        [JsonProperty("material")]
        public MaterialData Material { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            Console.WriteLine($"{spaces}SPHERE");
            this.Transform.PrintData(indentation);
            Console.WriteLine($"{spaces} - Radius: {this.Radius}");
            this.Material.PrintData(indentation);
        }
    }

    public class BoxData
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("Material")]
        public MaterialData Material { get; set; }

        public void PrintData(int indentAmount)
        {
            string indentation = new string(' ', indentAmount);

            Console.WriteLine($"{indentation}BOX");
            this.Transform.PrintData(indentAmount + 3);
            this.Material.PrintData(indentAmount + 3);
        }
    }

    public class PointData
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("color")]
        public List<float> Color { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            Console.WriteLine($"{spaces}POINT");
            this.Transform.PrintData(indentation);
            Console.WriteLine($"{spaces} - Color: [{this.Color[0]}, {this.Color[1]}, {this.Color[2]}]");
        }
    }

    public class LightData
    {
        [JsonProperty("points")]
        public List<PointData> PointLights { get; set; }

        public void PrintData(int spaceCount = 0)
        {
            string spaces = new string(' ', spaceCount);

            Console.WriteLine($"{spaces}LIGHTS");

            int indentation = 3 + spaceCount;
            foreach (var light in this.PointLights)
            {
                light.PrintData(indentation);
            }
        }
    }

    public class MaterialData
    {
        [JsonProperty("color")]
        public List<float> Color { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            Console.WriteLine($"{spaces}MATERIAL");
            Console.WriteLine($"{spaces} - Color: [{this.Color[0]}, {this.Color[1]}, {this.Color[2]}]");
        }
    }
}