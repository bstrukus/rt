/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide.Shapes
{
    using rt.Math;
    using rt.Present;
    using System;

    /// <summary>
    /// A point and a radius, but in the case of material orientation it has that as well.
    /// </summary>
    public class Sphere : Shape
    {
        // #todo Make every property private, keep these Shape classes locked down
        // A unit sphere has a diameter of 1, so a radius of 1/2
        public float Radius { get; private set; }

        public Vec3 Center => this.Transform.Position;

        public Sphere(Transform transform, Material material, float radius)
            : base(transform, material)
        {
            this.Radius = radius;
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            // Vector from sphere origin to ray origin
            Vec3 m = ray.Origin - this.Center;

            float b = Vec3.Dot(m, ray.Direction);
            float c = Vec3.Dot(m, m) - (this.Radius * this.Radius);

            // Exit if ray's origin is outside sphere (c > 0) and
            // ray is pointing away from sphere (b > 0)
            if (c > 0.0f && b > 0.0f)
            {
                return null;
            }

            float discr = b * b - c;

            // A negative discriminant corresponds to ray missing sphere
            if (discr < 0.0f)
            {
                return null;
            }

            // Ray now found to intersect sphere, compute smallest t-value of intersection
            float sqrt = MathF.Sqrt(discr);
            float t = -b - sqrt;

            // If t is negative, ray started inside sphere so clamp t to zero
            // #todo Revisit this, might need to be altered to support refractions
            if (t < 0.0f)
            {
                t = 0.0f;
            }

            Vec3 hitPoint = ray.GetPointAlong(t);

            // #todo Figure out how I want to normalize this on-demand instead of every time
            Vec3 normal = (hitPoint - this.Transform.Position).Normalized();

            return new HitInfo(hitPoint, normal, t, base.Material);
        }
    }
}