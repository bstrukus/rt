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
            // #todo Figure out why I'm getting artifacts on near misses/hits
            // #precalc Matrix can be saved off as inverted
            var matrix = Mat3.FromColumns(this.uAxis, this.vAxis, this.wAxis);
            matrix = matrix.Inverted();

            Vec3 localDirection = matrix.Multiply(ray.Direction);
            Vec3 localRelativePoint = matrix.Multiply(ray.Origin - this.center);

            // a = |M^-1 d|^2
            float a = localDirection.LengthSq();

            // b = 2[M^-1(P - C)]*[M^-1 d]
            float b = 2.0f * Vec3.Dot(localRelativePoint, localDirection);

            // c = |M^-1 (P - C)|^2 - 1
            float c = localRelativePoint.LengthSq() - 1.0f;

            float discriminant = (b * b) - (4.0f * a * c);
            if (discriminant < 0.0f)
            {
                return null;
            }

            float sqrtDiscriminant = Numbers.Sqrt(discriminant);

            float hitValue;

            float posHitValue = (-b + sqrtDiscriminant) / (2.0f * a);
            if (posHitValue < 0.0f)
            {
                return null;
            }

            float negHitValue = (-b - sqrtDiscriminant) / (2.0f * a);
            if (negHitValue < 0.0f)
            {
                hitValue = posHitValue;
            }
            else
            {
                hitValue = negHitValue;
            }

            Vec3 hitPoint = ray.GetPointAlong(hitValue);
            Vec3 hitNormal = hitPoint - this.center;
            hitNormal = matrix.Multiply(hitNormal);
            hitNormal = matrix.Transposed().Multiply(hitNormal).Normalized();
            return new HitInfo(hitPoint, hitNormal, hitValue, this.Material);
        }
    }
}