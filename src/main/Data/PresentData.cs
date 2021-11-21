/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Data
{
    using Newtonsoft.Json;
    using rt.Utility;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class SceneData : DataBase
    {
        // #idea Considering these are all of type IData, could store them in a list and do a lookup to "get" them. Would make it simpler to process all data at once (error checking, printing, etc)
        // #todo Get rid of the ImageData from the SceneData, load it in another way, maybe read in a standard config file that contains image data & scene filename
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

        public void AddShape(PolygonData polygonData)
        {
            Debug.Assert(this.Shapes != null && this.Shapes.Polygons != null);

            this.Shapes.Polygons.Add(polygonData);
        }

        public void AddShape(EllipsoidData ellipsoidData)
        {
            Debug.Assert(this.Shapes != null && this.Shapes.Ellipsoids != null);
            this.Shapes.Ellipsoids.Add(ellipsoidData);
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

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            int indentation = spaceCount + 5;
            this.Image.PrintData(indentation);
            this.Camera.PrintData(indentation);
            this.Shapes.PrintData(indentation);
            this.Lights.PrintData(indentation);
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
        // Diffuse reflection coefficients
        [JsonProperty("diffuse")]
        public List<double> Diffuse { get; set; }

        // Specular reflection coefficients
        [JsonProperty("specularCoefficient ")]
        public double SpecularCoefficient { get; set; }

        [JsonProperty("specularExponent")]
        public double SpecularExponent { get; set; } // Phong model

        // Transmission attenuation factors
        [JsonProperty("transmissionAttenuation")]
        public List<double> TransmissionAttenuation;

        // Index of refraction
        [JsonProperty("electricPermittivity")]
        public double ElectricPermittivity { get; set; } // Relative

        [JsonProperty("magneticPermeability")]
        public double MagneticPermeability { get; set; } // Relative

        public MaterialData()
        {
            this.Diffuse = new List<double>();
            this.TransmissionAttenuation = new List<double>();
        }

        public override bool IsValid()
        {
            return DataFactory.ValidateList(this.Diffuse, 3) &&
                DataFactory.ValidateList(this.TransmissionAttenuation, 3) &&
                this.SpecularCoefficient >= 0.0f &&
                this.SpecularExponent >= 0.0f &&
                this.ElectricPermittivity >= 0.0f &&
                this.MagneticPermeability >= 0.0f;
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

        public new void PrintData(int spaceCount)
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

        [JsonProperty("radius")]
        public double Radius { get; set; }

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
            base.Print("Radius", this.Radius);
        }
    }

    // #todo Store AirData somewhere in the SceneData
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

    // #todo Store AmbientData somewhere in the SceneData
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