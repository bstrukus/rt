/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace UnitTests
{
    using rt.Math;
    using rt.Present;

    internal static class Helpers
    {
        public static Transform SimpleTransform(Vec3 pos, float scale)
        {
            return new Transform(pos, Quat.Identity, new Vec3(scale, scale, scale));
        }

        public static Material SimpleMaterial(Vec3 color)
        {
            return new Material(color);
        }
    }
}