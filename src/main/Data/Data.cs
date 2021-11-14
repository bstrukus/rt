﻿/*
 * Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace rt.Data
{
    using Collide;
    using Present;
    using Render;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Utility;

    public static class Helpers
    {
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
            //Log.Info($"FILE", fileContents}\r\n");

            this.sceneData = Newtonsoft.Json.JsonConvert.DeserializeObject<SceneData>(fileContents);
            if (this.sceneData == null || !this.sceneData.IsValid())
            {
                Log.Error("Error loading Scene");
                return false;
            }

            // #todo Make this togglable, debug option
            this.sceneData.PrintData();
            return true;
        }

        public Scene CreateScene()
        {
            //////////////////////////////////////////////////////////////////////////
            var shapeCollection = this.sceneData.Shapes;

            // Go through the list of Spheres & Boxes to create a list of IHittables
            List<IHittable> hittables = new List<IHittable>();
            if (shapeCollection.HasSpheres)
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

            if (shapeCollection.HasBoxes)
            {
                foreach (var boxData in shapeCollection.Boxes)
                {
                    hittables.Add(new Box(
                        this.CreateTransform(boxData.Transform),
                        this.CreateMaterial(boxData.Material)
                        ));
                }
            }

            //////////////////////////////////////////////////////////////////////////
            var lightCollection = this.sceneData.Lights;
            List<Light> lights = new List<Light>();
            if (lightCollection.IsValid())
            {
                foreach (var pointLightData in lightCollection.PointLights)
                {
                    lights.Add(new PointLight(
                        this.CreateTransform(pointLightData.Transform),
                        DoubleListToVec3(pointLightData.Color)
                        ));
                }
            }

            return new Scene(hittables, lights);
        }

        public Camera CreateCamera()
        {
            var cameraData = this.sceneData.Camera;

            return new Camera(
                eyeDirection: DoubleListToVec3(cameraData.EyeDirection),
                plane: this.CreateProjectionPlane());
        }

        public Image CreateImage()
        {
            var imageData = this.sceneData.Image;
            var camera = this.CreateCamera();

            Debug.Assert(imageData != null);
            Debug.Assert(camera != null);

            float aspectRatio = camera.AspectRatio;
            int width = imageData.Width;
            int height = (int)((float)width / aspectRatio);

            return new Image(width, height, imageData.FileName);
        }

        private ProjectionPlane CreateProjectionPlane()
        {
            var projectionPlaneData = this.sceneData.Camera.ProjectionPlane;

            return new ProjectionPlane(
                center: DoubleListToVec3(projectionPlaneData.Center),
                horizontalAxis: DoubleListToVec3(projectionPlaneData.UAxis),
                verticalAxis: DoubleListToVec3(projectionPlaneData.VAxis));
        }

        private Transform CreateTransform(TransformData data)
        {
            return new Transform(
                position: DoubleListToVec3(data.Position),
                orientation: DoubleListToQuat(data.Orientation),
                scale: DoubleListToVec3(data.Scale));
        }

        private Material CreateMaterial(MaterialData data)
        {
            return new Material(
                color: DoubleListToVec3(data.Diffuse));
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

        public static Math.Vec3 DoubleListToVec3(List<double> vec3)
        {
            Debug.Assert(vec3 != null);
            Debug.Assert(vec3.Count == 3);
            return new Math.Vec3((float)vec3[0], (float)vec3[1], (float)vec3[2]);
        }

        public static Math.Quat DoubleListToQuat(List<double> quat)
        {
            Debug.Assert(quat != null);
            Debug.Assert(quat.Count == 4);
            return new Math.Quat((float)quat[0], (float)quat[1], (float)quat[2], (float)quat[3]);
        }

        public static bool ValidateList(List<double> vector, int expectedCount)
        {
            return vector != null && vector.Count == expectedCount;
        }


    }

    public abstract class DataBase
    {
        private int spaceCount;
        private string indentation
        {
            get
            {
                string spaces = new string(' ', this.spaceCount);

                return $"{spaces} - ";
            }
        }

        // Error checking
        abstract public bool IsValid();

        // Pretty-Print
        virtual public void PrintData(int spaceCount = 0)
        {
            this.spaceCount = spaceCount;
            this.PrintTitle(this.GetType().Name.ToUpper());
        }

        virtual public void Print(string label, string value)
        {
            Log.Info($"{this.indentation}{label}: {value}");
        }
        virtual public void Print(string label, int value)
        {
            Print(label, value.ToString());
        }
        virtual public void Print(string label, double value)
        {
            Print(label, value.ToString());
        }
        virtual public void Print(string label, List<double> list)
        {
            Log.Info($"{this.indentation}{label}: [{Format(list)}]");
        }

        public void PrintTitle(string label)
        {
            string spaces = new string(' ', this.spaceCount - 2);
            Log.Info($"{spaces}- {label}");
        }

        private static string Format(List<double> vectorDoubles)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < vectorDoubles.Count; ++i)
            {
                sb.Append($"{vectorDoubles[i]}");
                if (i != (vectorDoubles.Count - 1))
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }

    public class SceneData : DataBase
    {
        // #idea Considering these are all of type IData, could store them in a list and do a lookup to "get" them. Would make it simpler to process all data at once (error checking, printing, etc)
        [JsonProperty("image")]
        public ImageData Image { get; set; }

        [JsonProperty("camera")]
        public CameraData Camera { get; set; }

        [JsonProperty("shapes")]
        public ShapeData Shapes { get; set; }

        [JsonProperty("lights")]
        public LightData Lights { get; set; }

        public SceneData()
        {
            // Initialize those data containers that are glorified Lists
            this.Shapes = new ShapeData();
            this.Lights = new LightData();
        }

        public void AddShape(SphereData sphereData)
        {
            Debug.Assert(this.Shapes != null && this.Shapes.Spheres != null);
            this.Shapes.Spheres.Add(sphereData);
        }

        public void AddShape(BoxData boxData)
        {
            Debug.Assert(this.Shapes != null && this.Shapes.Boxes != null);
            this.Shapes.Boxes.Add(boxData);
        }

        public void AddLight(PointLightData pointLightData)
        {
            Debug.Assert(this.Lights != null && this.Lights.PointLights != null);
            this.Lights.PointLights.Add(pointLightData);
        }

        public override bool IsValid()
        {
            if (!this.IsValid(this.Image))
            {
                Log.Error("Image data could not be loaded");
                return false;
            }

            if (!this.IsValid(this.Camera))
            {
                Log.Error("Camera data could not be loaded");
                return false;
            }

            if (!this.IsValid(this.Shapes))
            {
                Log.Error("Shape data could not be loaded");
                return false;
            }

            if (!this.IsValid(this.Lights))
            {
                Log.Error("Light data could not be loaded");
                return false;
            }

            return true;
        }

        private bool IsValid(DataBase data)
        {
            return data != null && data.IsValid();
        }

        public new void PrintData(int spaceCount = 0)
        {
            base.PrintData(spaceCount);

            int indentation = 3;
            Log.Info("SCENE");
            this.Image.PrintData(indentation);
            this.Camera.PrintData(indentation);
            this.Shapes.PrintData(indentation);
            this.Lights.PrintData(indentation);
        }
    }

    public class ImageData : DataBase
    {
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        public override bool IsValid()
        {
            return this.Width > 0 && !string.IsNullOrEmpty(this.FileName);
        }

        public new void PrintData(int spaceCount = 0)
        {
            base.PrintData(spaceCount);
            base.Print("Width", this.Width);
            base.Print("FileName", this.FileName);
        }
    }

    public class CameraData : DataBase
    {
        [JsonProperty("eyeDirection")]
        public List<double> EyeDirection { get; set; }

        [JsonProperty("projectionPlane")]
        public ProjectionPlaneData ProjectionPlane { get; set; }

        public override bool IsValid()
        {
            bool isProjectionPlaneValid = this.ProjectionPlane != null && this.ProjectionPlane.IsValid();
            return DataFactory.ValidateList(this.EyeDirection, 3) && isProjectionPlaneValid;
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);
            base.Print("Eye Direction", this.EyeDirection);
            this.ProjectionPlane.PrintData(spaceCount + 3);
        }
    }
    public class ProjectionPlaneData : DataBase
    {
        [JsonProperty("center")]
        public List<double> Center { get; set; }

        [JsonProperty("uAxis")]
        public List<double> UAxis { get; set; }

        [JsonProperty("vAxis")]
        public List<double> VAxis { get; set; }

        public override bool IsValid()
        {
            return DataFactory.ValidateList(this.Center, 3) &&
                DataFactory.ValidateList(this.UAxis, 3) &&
                DataFactory.ValidateList(this.VAxis, 3);
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);
            base.Print("Center", this.Center);
            base.Print("U-Axis", this.UAxis);
            base.Print("V-Axis", this.VAxis);
        }
    }

    public class TransformData : DataBase
    {
        [JsonProperty("position")]
        public List<double> Position { get; set; }

        [JsonProperty("orientation")]
        public List<double> Orientation { get; set; }

        [JsonProperty("scale")]
        public List<double> Scale { get; set; }

        public TransformData()
        {
            this.Position = new List<double>();
            this.Orientation = new List<double>();
            this.Scale = new List<double>();
        }

        public override bool IsValid()
        {
            return DataFactory.ValidateList(this.Position, 3) &&
                DataFactory.ValidateList(this.Orientation, 4) &&
                DataFactory.ValidateList(this.Scale, 3);
        }

        public new void PrintData(int spaceCount = 0)
        {
            base.PrintData(spaceCount);
            base.Print("Position", this.Position);
            base.Print("Orientation", this.Orientation);
            base.Print("Scale", this.Scale);
        }
    }

    public class MaterialData : DataBase
    {
        // #todo Rename to "Diffuse"
        // Diffuse reflection coefficients
        [JsonProperty("color")]
        public List<double> Diffuse { get; set; }

        // Specular reflection coefficients
        public float SpecularCoefficient { get; set; }
        public float SpecularExponent { get; set; } // Phong model

        // Transmission attenuation factors
        public List<double> TransmissionAttenuation;

        // Index of refraction
        public float ElectricPermittivity { get; set; } // Relative
        public float MagneticPermeability { get; set; } // Relative


        public MaterialData()
        {
            this.Diffuse = new List<double>();
            this.TransmissionAttenuation = new List<double>();
        }

        public override bool IsValid()
        {
            return DataFactory.ValidateList(this.Diffuse, 3) &&
                DataFactory.ValidateList(this.TransmissionAttenuation, 3);
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);
            base.Print("Diffuse", this.Diffuse);
            base.Print("Specular Coefficient", this.SpecularCoefficient);
            base.Print("Specular Exponent", this.SpecularExponent);
            base.Print("Transmission Attenuation", this.TransmissionAttenuation);
            base.Print("Electric Permittivity", this.ElectricPermittivity);
            base.Print("Magnetic Permeability", this.MagneticPermeability);
        }
    }

    public class ShapeData : DataBase
    {
        [JsonProperty("spheres")]
        public List<SphereData> Spheres { get; set; }

        [JsonProperty("boxes")]
        public List<BoxData> Boxes { get; set; }

        public bool HasSpheres => this.Spheres != null && this.Spheres.Count > 0;

        public bool HasBoxes => this.Boxes != null && this.Boxes.Count > 0;

        public ShapeData()
        {
            this.Spheres = new List<SphereData>();
            this.Boxes = new List<BoxData>();
        }

        public override bool IsValid()
        {
            bool hasShapes = this.HasSpheres || this.HasBoxes;
            return hasShapes;
        }

        public new void PrintData(int spaceCount = 0)
        {
            base.PrintData(spaceCount);

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

    public class SphereData : DataBase
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("radius")]
        public double Radius { get; set; }

        [JsonProperty("material")]
        public MaterialData Material { get; set; }

        public override bool IsValid()
        {
            return this.Transform.IsValid() && this.Material.IsValid() && this.Radius > 0.0f;
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            base.Print("Radius", this.Radius);

            int indentation = spaceCount + 3;
            this.Transform.PrintData(indentation);
            this.Material.PrintData(indentation);
        }
    }

    public class BoxData : DataBase
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("material")]
        public MaterialData Material { get; set; }

        public override bool IsValid()
        {
            return this.Transform.IsValid() && this.Material.IsValid();
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            this.Transform.PrintData(spaceCount + 3);
            this.Material.PrintData(spaceCount + 3);
        }
    }

    public class LightData : DataBase
    {
        [JsonProperty("points")]
        public List<PointLightData> PointLights { get; set; }

        // #todo Spot/Shaped lights
        // #todo Ambient light

        public LightData()
        {
            this.PointLights = new List<PointLightData>();
        }

        public void AddLight(PointLightData pointLightData)
        {
            Debug.Assert(this.PointLights != null);
            this.PointLights.Add(pointLightData);
        }

        public override bool IsValid()
        {
            return this.PointLights != null && this.PointLights.Count > 0;
        }

        public new void PrintData(int spaceCount = 0)
        {
            base.PrintData(spaceCount);

            int indentation = 3 + spaceCount;
            foreach (var light in this.PointLights)
            {
                light.PrintData(indentation);
            }
        }
    }

    public class PointLightData : DataBase
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("color")]
        public List<double> Color { get; set; }


        public override bool IsValid()
        {
            return this.Transform.IsValid() && DataFactory.ValidateList(this.Color, 3);
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            int indentation = spaceCount + 3;
            this.Transform.PrintData(indentation);

            base.Print("Color", this.Color);
        }
    }

    public class AirData : DataBase
    {
        public override bool IsValid()
        {
            return false;
        }

        public new void PrintData(int spaces)
        {
            //
        }
    }

    public class AmbientData : DataBase
    {
        public override bool IsValid()
        {
            return false;
        }

        public new void PrintData(int spaces)
        {
            //
        }
    }
}