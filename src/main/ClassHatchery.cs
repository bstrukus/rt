/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using rt.Math;
using System.Collections.Generic;

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
    /// Worker classes for basic mathematical functions.
    /// </summary>
    namespace Math
    {
        public static class Vec
        {
            public static float Dot(Vec2 lhs, Vec2 rhs)
            {
                return lhs.X * rhs.X + lhs.Y * rhs.Y;
            }
        }

        public class Vec2
        {
            public float X => val[0];
            public float Y => val[1];

            public Vec2(float x, float y)
            {
                this.val[0] = x;
                this.val[1] = y;
            }

            public static Vec2 operator *(Vec2 vec, float scalar)
            {
                return new Vec2(vec.X * scalar, vec.Y * scalar);
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

        public class Mat2
        {
            private readonly float[] m2;

            public Mat2(float a, float b, float c, float d)
            {
                m2 = new float[]
                {
                    a, b,
                    c, d
                };
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
    }
}