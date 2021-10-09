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

        [JsonProperty("camera")]
        public CameraData Camera { get; set; }

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
            this.Camera.PrintData(indentation);
            this.Shapes.PrintData(indentation);
            this.Lights.PrintData(indentation);
        }

        static public void PrintTitle(int spaceCount, string title)
        {
            string spaces = new string(' ', spaceCount - 2);
            Console.WriteLine($"{spaces}- {title}");
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
            SceneData.PrintTitle(spaceCount, "IMAGE");
            Console.WriteLine($"{spaces} - StepFactor: {this.StepFactor}");
            Console.WriteLine($"{spaces} - FileFormat: {this.FileFormat}");
        }
    }

    public class TransformData
    {
        [JsonProperty("position")]
        public List<double> Position { get; set; }

        [JsonProperty("orientation")]
        public List<double> Orientation { get; set; }

        [JsonProperty("scale")]
        public List<double> Scale { get; set; }

        public void PrintData(int spaceCount = 0)
        {
            string spaces = new string(' ', spaceCount);

            SceneData.PrintTitle(spaceCount, "TRANSFORM");
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

            SceneData.PrintTitle(spaceCount, "SHAPES");

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
        public double Radius { get; set; }

        [JsonProperty("material")]
        public MaterialData Material { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            SceneData.PrintTitle(spaceCount, "SPHERE");
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

            SceneData.PrintTitle(indentAmount, "BOX");
            this.Transform.PrintData(indentAmount + 3);
            this.Material.PrintData(indentAmount + 3);
        }
    }

    public class PointData
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("color")]
        public List<double> Color { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            SceneData.PrintTitle(spaceCount, "POINT");
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

            SceneData.PrintTitle(spaceCount, "LIGHTS");

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
        public List<double> Color { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);
            int indentation = spaceCount + 3;

            SceneData.PrintTitle(spaceCount, "MATERIAL");
            Console.WriteLine($"{spaces} - Color: [{this.Color[0]}, {this.Color[1]}, {this.Color[2]}]");
        }
    }

    public class ProjectionPlaneData
    {
        [JsonProperty("center")]
        public List<double> Center { get; set; }

        [JsonProperty("uAxis")]
        public List<double> UAxis { get; set; }

        [JsonProperty("vAxis")]
        public List<double> VAxis { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);

            SceneData.PrintTitle(spaceCount, "PROJECTION PLANE");
            Console.WriteLine($"{spaces} - Center: [{this.Center[0]}, {this.Center[1]}, {this.Center[2]}]");
            Console.WriteLine($"{spaces} - U-Axis: [{this.UAxis[0]}, {this.UAxis[1]}, {this.UAxis[2]}]");
            Console.WriteLine($"{spaces} - V-Axis: [{this.VAxis[0]}, {this.VAxis[1]}, {this.VAxis[2]}]");
        }
    }

    public class CameraData
    {
        [JsonProperty("eyePosition")]
        public List<double> EyePosition { get; set; }

        [JsonProperty("projectionPlane")]
        public ProjectionPlaneData ProjectionPlane { get; set; }

        public void PrintData(int spaceCount)
        {
            string spaces = new string(' ', spaceCount);

            SceneData.PrintTitle(spaceCount, "CAMERA");
            Console.WriteLine($"{spaces} - Eye Position: [{this.EyePosition[0]}, {this.EyePosition[1]}, {this.EyePosition[2]}]");
            this.ProjectionPlane.PrintData(spaceCount + 3);
        }
    }
}