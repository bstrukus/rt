/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Collide
{
    using rt.Math;
    using rt.Present;

    public class Box : Shape
    {
        // #todo Need to store planes, regardless of input

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
            Vec3 backNormal = -frontNormal;

            Vec3 leftNormal = Vec3.Cross(heightVector, widthVector).Normalized();
            Vec3 rightNormal = -leftNormal;

            Vec3 bottomNormal = Vec3.Cross(widthVector, lengthVector).Normalized();
            Vec3 topNormal = -bottomNormal;
        }

        public override HitInfo TryIntersect(Ray ray)
        {
            return null;
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