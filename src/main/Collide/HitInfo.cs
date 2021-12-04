/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    using rt.Math;
    using rt.Present;

    /// <summary>
    /// Data returned from a collision.
    /// </summary>
    public class HitInfo
    {
        /// <summary>
        /// First point of intersection with ray.
        /// </summary>
        public Vec3 Point { get; private set; }

        /// <summary>
        /// Normal at point of intersection.
        /// </summary>
        public Vec3 Normal { get; private set; }

        /// <summary>
        /// Distance along ray of first point of intersection.
        /// </summary>
        public float Distance { get; private set; }

        /// <summary>
        /// Material information of the object hit.
        /// </summary>
        public Material Material { get; private set; }

        public HitInfo(Vec3 hitPoint, Vec3 surfaceNormal, float hitDistance, Material material)
        {
            this.Point = hitPoint;
            this.Normal = surfaceNormal;
            this.Distance = hitDistance;

            this.Material = material;
        }

        public void InvertNormal()
        {
            this.Normal = -this.Normal;
        }
    }
}