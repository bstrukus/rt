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
    }
}