/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Math
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
}