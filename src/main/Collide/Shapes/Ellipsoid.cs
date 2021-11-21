/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide.Shapes
{
    using rt.Math;
    using rt.Present;

    public class Ellipsoid : Shape
    {
        private Vec3 center;
        private Vec3 uAxis;
        private Vec3 vAxis;
        private Vec3 wAxis;

        public Ellipsoid(Vec3 center, Vec3 uAxis, Vec3 vAxis, Vec3 wAxis, Material material)
            : base(null, material)
        {
            this.center = center;
            this.uAxis = uAxis;
            this.vAxis = vAxis;
            this.wAxis = wAxis;
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            return null;
        }
    }
}