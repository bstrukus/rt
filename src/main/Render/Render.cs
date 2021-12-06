/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

/// <summary>
/// Information specific to setting up the render.
/// </summary>
namespace rt.Render
{
    using rt.Collide;
    using rt.Math;
    using rt.Utility;

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

        /// <summary>
        /// Value specifying the number of recursive ray casts allowed during scene tracing.
        /// </summary>
        public int RenderDepth { get; private set; }

        private readonly string fileName;
        private PixelBuffer buffer;
        private int pixelCount;

        public Image(int width, int height, string fileName, int renderDepth)
        {
            this.Width = width;
            this.Height = height;
            this.fileName = fileName;
            this.RenderDepth = renderDepth;
            this.pixelCount = this.Width * this.Height;

            this.buffer = new PixelBuffer(this.Width, this.Height);
        }

        public void SetPixel(int x, int y, Vec3 color)
        {
            Debug.Assert(Numbers.InRange(x, 0, this.Width));
            Debug.Assert(Numbers.InRange(y, 0, this.Height));

            this.buffer.SetPixel(x, y, color);
            this.PrintProgress(x, y);
        }

        private void PrintProgress(int x, int y)
        {
            int currPixel = 1 + x + (y * this.Width);
            float percentage = 100.0f * (float)currPixel / (float)this.pixelCount;
            string progress = $"PROGRESS: {percentage.ToString("0.00")}%";
            Log.WriteInPlace(progress);
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

        public static ColorReport Black()
        {
            return new ColorReport(Vec3.Zero);
        }

        public static ColorReport operator *(float scalar, ColorReport colorReport)
        {
            return new ColorReport(colorReport.Color * scalar);
        }

        public static ColorReport operator *(ColorReport colorReport, float scalar)
        {
            return new ColorReport(colorReport.Color * scalar);
        }

        public static ColorReport operator +(ColorReport lhs, ColorReport rhs)
        {
            return new ColorReport(lhs.Color + rhs.Color);
        }
    }

    /// <summary>
    /// Contains common calculations used in ray tracing, extracted to a singular class for testability.
    /// </summary>
    public static class Calc
    {
        public const float ReflectionNudgeEpsilon = 1.0e-4f;
        public const float TransmissionNudgeEpsilon = 1.0e-4f;

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
            // This simply reflects a vector about a normal, any normalization needs should be handled by the calling function
            return 2.0f * Vec3.Dot(incidentVector, normal) * normal - incidentVector;
        }

        public static Vec3 Refract(Vec3 incidentVector, Vec3 normal, float currRefractionIndex, float nextRefractionIndex)
        {
            float relativeRefractionIndex = currRefractionIndex / nextRefractionIndex;
            float iDotN = Vec3.Dot(incidentVector, normal);

            float cosThetaT = Numbers.Sqrt(1.0f - Numbers.Squared(relativeRefractionIndex) * (1.0f - Numbers.Squared(iDotN)));
            if (iDotN >= 0.0f)
            {
                cosThetaT *= -1.0f;
            }

            return (cosThetaT + relativeRefractionIndex * iDotN) * normal - relativeRefractionIndex * incidentVector;
        }

        public static Ray ReflectedRay(Ray ray, HitInfo hitInfo)
        {
            Vec3 newOrigin = hitInfo.Point + hitInfo.Normal * ReflectionNudgeEpsilon;
            Vec3 newDirection = Calc.Reflect(-ray.Direction, hitInfo.Normal).Normalized();
            return new Ray(newOrigin, newDirection);
        }

        public static Ray RefractedRay(Ray ray, HitInfo hitInfo, float currentRefractionIndex, float nextRefractionIndex)
        {
            // Snell's Law of Refraction
            float nudgeDirection = -1.0f;// currentRefractionIndex == 1.0f ? -1.0f : 1.0f;
            Vec3 newOrigin = hitInfo.Point + hitInfo.Normal * TransmissionNudgeEpsilon * nudgeDirection;
            Vec3 newDirection = Calc.Refract(-ray.Direction, hitInfo.Normal, currentRefractionIndex, nextRefractionIndex).Normalized();
            return new Ray(newOrigin, newDirection);
        }

        public static float ReflectionCoefficient(Ray ray, HitInfo hitInfo, float currentRefractionIndex, float nextRefractionIndex)
        {
            // Fresnel Equations
            Vec3 incidentVector = -ray.Direction;
            Vec3 normal = hitInfo.Normal;

            const float relativeMagneticPermeability = 1.0f;  // Assumption
            float relativeRefractionIndex = currentRefractionIndex / nextRefractionIndex;
            float cosThetaI = Vec3.Dot(incidentVector, normal);

            // #optimize This is the same term calculated in the refracted ray equation
            float radicand = 1.0f - Numbers.Squared(relativeRefractionIndex) * (1.0f - Numbers.Squared(cosThetaI));
            if (radicand < 0.0f)
            {
                // Handle total internal reflection
                return 1.0f;
            }
            float cosThetaT = Numbers.Sqrt(radicand);

            // Perpendicular ratio
            float commonPerpendicularTerm = relativeRefractionIndex * cosThetaI;
            float perpendicularCoefficient = (commonPerpendicularTerm - relativeMagneticPermeability * cosThetaT) /
                                             (commonPerpendicularTerm + relativeMagneticPermeability * cosThetaT);

            // Parallel ratio
            float commonParallelTerm = relativeRefractionIndex * cosThetaT;
            float parallelCoefficient = (relativeMagneticPermeability * cosThetaI - commonParallelTerm) /
                                        (relativeMagneticPermeability * cosThetaI + commonParallelTerm);

            return 0.5f * Numbers.Squared(perpendicularCoefficient) + Numbers.Squared(parallelCoefficient);
        }
    }
}