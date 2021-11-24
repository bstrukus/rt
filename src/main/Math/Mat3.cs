/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Math
{
    public class Mat3
    {
        // #todo Need to find a way to make this static
        public static Mat3 Identity = new Mat3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);

        public float this[int i, int j]
        {
            get
            {
                Debug.Assert(Numbers.InRange(i, 0, 2) && Numbers.InRange(j, 0, 2));

                int index = i * 3 + j;
                return this.val[index];
            }

            private set
            {
                Debug.Assert(Numbers.InRange(i, 0, 2) && Numbers.InRange(j, 0, 2));

                int index = i * 3 + j;
                this.val[index] = value;
            }
        }

        public float this[int i]
        {
            get
            {
                Debug.Assert(Numbers.InRange(i, 0, 8));
                return this.val[i];
            }

            private set
            {
                Debug.Assert(Numbers.InRange(i, 0, 8));
                this.val[i] = value;
            }
        }

        public Mat3()
        {
            // Do nothing
        }

        public Mat3(float m00, float m01, float m02,
                    float m10, float m11, float m12,
                    float m20, float m21, float m22)
        {
            this.val[0] = m00;
            this.val[1] = m01;
            this.val[2] = m02;

            this.val[3] = m10;
            this.val[4] = m11;
            this.val[5] = m12;

            this.val[6] = m20;
            this.val[7] = m21;
            this.val[8] = m22;
        }

        public static Mat3 FromRows(Vec3 topRow, Vec3 midRow, Vec3 botRow)
        {
            return new Mat3(
                topRow.X, topRow.Y, topRow.Z,
                midRow.X, midRow.Y, midRow.Z,
                botRow.X, botRow.Y, botRow.Z);
        }

        public static Mat3 FromColumns(Vec3 leftCol, Vec3 midCol, Vec3 rightCol)
        {
            return new Mat3(
                leftCol.X, midCol.X, rightCol.X,
                leftCol.Y, midCol.Y, rightCol.Y,
                leftCol.Z, midCol.Z, rightCol.Z);
        }

        public Vec3 Multiply(Vec3 vec)
        {
            Vec3 top = new Vec3(val[0], val[1], val[2]);
            Vec3 mid = new Vec3(val[3], val[4], val[5]);
            Vec3 bot = new Vec3(val[6], val[7], val[8]);
            return new Vec3(Vec3.Dot(top, vec), Vec3.Dot(mid, vec), Vec3.Dot(bot, vec));
        }

        public Mat3 Transposed()
        {
            return new Mat3(this[0, 0], this[1, 0], this[2, 0],
                            this[0, 1], this[1, 1], this[2, 1],
                            this[0, 2], this[1, 2], this[2, 2]);
        }

        public static bool operator ==(Mat3 lhs, Mat3 rhs)
        {
            for (int i = 0; i < 9; ++i)
            {
                if (!Numbers.AreEqual(lhs[i], rhs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Mat3 lhs, Mat3 rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object o)
        {
            var otherMatrix = (Mat3)o;

            return this == otherMatrix;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < 9; ++i)
            {
                hashCode += this.val[i].GetHashCode();
            }
            return hashCode;
        }

        public float Determinant()
        {
            /*
             * A = [ a b ]
             *     [ c d ]
             *
             * det(A) = ad - bc
             */
            return this.val[0] * Numbers.Determinant(this.val[4], this.val[5], this.val[7], this.val[8]) -
                   this.val[1] * Numbers.Determinant(this.val[3], this.val[5], this.val[6], this.val[8]) +
                   this.val[2] * Numbers.Determinant(this.val[3], this.val[4], this.val[6], this.val[7]);
        }

        public Mat3 Inverted()
        {
            // [M]-1 = 1 / det[M] *
            float oneOverDet = this.Determinant();
            Debug.Assert(Numbers.AreEqual(oneOverDet, 0.0f) == false);
            oneOverDet = 1.0f / oneOverDet;

            // m11 * m22 - m12 * m21 | m02 * m21 - m01 * m22 | m01 * m12 - m02 * m11
            // m12 * m20 - m10 * m22 | m00 * m22 - m02 * m20 | m02 * m10 - m00 * m12
            // m10 * m21 - m11 * m20 | m01 * m20 - m00 * m21 | m00 * m11 - m01 * m10

            Mat3 inverse = new Mat3();
            inverse[0] = (this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1]) * oneOverDet;
            inverse[1] = (this[0, 2] * this[2, 1] - this[0, 1] * this[2, 2]) * oneOverDet;
            inverse[2] = (this[0, 1] * this[1, 2] - this[0, 2] * this[1, 1]) * oneOverDet;

            inverse[3] = (this[1, 2] * this[2, 0] - this[1, 0] * this[2, 2]) * oneOverDet;
            inverse[4] = (this[0, 0] * this[2, 2] - this[0, 2] * this[2, 0]) * oneOverDet;
            inverse[5] = (this[0, 2] * this[1, 0] - this[0, 0] * this[1, 2]) * oneOverDet;

            inverse[6] = (this[1, 0] * this[2, 1] - this[1, 1] * this[2, 0]) * oneOverDet;
            inverse[7] = (this[0, 1] * this[2, 0] - this[0, 0] * this[2, 1]) * oneOverDet;
            inverse[8] = (this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0]) * oneOverDet;
            return inverse;
        }

        private readonly float[] val = new float[9];
    }
}