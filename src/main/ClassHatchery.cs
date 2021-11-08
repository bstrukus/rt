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

        /// <summary>
        /// The surface that rays pass through.
        /// </summary>
        public class ProjectionPlane
        {
            private readonly Vec3 center;

            // These need to be scaled
            private readonly Vec3[] axes;

            public ProjectionPlane(Vec3 center, Vec3 horizontalAxis, Vec3 verticalAxis)
            {
                this.center = center;
                this.axes = new Vec3[] {
                    horizontalAxis.Normalized(),
                    verticalAxis.Normalized()
                };
            }

            public Vec3 GetPointOnPlane(float hValue, float vValue)
            {
                Debug.Assert(Numbers.InRange(hValue, -1.0f, 1.0f));
                Debug.Assert(Numbers.InRange(vValue, -1.0f, 1.0f));

                return this.center + (hValue * this.axes[0]) + (vValue * this.axes[1]);
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
            private readonly Vec3 eyeDirection;
            private readonly ProjectionPlane projectionPlane;

            public float AspectRatio { get; private set; }

            public Camera(Vec3 eyeDirection, ProjectionPlane plane)
            {
                this.eyeDirection = eyeDirection;
                this.projectionPlane = plane;
                this.AspectRatio = this.projectionPlane.CalculateAspectRatio();
            }

            public Ray GenerateRay(Vec2 unitCoords)
            {
                Vec3 direction = this.projectionPlane.GetPointOnPlane(unitCoords.X, unitCoords.Y) - this.eyeDirection;
                return new Ray(eyeDirection, direction);
            }
        }

        /// <summary>
        /// #todo
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
            public Vec3 Color { get; set; }
        }
    }

    /// <summary>
    /// Where rays & objects meet.
    /// </summary>
    namespace Collide
    {
        using Present;
        using System;

        /// <summary>
        /// Data returned from a collision.
        /// </summary>
        public class HitInfo
        {
            /// <summary>
            /// First point of intersection with ray.
            /// </summary>
            public Vec3 Point { get; private set; }

            /// <summary>
            /// Normal at point of intersection.
            /// </summary>
            public Vec3 Normal { get; private set; }

            /// <summary>
            /// Distance along ray of first point of intersection.
            /// </summary>
            public float Distance { get; private set; }

            /// <summary>
            /// Material information of the object hit.
            /// </summary>
            public Material Material { get; private set; }

            public HitInfo(Vec3 hitPoint, Vec3 surfaceNormal, float hitDistance, Material material)
            {
                this.Point = hitPoint;
                this.Normal = surfaceNormal;
                this.Distance = hitDistance;

                this.Material = material;
            }
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

            public Vec3 GetPointAlong(float interpolationValue)
            {
                return this.Origin + interpolationValue * this.Direction;
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

            public Vec3 Center => this.Transform.Position;

            public Sphere(Transform transform, Material material, float radius)
                : base(transform, material)
            {
                this.Radius = radius;
            }

            public override HitInfo TryIntersect(Ray ray)
            {
                // Vector from sphere origin to ray origin
                Vec3 m = ray.Origin - this.Center;

                float b = Vec3.Dot(m, ray.Direction);
                float c = Vec3.Dot(m, m) - (this.Radius * this.Radius);

                // Exit if ray's origin is outside sphere (c > 0) and
                // ray is pointing away from sphere (b > 0)
                if (c > 0.0f && b > 0.0f)
                {
                    return null;
                }

                float discr = b * b - c;

                // A negative discriminant corresponds to ray missing sphere
                if (discr < 0.0f)
                {
                    return null;
                }

                // Ray now found to intersect sphere, compute smallest t-value of intersection
                float sqrt = MathF.Sqrt(discr);
                float t = -b - sqrt;

                // If t is negative, ray started inside sphere so clamp t to zero
                // #todo Revisit this, might need to be altered to support refractions
                if (t < 0.0f)
                {
                    t = 0.0f;
                }

                Vec3 hitPoint = ray.GetPointAlong(t);

                // #todo Figure out how I want to normalize this on-demand instead of every time
                Vec3 normal = (hitPoint - this.Transform.Position).Normalized();

                //return new HitInfo(hitPoint, normal, t);
                return null;
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
                return null;
            }
        }
    }

    /// <summary>
    /// Worker classes for basic mathematical functions.
    /// </summary>
    namespace Math
    {
        public class Vec2
        {
            public float X => val[0];
            public float Y => val[1];

            public Vec2(float x, float y)
            {
                this.val[0] = x;
                this.val[1] = y;
            }

            private readonly float[] val = new float[2];
        }

        public class Quat
        {
            private float[] val = new float[4];

            public Quat(float x, float y, float z, float w)
            {
                this.val[0] = x;
                this.val[1] = y;
                this.val[2] = z;
                this.val[3] = w;
            }
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
        /// Describes an object's spatial representation in the <see cref="Scene"/>
        /// </summary>
        public class Transform
        {
            public Vec3 Position { get; private set; }

            public Quat Orientation { get; private set; }

            public Vec3 Scale { get; private set; }

            public Transform(Vec3 position, Quat orientation, Vec3 scale)
            {
                this.Position = position;
                this.Orientation = orientation;
                this.Scale = scale;
            }
        }

        /// <summary>
        /// Describes the object's visual representation in the <see cref="Scene"/>
        /// </summary>
        public class Material
        {
            public Vec3 Color { get; private set; }

            public Material(Vec3 color)
            {
                this.Color = color;
            }
        }

        /// <summary>
        /// Holds information about hittable objects, lights, and other data.
        /// </summary>
        public class Scene
        {
            public Vec3 AmbientColor { get; private set; }

            private List<IHittable> hittables;

            public Scene(List<IHittable> hittables)
            {
                this.hittables = hittables;

                // #todo Read Scene.AmbientColor in from data
                this.AmbientColor = Vec3.One;
            }

            public HitInfo Project(Ray ray)
            {
                HitInfo result = null;
                float hitDistance = float.MaxValue;
                foreach (var hittable in this.hittables)
                {
                    var hitInfo = hittable.TryIntersect(ray);
                    if (hitInfo != null && hitInfo.Distance < hitDistance)
                    {
                        result = hitInfo;
                    }
                }
                return result;
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
            private Render.Image image;
            private Present.Scene scene;

            // Jobs
            public Runner(Workload workload)
            {
                //
            }

            public Runner(Present.Scene scene, Render.Camera camera, Render.Image image)
            {
                this.scene = scene;

                this.camera = camera;
                this.image = image;
            }

            public void Execute()
            {
                // Generate rays from the camera's eye through the projection plane
                for (int y = 0; y < this.image.Height; ++y)
                {
                    for (int x = 0; x < this.image.Width; ++x)
                    {
                        var scaledPixel = this.image.InterpolatedPixel(x, y);
                        var ray = this.camera.GenerateRay(scaledPixel);
                        var hitInfo = this.scene.Project(ray);
                        if (hitInfo == null)
                        {
                            image.SetPixel(x, y, color: scene.AmbientColor);
                            continue;
                        }

                        image.SetPixel(x, y, color: hitInfo.Material.Color);
                    }
                }

                image.Save();
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