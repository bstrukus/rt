/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Collections.Generic;
using System.Diagnostics;

namespace rt.Collide.Shapes
{
    using rt.Math;
    using rt.Present;

    public class Polygon : Shape
    {
        internal class Triangle
        {
            public Plane Plane { get; private set; }
            private readonly Vec3 anchorPoint;
            private readonly Vec3[] axes;

            public Triangle(Vec3 vertexA, Vec3 vertexB, Vec3 vertexC)
            {
                this.anchorPoint = vertexA;
                this.axes = new Vec3[]
                {
                    vertexB - this.anchorPoint,
                    vertexC - this.anchorPoint
                };

                this.Plane = new Plane(this.anchorPoint, Vec3.Cross(this.axes[0], this.axes[1]).Normalized());
            }

            public Vec2 CalcAffineCoordinates(Vec3 point)
            {
                float A = this.axes[0].Length();
                float B = this.axes[1].Length();
                float C = Vec3.Dot(this.axes[0], this.axes[1]);
                float det = 1.0f / (A * B - C * C);

                // #todo Mat2 - This math would be best handled by a 2x2 matrix but, needs must!
                Vec2 topRow = new Vec2(B, -C) * det;
                Vec2 botRow = new Vec2(-C, A) * det;

                Vec3 anchorToPoint = point - this.anchorPoint;
                Vec2 pointValues = new Vec2(
                    Vec3.Dot(anchorToPoint, this.axes[0]),
                    Vec3.Dot(anchorToPoint, this.axes[1]));
                float alpha = Vec.Dot(topRow, pointValues);
                float beta = Vec.Dot(botRow, pointValues);

                return new Vec2(alpha, beta);
            }
        }

        private readonly List<Triangle> triangles;

        public Polygon(List<Vec3> vertices, Material material)
            : base(null, material)
        {
            this.triangles = new List<Triangle>(vertices.Count - 2);
            Vec3 first = vertices[0];
            for (int i = 1; i < vertices.Count - 1; ++i)
            {
                var newTriangle = new Triangle(first, vertices[i], vertices[i + 1]);
                this.triangles.Add(newTriangle);
            }
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            Debug.Assert(triangles.Count != 0);

            // This will be the same for all triangles as they are all coplanar.

            // Test to see if ray intersects plane in a non-negative way
            foreach (var triangle in this.triangles)
            {
                float hitVal = triangle.Plane.CalcHitValue(ray);

                if (hitVal < 0.0f)
                {
                    return null;
                }

                // Test to see if ray/plane collision point exists within polygon
                Vec2 affineCoords = triangle.CalcAffineCoordinates(ray.GetPointAlong(hitVal));
                if (affineCoords.X >= 0.0f && affineCoords.Y >= 0.0f && (affineCoords.X + affineCoords.Y) <= 1.0f)
                {
                    return new HitInfo(ray.GetPointAlong(hitVal), triangle.Plane.Normal, hitVal, base.Material);
                }
            }

            return null;
        }
    }
}