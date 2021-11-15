/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    /// <summary>
    /// An object that a ray can intersect.
    /// </summary>
    public interface IHittable
    {
        HitInfo TryIntersect(Ray ray);
    }
}