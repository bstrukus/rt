﻿/*
 * Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace rt.Data
{
    using Collide;
    using Present;

    using System.Diagnostics;
    using System.IO;

    public static class Helpers
    {
        public static Math.Vec3 Vec3FromListOfDoubles(List<double> vec3)
        {
            Debug.Assert(vec3 != null);
            Debug.Assert(vec3.Count == 3);
            return new Math.Vec3((float)vec3[0], (float)vec3[1], (float)vec3[2]);
        }
    };

    /// <summary>
    /// Consumes a data file, outputs components of the ray tracer
    /// </summary>
    public class DataFactory
    {
        private SceneData sceneData;

        public bool Load(string filename)
        {
            using StreamReader file = File.OpenText(filename);
            string fileContents = file.ReadToEnd();
            Console.WriteLine($"FILE: {fileContents}\r\n");

            this.sceneData = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.SceneData>(fileContents);
            if (!sceneData.Validate())
            {
                Console.WriteLine("Error parsing JSON");
                return false;
            }

            this.sceneData.PrintData();
            return true;
        }

        public Scene CreateScene()
        {
            if (!this.sceneData.HasShapes())
            {
                return null;
            }

            var shapeCollection = this.sceneData.Shapes;

            // Go through the list of Spheres & Boxes to create a list of IHittables
            List<IHittable> hittables = new List<IHittable>();
            if (shapeCollection.HasSpheres())
            {
                foreach (var sphereData in shapeCollection.Spheres)
                {
                    hittables.Add(new Sphere(
                        this.CreateTransform(sphereData.Transform),
                        this.CreateMaterial(sphereData.Material),
                        (float)sphereData.Radius
                        ));
                }
            }

            if (shapeCollection.HasBoxes())
            {
                foreach (var boxData in shapeCollection.Boxes)
                {
                    hittables.Add(new Box(
                        this.CreateTransform(boxData.Transform),
                        this.CreateMaterial(boxData.Material)
                        ));
                }
            }

            return new Scene(hittables);
        }

        private Transform CreateTransform(TransformData data)
        {
            return new Transform();
        }

        private Material CreateMaterial(MaterialData data)
        {
            return new Material();
        }

        //         public Camera(Data.CameraData cameraData)
        //         {
        //             this.eyePosition = Data.Helpers.Vec3FromListOfDoubles(cameraData.EyePosition);
        //
        //             this.projectionPlane = new ProjectionPlane(cameraData.ProjectionPlane);
        //             this.stepCount = new int[] {
        //                     this.HorizontalStepCount * (int)projectionPlane.HorizontalScale,
        //                     this.projectionPlane. * (int)projectionPlane.VerticalScale
        //                 };
        //         }
    }

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

        public bool HasShapes() => this.Shapes != null && (this.Shapes.HasSpheres() || this.Shapes.HasBoxes());

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
            this.Image?.PrintData(indentation);
            this.Camera?.PrintData(indentation);
            this.Shapes?.PrintData(indentation);
            this.Lights?.PrintData(indentation);
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

        public bool HasSpheres() => this.Spheres != null && this.Spheres.Count > 0;

        public bool HasBoxes() => this.Boxes != null && this.Boxes.Count > 0;

        public void PrintData(int spaceCount = 0)
        {
            string spaces = new string(' ', spaceCount);

            SceneData.PrintTitle(spaceCount, "SHAPES");

            int indentation = 3 + spaceCount;
            this.PrintSpheres(indentation);
            this.PrintBoxes(indentation);
        }

        private void PrintSpheres(int indentation)
        {
            // Print out spheres
            if (this.Spheres != null)
            {
                foreach (var sphere in this.Spheres)
                {
                    sphere.PrintData(indentation);
                }
            }
        }

        private void PrintBoxes(int indentation)
        {
            if (this.Boxes != null)
            {
                foreach (var box in this.Boxes)
                {
                    box.PrintData(indentation);
                }
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

        [JsonProperty("material")]
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