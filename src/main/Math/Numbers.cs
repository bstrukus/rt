/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Math
{
    public class Numbers
    {
        public const float Epsilon = 0.000001f;

        static public bool InRange(float val, float min, float max)
        {
            return min <= val && val <= max;
        }

        static public bool InRange(int val, int min, int max)
        {
            return min <= val && val <= max;
        }

        static public float Clamp(float val, float min, float max)
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

        static public bool AreEqual(float lhs, float rhs)
        {
            // #todo Replace with epsilon
            return lhs == rhs;
        }
    }
}