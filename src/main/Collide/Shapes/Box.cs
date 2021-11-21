/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide.Shapes
{
    using rt.Math;
    using rt.Present;

    public class Box : Shape
    {
        private readonly Plane[] planes;

        /// <summary>
        /// Box with orthogonal axes.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="material"></param>
        public Box(Transform transform, Material material)
            : base(transform, material)
        {
        }

        /// <summary>
        /// Parallelepiped defined by a corner point and three "basis" vectors.
        /// </summary>
        /// <param name="cornerPoint">Fixed point in space describing one corner of the parallelepiped.</param>
        /// <param name="lengthVector">Direction vector describing one axis of the parallelepiped.</param>
        /// <param name="widthVector">Direction vector describing one axis of the parallelepiped.</param>
        /// <param name="heightVector">Direction vector describing one axis of the parallelepiped.</param>
        /// <remarks>
        /// If the three vectors are orthogonal, then we have a normal, non-skewed box.
        /// </remarks>
        public Box(Vec3 cornerPoint, Vec3 lengthVector, Vec3 widthVector, Vec3 heightVector, Material material)
            : base(ParallelepipedTransform(cornerPoint, lengthVector, widthVector, heightVector), material)
        {
            Vec3 frontNormal = Vec3.Cross(lengthVector, heightVector).Normalized();
            Vec3 leftNormal = Vec3.Cross(heightVector, widthVector).Normalized();
            Vec3 bottomNormal = Vec3.Cross(widthVector, lengthVector).Normalized();

            this.planes = new Plane[]
            {
                new Plane(cornerPoint, frontNormal),                    // Front face
                new Plane(cornerPoint + widthVector, -frontNormal),     // Back face
                new Plane(cornerPoint, leftNormal),                     // Left face
                new Plane(cornerPoint + lengthVector, -leftNormal),     // Right face
                new Plane(cornerPoint, bottomNormal),                   // Bottom face
                new Plane(cornerPoint + heightVector, -bottomNormal)    // Top face
            };
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            // Intersection interval
            float[] tValues = new float[]
            {
                0.0f,           // Min-value
                float.MaxValue  // Max-value
            };
            Vec3[] surfaceNormals = new Vec3[] { Vec3.Zero, Vec3.Zero };

            foreach (var plane in this.planes)
            {
                if (this.InvalidOverlap(tValues[0], tValues[1]))
                {
                    break;  // No intersection
                }

                float planeRayOrientation = Vec3.Dot(ray.Direction, plane.Normal);

                // Ray is pointing towards the back of the half-plane
                if (planeRayOrientation < 0.0f)
                {
                    float tIntersection = plane.CalcHitValue(ray);
                    if (tValues[0] < tIntersection)
                    {
                        tValues[0] = tIntersection;
                        surfaceNormals[0] = plane.Normal;
                    }
                }
                // Ray is pointing towards the front of the half-plane
                else if (planeRayOrientation > 0.0f)
                {
                    float tIntersection = plane.CalcHitValue(ray);
                    if (tValues[1] > tIntersection)
                    {
                        tValues[1] = tIntersection;
                        surfaceNormals[1] = plane.Normal;
                    }
                }
                // Ray is pointing parallel to the plane face and lies in front of the half-plane
                else if (Vec3.Dot(ray.Origin - plane.Point, plane.Normal) > 0.0f)
                {
                    break;  // Ray totally misses the box.
                }
            }

            if (this.InvalidOverlap(tValues[0], tValues[1]))
            {
                return null;
            }

            // We can get exactly 0.0f here, if so then we're inside the box!
            int minOrMax = tValues[0] == 0.0f ? 1 : 0;

            float hitDistance = tValues[minOrMax];
            Vec3 hitPoint = ray.GetPointAlong(hitDistance);
            Vec3 surfaceNormal = surfaceNormals[minOrMax];

            return new HitInfo(hitPoint, surfaceNormal, hitDistance, base.Material);
        }

        private bool InvalidOverlap(float min, float max)
        {
            return min > max;
        }

        private static Transform ParallelepipedTransform(Vec3 cornerPoint, Vec3 lengthVector, Vec3 widthVector, Vec3 heightVector)
        {
            Vec3 center = cornerPoint + 0.5f * (lengthVector + widthVector + heightVector);
            Vec3 scale = new Vec3(lengthVector.Length(), heightVector.Length(), widthVector.Length());
            // Orientation is going to be tricky. It's assuming that the box is a skewed unit box that's been rotated to the
            // orientation that the three vectors describe. See: https://stackoverflow.com/questions/13206220/3d-skew-transformation-matrix-along-one-coordinate-axis
            // For now, assume nothing and return null.
            return null;
        }
    }
}