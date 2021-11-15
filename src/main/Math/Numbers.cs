/*
 * #copyright_placeholder Copyright Ben Strukus
 */

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
            return (lhs - rhs) < Epsilon;
        }
    }
}