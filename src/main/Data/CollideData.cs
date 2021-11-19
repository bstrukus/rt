﻿/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Data
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ShapeData : DataBase
    {
        [JsonProperty("spheres")]
        public List<SphereData> Spheres { get; set; }

        [JsonProperty("boxes")]
        public List<BoxData> Boxes { get; set; }

        [JsonProperty("polygons")]
        public List<PolygonData> Polygons { get; set; }

        public bool HasSpheres() => this.Spheres != null && this.Spheres.Count > 0;

        public bool HasBoxes() => this.Boxes != null && this.Boxes.Count > 0;

        public bool HasPolygons() => this.Polygons != null && this.Polygons.Count > 0;

        public ShapeData()
        {
            this.Spheres = new List<SphereData>();
            this.Boxes = new List<BoxData>();
            this.Polygons = new List<PolygonData>();
        }

        public override bool IsValid()
        {
            bool hasShapes = this.HasSpheres() || this.HasBoxes() || this.HasPolygons();
            return hasShapes;
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            int indentation = 3 + spaceCount;
            this.PrintSpheres(indentation);
            this.PrintBoxes(indentation);
            this.PrintPolygons(indentation);
        }

        private void PrintSpheres(int indentation)
        {
            if (this.HasSpheres())
            {
                foreach (var sphere in this.Spheres)
                {
                    sphere.PrintData(indentation);
                }
            }
        }

        private void PrintBoxes(int indentation)
        {
            if (this.HasBoxes())
            {
                foreach (var box in this.Boxes)
                {
                    box.PrintData(indentation);
                }
            }
        }

        private void PrintPolygons(int indentation)
        {
            if (this.HasPolygons())
            {
                foreach (var polygon in this.Polygons)
                {
                    polygon.PrintData(indentation);
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

        [JsonProperty("corner")]
        public List<double> Corner { get; set; }

        [JsonProperty("lengthVector")]
        public List<double> LengthVector { get; set; }

        [JsonProperty("widthVector")]
        public List<double> WidthVector { get; set; }

        [JsonProperty("heightVector")]
        public List<double> HeightVector { get; set; }

        [JsonProperty("material")]
        public MaterialData Material { get; set; }

        public override bool IsValid()
        {
            return (this.HasTransform() || this.HasCornerAndDirectionVectors()) && this.Material.IsValid();
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            if (this.HasTransform())
            {
                this.Transform.PrintData(spaceCount + 3);
            }
            else if (this.HasCornerAndDirectionVectors())
            {
                base.Print("Corner", this.Corner);
                base.Print("Length", this.LengthVector);
                base.Print("Width", this.WidthVector);
                base.Print("Height", this.HeightVector);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
            this.Material.PrintData(spaceCount + 3);
        }

        public bool HasTransform()
        {
            return this.Transform != null && this.Transform.IsValid();
        }

        public bool HasCornerAndDirectionVectors()
        {
            return DataFactory.ValidateList(this.Corner, 3, shouldBeNormalizedValues: false) &&
                DataFactory.ValidateList(this.LengthVector, 3, shouldBeNormalizedValues: false) &&
                DataFactory.ValidateList(this.WidthVector, 3, shouldBeNormalizedValues: false) &&
                DataFactory.ValidateList(this.HeightVector, 3, shouldBeNormalizedValues: false);
        }
    }

    // #todo Support Polygons
    public class PolygonData : DataBase
    {
        [JsonProperty("transform")]
        public TransformData Transform { get; set; }

        [JsonProperty("material")]
        public MaterialData Material { get; set; }

        [JsonProperty("vertices")]
        public List<List<double>> Vertices { get; set; }

        public override bool IsValid()
        {
            return this.Transform.IsValid() && this.Material.IsValid() && this.HasVertices();
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            int i = 0;
            foreach (var vertex in this.Vertices)
            {
                base.Print($"Vertex {i++}", vertex);
            }
            this.Transform.PrintData(spaceCount + 3);
            this.Material.PrintData(spaceCount + 3);
        }

        private bool HasVertices()
        {
            return this.Vertices != null && this.Vertices.Count > 2;
        }
    }

    // #todo Support Ellipsoids
    public class EllipsoidData : DataBase
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
}