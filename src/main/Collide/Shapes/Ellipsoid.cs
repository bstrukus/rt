/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide.Shapes
{
    using rt.Math;
    using rt.Present;

    public class Ellipsoid : Shape
    {
        private readonly Vec3 center;
        private readonly Vec3 uAxis;
        private readonly Vec3 vAxis;
        private readonly Vec3 wAxis;
        private readonly Mat3 affineTransform;

        public Ellipsoid(Vec3 center, Vec3 uAxis, Vec3 vAxis, Vec3 wAxis, Material material)
            : base(null, material)
        {
            this.center = center;
            this.uAxis = uAxis;
            this.vAxis = vAxis;
            this.wAxis = wAxis;

            var matrix = Mat3.FromColumns(this.uAxis, this.vAxis, this.wAxis);
            this.affineTransform = matrix.Inverted();
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            Vec3 localDirection = this.affineTransform.Multiply(ray.Direction);
            Vec3 localRelativePoint = this.affineTransform.Multiply(ray.Origin - this.center);

            // a = |M^-1 d|^2
            float a = localDirection.LengthSq();

            // b = 2[M^-1(P - C)]*[M^-1 d]
            float b = 2.0f * Vec3.Dot(localRelativePoint, localDirection);

            // c = |M^-1 (P - C)|^2 - 1
            float c = localRelativePoint.LengthSq() - 1.0f;

            // Exit if ray's origin is outside sphere (c > 0) and
            // ray is pointing away from sphere (b > 0)
            if (c > 0.0f && b > 0.0f)
            {
                return null;
            }

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

            Vec3 hitPoint = ray.GetPointAlong((float)hitValue);

            Vec3 hitNormal = hitPoint - this.center;
            hitNormal = this.affineTransform.Multiply(hitNormal);
            hitNormal = this.affineTransform.Transposed().Multiply(hitNormal).Normalized();

            return new HitInfo(hitPoint, hitNormal, (float)hitValue, this.Material);
        }
    }
}