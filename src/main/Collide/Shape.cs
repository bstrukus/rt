/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    using rt.Present;

    /// <summary>
    /// A basic geometric shape, intersections with rays can be considered the result of a simple
    /// mathematical formula.
    /// </summary>
    public abstract class Shape : IHittable
    {
        public Transform Transform { get; private set; }
        public Material Material { get; private set; }

        public Shape(Transform transform, Material material)
        {
            this.Transform = transform;
            this.Material = material;
        }

        public abstract HitInfo TryIntersect(Ray ray);
    }
}