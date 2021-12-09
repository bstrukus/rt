/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Render
{
    using rt.Math;

    public class ColorReport
    {
        public Vec3 Color { get; private set; }

        public ColorReport(Vec3 color)
        {
            this.Color = color.Clamped(0.0f, 1.0f);
        }

        public static ColorReport Black()
        {
            return new ColorReport(Vec3.Zero);
        }

        public static ColorReport operator *(float scalar, ColorReport colorReport)
        {
            return new ColorReport(colorReport.Color * scalar);
        }

        public static ColorReport operator *(ColorReport colorReport, float scalar)
        {
            return new ColorReport(colorReport.Color * scalar);
        }

        public static ColorReport operator +(ColorReport lhs, ColorReport rhs)
        {
            return new ColorReport(lhs.Color + rhs.Color);
        }

        public void Scale(Vec3 color)
        {
            this.Color = Vec3.Multiply(this.Color, color);
        }
    }
}