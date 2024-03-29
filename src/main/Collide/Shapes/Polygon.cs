﻿/*
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
            private readonly Vec3 anchorPoint;
            private readonly Vec3[] axes;
            private readonly Mat2 affineTransform;

            public Triangle(Vec3 vertexA, Vec3 vertexB, Vec3 vertexC)
            {
                this.anchorPoint = vertexA;
                this.axes = new Vec3[]
                {
                    vertexB - this.anchorPoint,
                    vertexC - this.anchorPoint
                };

                float A = Vec3.Dot(this.axes[0], this.axes[0]);
                float B = Vec3.Dot(this.axes[1], this.axes[1]);
                float C = Vec3.Dot(this.axes[0], this.axes[1]);
                float det = 1.0f / (A * B - C * C);

                this.affineTransform = Mat2.FromRows(new Vec2(B, -C) * det,
                                                     new Vec2(-C, A) * det);
            }

            public Vec2 CalcAffineCoordinates(Vec3 point)
            {
                Vec3 anchorToPoint = point - this.anchorPoint;
                Vec2 pointValues = new Vec2(
                    Vec3.Dot(anchorToPoint, this.axes[0]),
                    Vec3.Dot(anchorToPoint, this.axes[1]));
                return this.affineTransform.Multiply(pointValues);
            }
        }

        private readonly Plane plane;
        private readonly List<Triangle> triangles;

        public Polygon(List<Vec3> vertices, Material material)
            : base(null, material)
        {
            Debug.Assert(vertices.Count > 2);
            this.triangles = new List<Triangle>(vertices.Count - 2);
            Vec3 first = vertices[0];

            this.plane = new Plane(first, Vec3.Cross(vertices[1] - first, vertices[2] - first).Normalized());

            for (int i = 1; i < vertices.Count - 1; ++i)
            {
                var newTriangle = new Triangle(first, vertices[i], vertices[i + 1]);
                this.triangles.Add(newTriangle);
            }
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            Debug.Assert(triangles.Count != 0);

            // Test to see if ray intersects plane in a non-negative way.  This will be the same for all triangles as they are
            // all coplanar.
            float hitVal = this.plane.CalcHitValue(ray);
            if (hitVal < 0.0f)
            {
                return null;
            }

            foreach (var triangle in this.triangles)
            {
                // Test to see if ray/plane collision point exists within polygon
                Vec2 affineCoords = triangle.CalcAffineCoordinates(ray.GetPointAlong(hitVal));
                if (affineCoords.X >= 0.0f && affineCoords.Y >= 0.0f && (affineCoords.X + affineCoords.Y) <= 1.0f)
                {
                    return new HitInfo(ray.GetPointAlong(hitVal), this.plane.Normal, hitVal, base.Material);
                }
            }

            return null;
        }
    }
}