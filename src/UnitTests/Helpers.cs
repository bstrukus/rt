/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace UnitTests
{
    using rt.Collide.Shapes;
    using rt.Math;
    using rt.Present;

    internal static class Helpers
    {
        public const float Epsilon = 0.00001f;

        public static Material DefaultMaterial = new Material(Vec3.One, 1.0f, 1.0f, 1.0f, 1.0f);

        public static Transform SimpleTransform(Vec3 pos, float scale)
        {
            return new Transform(pos, Quat.Identity, new Vec3(scale, scale, scale));
        }

        public static Material SimpleMaterial(Vec3 color)
        {
            return new Material(color, 1.0f, 1.0f, 1.0f, 1.0f);
        }

        public static Box OriginCube(float size)
        {
            float halfSize = size / 2.0f;
            return new Box(
                cornerPoint: new Vec3(-halfSize, -halfSize, halfSize),
                lengthVector: Vec3.AxisX * size,
                widthVector: -Vec3.AxisZ * size,
                heightVector: Vec3.AxisY * size,
                material: Helpers.SimpleMaterial(Vec3.One));
        }
    }
}