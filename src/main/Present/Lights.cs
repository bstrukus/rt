/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Present
{
    using rt.Math;

    public abstract class Light
    {
        public Transform Transform { get; private set; }
        public Vec3 Color { get; private set; }

        public Light(Transform transform, Vec3 color)
        {
            this.Transform = transform;
            this.Color = color;
        }
    }

    public class PointLight : Light
    {
        public PointLight(Transform transform, Vec3 color)
            : base(transform, color)
        {
            //
        }
    }
}