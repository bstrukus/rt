/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using rt.Math;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

/// <summary>
/// This file is used to quickly define classes & their compositions before they get their own files.
/// </summary>
namespace rt
{
    namespace Render
    {
        /// <summary>
        /// The surface that rays pass through.
        /// </summary>
        public class ProjectionPlane
        {
            private readonly Vec3 center;
            private readonly Vec3[] axes;

            public ProjectionPlane(Vec3 center, Vec3 horizontalAxis, Vec3 verticalAxis)
            {
                this.center = center;
                this.axes = new Vec3[]
                {
                horizontalAxis,
                verticalAxis
                };
            }

            public Vec3 GetPointOnPlane(float hValue, float vValue)
            {
                Debug.Assert(Utils.InRange(hValue, -1.0f, 1.0f));
                Debug.Assert(Utils.InRange(vValue, -1.0f, 1.0f));

                return this.center + (hValue * this.axes[0]) + (vValue * this.axes[1]);
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
            private Vec3 eyePosition;
            private ProjectionPlane projectionPlane;
            private int stepCount;

            public Camera(Vec3 eyePosition, ProjectionPlane plane, int stepCount)
            {
                this.eyePosition = eyePosition;
                this.projectionPlane = plane;
                this.stepCount = stepCount;
            }

            public Ray GenerateRay(int xStep, int yStep)
            {
                Debug.Assert(Utils.InRange(xStep, 0, 1));
                Debug.Assert(Utils.InRange(yStep, 0, 1));

                float hValue = this.StepToInterpolationValue(xStep);
                float vValue = this.StepToInterpolationValue(yStep);
                Vec3 direction = this.projectionPlane.GetPointOnPlane(hValue, vValue) - this.eyePosition;
                return new Ray(eyePosition, direction);
            }

            private float StepToInterpolationValue(int step)
            {
                // [0, 1] -> [-1, 1] => [0, 1] * 2 => [0, 2] - 1 => [-1, 1]
                float floatStep = (float)step;
                floatStep *= 2.0f;
                floatStep -= 1.0f;
                return floatStep;
            }
        }

        public class Image
        {
            //
        }
    }

    namespace Collide
    {
        /// <summary>
        /// Data returned from a collision.
        /// </summary>
        public class HitInfo
        {
        }

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
        }

        /// <summary>
        /// An object that a ray can intersect.
        /// </summary>
        public interface IHittable
        {
            HitInfo TryIntersect(Ray ray);
        }

        /// <summary>
        /// A basic geometric shape, intersections with rays can be considered the result of a simple
        /// mathematical formula.
        /// </summary>
        public abstract class Shape : IHittable
        {
            // Transform

            public abstract HitInfo TryIntersect(Ray ray);
        }

        /// <summary>
        /// A point and a radius, but in the case of material orientation it has that as well.
        /// </summary>
        public class Sphere : Shape
        {
            public override HitInfo TryIntersect(Ray ray)
            {
                return new HitInfo();
            }
        }
    }

    namespace Math
    {
        public class Quat
        {
            private float[] val = new float[4];
        }
    }

    namespace Present
    {
        using Render;
        using Collide;

        /// <summary>
        /// Describes an object's representation in the <see cref="Scene"/>
        /// </summary>
        public class Transform
        {
            public Vec3 Position { get; set; }

            public Quat Orientation { get; set; }

            public Vec3 Scale { get; set; }
        }

        internal class Scene
        {
            private Camera camera;
            private List<IHittable> hittables;

            public void Print()
            {
                //
            }
        }
    }
}