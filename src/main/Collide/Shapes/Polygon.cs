/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Collections.Generic;

namespace rt.Collide.Shapes
{
    using rt.Math;
    using rt.Present;

    internal class Polygon : Shape
    {
        private readonly List<Vec3> vertices;
        private readonly Plane plane;

        public Polygon(List<Vec3> vertices, Material material)
            : base(null, material)
        {
            this.vertices = vertices;
            this.plane = this.CalculatePlane();
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            // Test to see if ray intersects plane in a non-negative way
            // Test to see if ray/plane collision point exists within polygon
            return null;
        }

        private Plane CalculatePlane()
        {
            return null;
        }
    }
}