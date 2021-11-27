/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace UnitTests
{
    using rt.Math;
    using rt.Present;

    internal static class Helpers
    {
        public const float Epsilon = 0.00001f;

        public static Material DefaultMaterial = new Material(Vec3.One, 1.0f, 1.0f);

        public static Transform SimpleTransform(Vec3 pos, float scale)
        {
            return new Transform(pos, Quat.Identity, new Vec3(scale, scale, scale));
        }

        public static Material SimpleMaterial(Vec3 color)
        {
            return new Material(color, 1.0f, 1.0f);
        }
    }
}