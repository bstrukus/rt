/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Math
{
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
}