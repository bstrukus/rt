/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using rt.Math;
using System.Collections.Generic;
using rt.Render;

/* TODO LIST
 * [ ] Lighting
 * [ ] Timer for the ray tracing
 * [ ]
 */

/// <summary>
/// This file is used to quickly define classes & their compositions before they get their own files.
/// </summary>
namespace rt
{
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

                return new HitInfo(hitPoint, normal, t, base.Material);
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
            public static Quat Identity = new Quat(0.0f, 0.0f, 0.0f, 1.0f);

            public Quat(float x, float y, float z, float w)
            {
                this.val[0] = x;
                this.val[1] = y;
                this.val[2] = z;
                this.val[3] = w;
            }

            private readonly float[] val = new float[4];
        }
    }

    /// <summary>
    /// How objects are represented in the "world".
    /// </summary>
    namespace Present
    {
        using Collide;
        using rt.Render;

        public abstract class Light
        {
            public Transform Transform { get; private set; }
            public Vec3 Color { get; private set; }

            public Light(Transform transform, Vec3 color)
            {
                this.Transform = transform;
                this.Color = color;
            }
        }

        public class PointLight : Light
        {
            public PointLight(Transform transform, Vec3 color)
                : base(transform, color)
            {
                //
            }
        }

        /// <summary>
        /// Holds information about hittable objects, lights, and other data.
        /// </summary>
        public class Scene
        {
            public Vec3 AmbientColor { get; private set; }

            private List<IHittable> hittables;
            private List<Light> lights;

            public Scene(List<IHittable> hittables, List<Light> lights)
            {
                this.hittables = hittables;
                this.lights = lights;

                // #todo Read Scene.AmbientColor in from data
                this.AmbientColor = Vec3.One;
            }

            public ColorReport Trace(Ray ray)
            {
                HitInfo hitInfo = this.Project(ray);
                if (hitInfo == null)
                {
                    return new ColorReport(this.AmbientColor);
                }

                // #todo Unit tests for diffuse lighting calculations
                Vec3 color = hitInfo.Material.Color;
                foreach (var light in this.lights)
                {
                    Vec3 pointToLight = (light.Transform.Position - hitInfo.Point).Normalized();
                    float diffuseCoefficient = Vec3.Dot(hitInfo.Normal, pointToLight);
                    diffuseCoefficient = diffuseCoefficient > 0.0f ? 1.0f : 0.0f;
                    {
                        color *= diffuseCoefficient;
                    }
                }

                // #todo Create flag to draw normals as a debugging option
                color = hitInfo.Normal.Clamped(0.0f, 1.0f);

                return new ColorReport(color);
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
        using rt.Utility;

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
                        //var hitInfo = this.scene.Project(ray);

                        var colorReport = this.scene.Trace(ray);
                        image.SetPixel(x, y, color: colorReport.Color);
                    }
                }

                image.Save();
            }
        }
    }
}