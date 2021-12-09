/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Render
{
    using rt.Math;
    using rt.Collide;

    /// <summary>
    /// Point of view that observes the scene.
    /// </summary>
    /// <remarks>
    /// Conversion point between "step count" and actual interpolation values.
    /// </remarks>
    public class Camera
    {
        public float AspectRatio { get; private set; }

        private readonly Vec3 eyePosition;
        private readonly Vec3 eyeDirection;
        private readonly ProjectionPlane projectionPlane;

        public Camera(Vec3 eyeDirection, ProjectionPlane plane)
        {
            this.eyeDirection = eyeDirection;
            this.projectionPlane = plane;
            this.eyePosition = this.projectionPlane.Center + this.eyeDirection;
            this.AspectRatio = this.projectionPlane.CalculateAspectRatio();
        }

        public Ray GenerateRay(Vec2 unitCoords)
        {
            Vec3 direction = this.projectionPlane.GetPointOnPlane(unitCoords.X, unitCoords.Y) - this.eyeDirection;
            return new Ray(this.eyePosition, direction);
        }
    }
}