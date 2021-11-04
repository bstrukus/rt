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
    /// <summary>
    /// Information specific to setting up the render.
    /// </summary>
    namespace Render
    {
        using Collide;

        public enum Axis : int
        {
            Horizontal = 0,
            Vertical
        }

        /// <summary>
        /// The surface that rays pass through.
        /// </summary>
        public class ProjectionPlane
        {
            public float HorizontalScale => this.axes[(int)Axis.Horizontal].Length();
            public float VerticalScale => this.axes[(int)Axis.Vertical].Length();

            private readonly Vec3 center;
            private readonly Vec3[] axes;

            public ProjectionPlane(Vec3 center, Vec3 horizontalAxis, Vec3 verticalAxis)
            {
                this.center = center;
                this.axes = new Vec3[] {
                    horizontalAxis,
                    verticalAxis
                };
            }

            public Vec3 GetPointOnPlane(float hValue, float vValue)
            {
                Debug.Assert(Numbers.InRange(hValue, -1.0f, 1.0f));
                Debug.Assert(Numbers.InRange(vValue, -1.0f, 1.0f));

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
            public int HorizontalStepCount => this.stepCount[(int)Axis.Horizontal];
            public int VerticalStepCount => this.stepCount[(int)Axis.Vertical];

            private readonly Vec3 eyePosition;
            private readonly ProjectionPlane projectionPlane;
            private readonly int[] stepCount;

            public Camera(Vec3 eyePosition, ProjectionPlane plane, int stepCount)
            {
                this.eyePosition = eyePosition;
                this.projectionPlane = plane;
                this.stepCount = new int[] {
                    stepCount * (int)plane.HorizontalScale,
                    stepCount * (int)plane.VerticalScale
                };
            }

            public Ray GenerateRay(int xStep, int yStep)
            {
                Debug.Assert(Numbers.InRange(xStep, 0, this.HorizontalStepCount));
                Debug.Assert(Numbers.InRange(yStep, 0, this.VerticalStepCount));

                float hValue = this.StepToInterpolationValue(xStep, this.HorizontalStepCount);
                float vValue = this.StepToInterpolationValue(yStep, this.VerticalStepCount);
                Vec3 direction = this.projectionPlane.GetPointOnPlane(hValue, vValue) - this.eyePosition;
                return new Ray(eyePosition, direction);
            }

            private float StepToInterpolationValue(int step, int stepCount)
            {
                // [0, 1] -> [-1, 1] => [0, 1] * 2 => [0, 2] - 1 => [-1, 1]
                float floatStep = (float)step / (float)stepCount;
                floatStep *= 2.0f;
                floatStep -= 1.0f;
                return floatStep;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public class Image
        {
            public int StepFactor { get; private set; }

            private string fileFormat;
            private PixelBuffer buffer;

            public Image(int stepFactor, string fileFormat)
            {
                this.StepFactor = stepFactor;
                this.fileFormat = fileFormat;
                //this.buffer = new PixelBuffer()
            }
        }
    }

    /// <summary>
    /// Where rays & objects meet.
    /// </summary>
    namespace Collide
    {
        using Present;

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
            public Transform Transform { get; private set; }
            public Material Material { get; private set; }

            public Shape(Transform transform, Material material)
            {
                this.Transform = transform;
                this.Material = material;
            }

            public abstract HitInfo TryIntersect(Ray ray);
        }

        /// <summary>
        /// A point and a radius, but in the case of material orientation it has that as well.
        /// </summary>
        public class Sphere : Shape
        {
            // A unit sphere has a diameter of 1, so a radius of 1/2
            public float Radius { get; private set; }

            public Sphere(Transform transform, Material material, float radius)
                : base(transform, material)
            {
                this.Radius = radius;
            }

            public override HitInfo TryIntersect(Ray ray)
            {
                return new HitInfo();
            }
        }

        public class Box : Shape
        {
            public Vec3 HalfExtents => base.Transform.Scale * 0.5f;

            public Box(Transform transform, Material material)
                : base(transform, material)
            {
            }

            public override HitInfo TryIntersect(Ray ray)
            {
                return new HitInfo();
            }
        }
    }

    /// <summary>
    /// Worker classes for basic mathematical functions.
    /// </summary>
    namespace Math
    {
        public class Quat
        {
            private float[] val = new float[4];
        }
    }

    /// <summary>
    /// How objects are represented in the "world".
    /// </summary>
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

        public class Material
        {
            //
        }

        /// <summary>
        /// Holds information about hittable objects, lights, and other data.
        /// </summary>
        public class Scene
        {
            private List<IHittable> hittables;

            public Scene(List<IHittable> hittables)
            {
                this.hittables = hittables;
            }

            public HitInfo Project(Ray ray)
            {
                return new HitInfo();
            }
        }
    }

    /// <summary>
    /// Handles the execution of the ray tracing, including how the work gets distributed.
    /// </summary>
    namespace Execute
    {
        using Present;

        public class Job
        {
            // Collection of Rays
            // Scene reference
            // Collection of HitInfo?
            // Collection of RayResults (pixel values)
        }

        public class JobManager
        {
            // Breaks a Scene down into Jobs
            public JobManager(/* Some config to control job creation */)
            {
                // Maybe it knows about the capabilities, and creates
                // the workload based on what's available + specified
                // - GPU
                // - Other machines
                // - Threads
            }

            public Workload CreateJobs(Scene scene)
            {
                return new Workload();
            }
        }

        public class Workload
        {
            // Set of Jobs
            public Workload(/* Set of jobs? */)
            {
            }
        }

        public class Runner
        {
            private Render.Camera camera;
            private Present.Scene scene;

            // Jobs
            public Runner(Workload workload)
            {
                //
            }

            public Runner(Render.Camera camera, Present.Scene scene)
            {
                this.camera = camera;
                this.scene = scene;
            }

            public void Execute()
            {
                Render.PixelBuffer buffer = new Render.PixelBuffer(
                    this.camera.HorizontalStepCount,
                    this.camera.VerticalStepCount
                    );
                // Generate rays from the camera's eye through the projection plane
                for (int y = 0; y < this.camera.VerticalStepCount; ++y)
                {
                    for (int x = 0; x < this.camera.HorizontalStepCount; ++x)
                    {
                        var ray = this.camera.GenerateRay(x, y);
                        var hitInfo = this.scene.Project(ray);
                        if (hitInfo == null)
                        {
                            // Nothing
                            continue;
                        }

                        buffer.SetPixel(x, y, color: Math.Vec3.AxisX);
                    }
                }

                buffer.Save("test.bmp");
            }
        }
    }

    namespace Utility
    {
        using System;

        internal static class Log
        {
            static public void Info(string info)
            {
                Console.WriteLine(info);
            }
        }
    }
}