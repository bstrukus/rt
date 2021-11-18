/*
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

        public bool HasSpheres() => this.Spheres != null && this.Spheres.Count > 0;

        public bool HasBoxes() => this.Boxes != null && this.Boxes.Count > 0;

        public ShapeData()
        {
            this.Spheres = new List<SphereData>();
            this.Boxes = new List<BoxData>();
        }

        public override bool IsValid()
        {
            bool hasShapes = this.HasSpheres() || this.HasBoxes();
            return hasShapes;
        }

        public new void PrintData(int spaceCount)
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