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
            this.Image.PrintData();
            this.Shapes.PrintData();
        }
    }

    public class ImageData
    {
        [JsonProperty("stepFactor")]
        public int StepFactor { get; set; }

        [JsonProperty("format")]
        public string FileFormat { get; set; }

        public void PrintData()
        {
            Console.WriteLine("IMAGE");
            Console.WriteLine($" - StepFactor: {this.StepFactor}");
            Console.WriteLine($" - FileFormat: {this.FileFormat}");
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

        public void PrintData()
        {
            Console.WriteLine("SHAPES");

            int indentation = 3;
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

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            Console.WriteLine($"{spaces}SPHERE");
            this.Transform.PrintData(indentation);
            Console.WriteLine($"{spaces} - Radius: {this.Radius}");
        }
    }

    public class BoxData
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        public void PrintData(int indentAmount)
        {
            string indentation = new string(' ', indentAmount);

            Console.WriteLine($"{indentation}BOX");
            this.Transform.PrintData(indentAmount + 3);
        }
    }
}