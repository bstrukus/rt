/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Present
{
    using rt.Math;

    /// <summary>
    /// Describes the object's visual representation in the <see cref="Scene"/>
    /// </summary>
    public class Material
    {
        public Vec3 Color { get; private set; }

        public float SpecularCoefficient { get; private set; }
        public float SpecularExponent { get; private set; }

        public float IndexOfRefraction { get; private set; }

        public Vec3 TransmissionAttenuation;

        private readonly float electricPermittivity;
        private readonly float magneticPermeability;

        public Material(Vec3 color, Vec3 transmissionAttenuation,
                        float specularCoefficient, float specularExponent,
                        float electricPermittivity, float magneticPermeability)
        {
            this.Color = color;
            this.TransmissionAttenuation = transmissionAttenuation;

            this.SpecularCoefficient = specularCoefficient;
            this.SpecularExponent = specularExponent;

            this.electricPermittivity = electricPermittivity;
            this.magneticPermeability = magneticPermeability;

            this.IndexOfRefraction = Numbers.Sqrt(this.electricPermittivity * this.magneticPermeability);
        }
    }
}