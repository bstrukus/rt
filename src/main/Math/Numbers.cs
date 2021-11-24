/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Math
{
    public class Numbers
    {
        public const float Epsilon = 0.000001f;

        public static bool InRange(float val, float min, float max)
        {
            return min <= val && val <= max;
        }

        public static bool InRange(double val, double min, double max)
        {
            return min <= val && val <= max;
        }

        public static bool InRange(Vec2 val, float min, float max)
        {
            return InRange(val.X, min, max) && InRange(val.Y, min, max);
        }

        public static bool InRange(int val, int min, int max)
        {
            return min <= val && val <= max;
        }

        public static float Clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            return val;
        }

        public static bool AreEqual(float lhs, float rhs)
        {
            return Abs(lhs - rhs) < Epsilon;
        }

        public static float Abs(float val)
        {
            return val < 0.0f ? -val : val;
        }

        public static float Min(float lhs, float rhs)
        {
            return lhs < rhs ? lhs : rhs;
        }

        public static float Max(float lhs, float rhs)
        {
            return lhs > rhs ? lhs : rhs;
        }

        public static float Determinant(float a, float b, float c, float d)
        {
            return a * d - b * c;
        }

        public static float Sqrt(float value)
        {
            if (value == 0.0f)
            {
                return 0.0f;
            }

            Debug.Assert(value > 0.0f);

            return System.MathF.Sqrt(value);
        }
    }
}