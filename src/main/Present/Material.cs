/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Present
{
    using Math;

    /// <summary>
    /// Describes the object's visual representation in the <see cref="Scene"/>
    /// </summary>
    public class Material
    {
        // #todo Consider creating a standalone Color class
        public Vec3 Color { get; private set; }

        public Material(Vec3 color)
        {
            this.Color = color;
        }
    }
}