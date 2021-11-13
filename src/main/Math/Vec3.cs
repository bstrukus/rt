/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Math
{
    /// <summary>
    /// 3D vector class used for linear algebra operations.
    /// </summary>
    /// <remarks>
    /// I could have used the .NET implementation of Vector3 in System.Numerics, but since this
    /// project is a learning exercise I've decided to roll my own.
    /// </remarks>
    public class Vec3
    {
        public static Vec3 Zero = new Vec3(0.0f, 0.0f, 0.0f);
        public static Vec3 One = new Vec3(1.0f, 1.0f, 1.0f);
        public static Vec3 AxisX = new Vec3(1.0f, 0.0f, 0.0f);
        public static Vec3 AxisY = new Vec3(0.0f, 1.0f, 0.0f);
        public static Vec3 AxisZ = new Vec3(0.0f, 0.0f, 1.0f);

        public float X => val[0];
        public float Y => val[1];
        public float Z => val[2];

        public Vec3(float[] values)
        {
            Debug.Assert(values.Length == 3, "Incorrect number of elements for Vec3");

            this.val = values;
        }

        public Vec3(float x, float y, float z)
        {
            this.val[0] = x;
            this.val[1] = y;
            this.val[2] = z;
        }

        public float Length()
        {
            float squaredLength = Dot(this, this);

            Debug.Assert(squaredLength > Numbers.Epsilon, "Vec3 - Can't get length of zero vector");

            return (float)System.Math.Sqrt(squaredLength);
        }

        public Vec3 Normalized()
        {
            return this / this.Length();
        }

        public Vec3 Clamped(float min, float max)
        {
            return new Vec3(
                Numbers.Clamp(this.X, min, max),
                Numbers.Clamp(this.Y, min, max),
                Numbers.Clamp(this.Z, min, max));
        }

        public bool IsNormalized()
        {
            return Numbers.AreEqual(Dot(this, this), 1.0f);            
        }

        #region Operators

        static public Vec3 operator -(Vec3 vec)
        {
            return new Vec3(-vec.X, -vec.Y, -vec.Z);
        }

        static public Vec3 operator *(Vec3 vec, float scalar)
        {
            return new Vec3(
                vec.val[0] * scalar,
                vec.val[1] * scalar,
                vec.val[2] * scalar);
        }

        static public Vec3 operator *(float scalar, Vec3 vec)
        {
            return new Vec3(
                vec.val[0] * scalar,
                vec.val[1] * scalar,
                vec.val[2] * scalar);
        }

        static public Vec3 operator /(Vec3 vec, float scalar)
        {
            Debug.Assert(scalar != 0.0f, "Vec3 - Scalar value cannot be zero when dividing");
            return new Vec3(
                vec.val[0] / scalar,
                vec.val[1] / scalar,
                vec.val[2] / scalar);
        }

        static public Vec3 operator +(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(
                lhs.val[0] + rhs.val[0],
                lhs.val[1] + rhs.val[1],
                lhs.val[2] + rhs.val[2]);
        }

        static public Vec3 operator -(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(
                lhs.val[0] - rhs.val[0],
                lhs.val[1] - rhs.val[1],
                lhs.val[2] - rhs.val[2]);
        }

        static public bool operator ==(Vec3 lhs, Vec3 rhs)
        {
            return Numbers.AreEqual(lhs.X, rhs.X) &&
                   Numbers.AreEqual(lhs.Y, rhs.Y) &&
                   Numbers.AreEqual(lhs.Z, rhs.Z);
        }

        static public bool operator !=(Vec3 lhs, Vec3 rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object o)
        {
            var otherVector = (Vec3)o;

            return this == otherVector;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        #endregion Operators

        static public float Dot(Vec3 lhs, Vec3 rhs)
        {
            return lhs.val[0] * rhs.val[0] +
                   lhs.val[1] * rhs.val[1] +
                   lhs.val[2] * rhs.val[2];
        }

        static public Vec3 Cross(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(
                lhs.Y * rhs.Z - lhs.Z * rhs.Y,
                lhs.Z * rhs.X - lhs.X * rhs.Z,
                lhs.X * rhs.Y - lhs.Y * rhs.X);
        }

        private readonly float[] val = new float[3];
    }
}