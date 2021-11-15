/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    using rt.Math;
    using rt.Present;

    public class Box : Shape
    {
        public Vec3 HalfExtents => base.Transform.Scale * 0.5f;

        public Box(Transform transform, Material material)
            : base(transform, material)
        {
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            return null;
        }
    }
}