/*
 * Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace rt.Data
{
    public class CameraData : DataBase
    {
        [JsonProperty("eyeDirection")]
        public List<double> EyeDirection { get; set; }

        [JsonProperty("projectionPlane")]
        public ProjectionPlaneData ProjectionPlane { get; set; }

        public CameraData()
        {
            this.EyeDirection = new List<double>();
        }

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

        public ProjectionPlaneData()
        {
            this.Center = new List<double>();
            this.UAxis = new List<double>();
            this.VAxis = new List<double>();
        }

        public override bool IsValid()
        {
            return DataFactory.ValidateList(this.Center, 3, false) &&
                DataFactory.ValidateList(this.UAxis, 3, false) &&
                DataFactory.ValidateList(this.VAxis, 3, false);
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);
            base.Print("Center", this.Center);
            base.Print("U-Axis", this.UAxis);
            base.Print("V-Axis", this.VAxis);
        }
    }
}