/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Render
{
    using rt.Math;

    /// <summary>
    /// The surface that rays pass through.
    /// </summary>
    public class ProjectionPlane
    {
        public Vec3 Center { get; private set; }

        // These need to be scaled
        private readonly Vec3[] axes;

        public ProjectionPlane(Vec3 center, Vec3 horizontalAxis, Vec3 verticalAxis)
        {
            this.Center = center;
            this.axes = new Vec3[] {
                    horizontalAxis,
                    verticalAxis
                };
        }

        public Vec3 GetPointOnPlane(float hValue, float vValue)
        {
            Debug.Assert(Numbers.InRange(hValue, -1.0f, 1.0f));
            Debug.Assert(Numbers.InRange(vValue, -1.0f, 1.0f));

            return (hValue * this.axes[0]) + (vValue * this.axes[1]);
        }

        public float CalculateAspectRatio()
        {
            return this.axes[0].Length() / this.axes[1].Length();
        }
    }
}