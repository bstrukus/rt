/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    using rt.Math;

    /// <summary>
    /// Point and direction, used to trace light paths through scene.
    /// </summary>
    public class Ray
    {
        public readonly Vec3 Origin;
        public readonly Vec3 Direction;

        public Ray(Vec3 origin, Vec3 direction, bool normalized = false)
        {
            this.Origin = origin;
            this.Direction = normalized ? direction : direction.Normalized();
        }

        public Vec3 GetPointAlong(float interpolationValue)
        {
            return this.Origin + interpolationValue * this.Direction;
        }
    }
}