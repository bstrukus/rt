/*
 * #copyright_placeholder Copyright Ben Strukus
 */

/// <summary>
/// Information specific to setting up the render.
/// </summary>
namespace rt.Render
{
    using Collide;
    using Math;
    using rt.Utility;
    using System.Diagnostics;

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

    /// <summary>
    /// Represents the final output image and all its properties.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Width of image, in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of image, in pixels.
        /// </summary>
        public int Height { get; private set; }

        private readonly string fileName;
        private PixelBuffer buffer;

        public Image(int width, int height, string fileName)
        {
            this.Width = width;
            this.Height = height;
            this.fileName = fileName;

            this.buffer = new PixelBuffer(this.Width, this.Height);
        }

        public void SetPixel(int x, int y, Vec3 color)
        {
            Debug.Assert(Numbers.InRange(x, 0, this.Width));
            Debug.Assert(Numbers.InRange(y, 0, this.Height));

            this.buffer.SetPixel(x, y, color);
        }

        public void Save()
        {
            this.buffer.Save(this.fileName);
        }

        public void Open()
        {
            string outputFile = Dir.GetOutputFilePath(this.fileName);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo(outputFile)
                {
                    UseShellExecute = true
                }
            };
            p.Start();
        }

        public Vec2 InterpolatedPixel(int x, int y)
        {
            return new Vec2(this.InterpolatedStep(x, this.Width), this.InterpolatedStep(y, this.Height));
        }

        private float InterpolatedStep(int step, int stepCount)
        {
            // [0, 1] -> [-1, 1] => [0, 1] * 2 => [0, 2] - 1 => [-1, 1]
            float floatStep = (float)step / (float)stepCount;
            floatStep *= 2.0f;
            floatStep -= 1.0f;
            return floatStep;
        }
    }

    public class ColorReport
    {
        public Vec3 Color { get; private set; }

        public ColorReport(Vec3 color)
        {
            this.Color = color.Clamped(0.0f, 1.0f);
        }
    }

    public static class Calc
    {
        public static float DiffuseCoefficient(Vec3 normal, Vec3 lightVector)
        {
            Debug.Assert(normal.IsNormalized());
            Debug.Assert(lightVector.IsNormalized());

            return Numbers.Clamp(Vec3.Dot(normal, lightVector), 0.0f, 1.0f);
        }

        public static float SpecularCoefficient(Vec3 reflectionVector, Vec3 viewVector, float specBase, float specExponent)
        {
            float relativeViewFactor = Numbers.Clamp(Vec3.Dot(reflectionVector, viewVector), 0.0f, 1.0f);
            return specBase * Numbers.Pow(relativeViewFactor, specExponent);
        }

        public static Vec3 Reflect(Vec3 incidentVector, Vec3 normal)
        {
            // #qué Should this be normalized?
            return 2.0f * Vec3.Dot(incidentVector, normal) * normal - incidentVector;
        }

        public static Vec3 Refract()
        {
            return Vec3.Zero;
        }
    }
}