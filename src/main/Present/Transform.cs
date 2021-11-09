/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Present
{
    using Math;

    /// <summary>
    /// Describes an object's spatial representation in the <see cref="Scene"/>
    /// </summary>
    public class Transform
    {
        public Vec3 Position { get; private set; }

        public Quat Orientation { get; private set; }

        public Vec3 Scale { get; private set; }

        public Transform(Vec3 position, Quat orientation, Vec3 scale)
        {
            this.Position = position;
            this.Orientation = orientation;
            this.Scale = scale;
        }
    }
}