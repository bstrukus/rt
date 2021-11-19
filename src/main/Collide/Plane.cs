/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    using rt.Math;

    /// <summary>
    /// Basic unit for several intersection tests. Not quite a <see cref="Shape"/> but close enough.
    /// </summary>
    internal class Plane
    {
        public Vec3 Point { get; private set; }
        public Vec3 Normal { get; private set; }

        public Plane(Vec3 point, Vec3 normal)
        {
            this.Point = point;
            this.Normal = normal;
        }

        public float CalcIntervalValue(Ray ray)
        {
            return -Vec3.Dot((ray.Origin - this.Point), this.Normal) / Vec3.Dot(ray.Direction, this.Normal);
        }
    }
}