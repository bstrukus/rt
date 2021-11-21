/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

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
        public class Vec2
        {
            public float X => this.val[0];
            public float Y => this.val[1];

            private float this[int i]
            {
                get
                {
                    Debug.Assert(Numbers.InRange(i, 0, 1));
                    return this.val[i];
                }
            }

            public Vec2(float x, float y)
            {
                this.val[0] = x;
                this.val[1] = y;
            }

            public static Vec2 operator *(Vec2 vec, float scalar)
            {
                return new Vec2(vec[0] * scalar, vec[1] * scalar);
            }

            public static float Dot(Vec2 lhs, Vec2 rhs)
            {
                return lhs[0] * rhs[0] + lhs[1] * rhs[1];
            }

            private readonly float[] val = new float[2];
        }

        public class Mat2
        {
            private float this[int i, int j]
            {
                get
                {
                    Debug.Assert(Numbers.InRange(i, 0, 1) && Numbers.InRange(j, 0, 1));
                    return this.val[i + j * 2];
                }
            }

            public Mat2(float a00, float a01, float a10, float a11)
            {
                this.val[0] = a00;
                this.val[1] = a01;
                this.val[2] = a10;
                this.val[3] = a11;
            }

            public Vec2 Multiply(Vec2 vec)
            {
                return new Vec2(
                    this.val[0] * vec.X + this.val[1] * vec.Y,
                    this.val[2] * vec.X + this.val[3] * vec.Y
                    );
            }

            public static Mat2 FromRows(Vec2 topRow, Vec2 bottomRow)
            {
                return new Mat2(
                    topRow.X, topRow.Y,
                    bottomRow.X, bottomRow.Y);
            }

            public static Mat2 FromColumns(Vec2 leftColumn, Vec2 rightColumn)
            {
                return new Mat2(
                    leftColumn.X, rightColumn.X,
                    leftColumn.Y, rightColumn.Y);
            }

            private readonly float[] val = new float[4];
        }

        public class Mat3
        {
            private float this[int i, int j]
            {
                get
                {
                    Debug.Assert(Numbers.InRange(i, 0, 2) && Numbers.InRange(j, 0, 2));
                    return this.val[i + j * 3];
                }
            }

            public Vec3 Multiply(Vec3 vec)
            {
                Vec3 top = new Vec3(val[0], val[1], val[2]);
                Vec3 mid = new Vec3(val[3], val[4], val[5]);
                Vec3 bot = new Vec3(val[6], val[7], val[8]);
                return new Vec3(Vec3.Dot(top, vec), Vec3.Dot(mid, vec), Vec3.Dot(bot, vec));
            }

            private readonly float[] val = new float[9];
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