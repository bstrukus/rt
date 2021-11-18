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
        // #note I'm not sure if this is necessary to expose publicly, I would like for shapes to be interacted with through
        // functions rather than properties.
        protected Transform Transform { get; private set; }

        public Material Material { get; private set; }

        public Shape(Transform transform, Material material)
        {
            this.Transform = transform;
            this.Material = material;
        }

        public abstract HitInfo TryIntersect(Ray ray);

        // #todo Support function stubbed out for future AABB calculations
        //public abstract Vec3 Support(Vec3 direction);
    }
}