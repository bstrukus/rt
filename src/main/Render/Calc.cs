/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Render
{
    using rt.Math;

    /// <summary>
    /// Contains common calculations used in ray tracing, extracted to a singular class for testability.
    /// </summary>
    public static class Calc
    {
        public const float ReflectionNudgeEpsilon = 1.0e-4f;
        public const float TransmissionNudgeEpsilon = 1.0e-4f;

        public static float DiffuseCoefficient(Vec3 normal, Vec3 lightVector)
        {
            Debug.Assert(normal.IsNormalized());
            Debug.Assert(lightVector.IsNormalized());

            return Numbers.Clamp(Vec3.Dot(normal, lightVector), 0.0f, 1.0f);
        }

        public static float SpecularCoefficient(Vec3 reflectionVector, Vec3 viewVector, float specBase, float specExponent)
        {
            float relativeViewFactor = Numbers.Clamp(Vec3.Dot(reflectionVector, viewVector), 0.0f, 1.0f);
            return specBase * Numbers.Pow(relativeViewFactor, specExponent);
        }

        public static Vec3 Reflect(Vec3 incidentVector, Vec3 normal)
        {
            // This simply reflects a vector about a normal, any normalization needs should be handled by the calling function
            return 2.0f * Vec3.Dot(incidentVector, normal) * normal - incidentVector;
        }

        public static Vec3 Refract(Vec3 incidentVector, Vec3 normal, float currRefractionIndex, float nextRefractionIndex)
        {
            float relativeRefractionIndex = currRefractionIndex / nextRefractionIndex;
            float iDotN = Vec3.Dot(incidentVector, normal);

            float cosThetaT = Numbers.Sqrt(1.0f - Numbers.Squared(relativeRefractionIndex) * (1.0f - Numbers.Squared(iDotN)));
            if (iDotN >= 0.0f)
            {
                cosThetaT *= -1.0f;
            }

            return (cosThetaT + relativeRefractionIndex * iDotN) * normal - relativeRefractionIndex * incidentVector;
        }

        public static Ray ReflectedRay(Ray ray, HitInfo hitInfo)
        {
            Vec3 newOrigin = hitInfo.Point + hitInfo.Normal * ReflectionNudgeEpsilon;
            Vec3 newDirection = Calc.Reflect(-ray.Direction, hitInfo.Normal).Normalized();
            return new Ray(newOrigin, newDirection);
        }

        public static Ray RefractedRay(Ray ray, HitInfo hitInfo, float currentRefractionIndex, float nextRefractionIndex)
        {
            // Snell's Law of Refraction
            float nudgeDirection = -1.0f;// currentRefractionIndex == 1.0f ? -1.0f : 1.0f;
            Vec3 newOrigin = hitInfo.Point + hitInfo.Normal * TransmissionNudgeEpsilon * nudgeDirection;
            Vec3 newDirection = Calc.Refract(-ray.Direction, hitInfo.Normal, currentRefractionIndex, nextRefractionIndex).Normalized();
            return new Ray(newOrigin, newDirection);
        }

        public static float ReflectionCoefficient(Ray ray, HitInfo hitInfo, float currentRefractionIndex, float nextRefractionIndex)
        {
            // Fresnel Equations
            Vec3 incidentVector = -ray.Direction;
            Vec3 normal = hitInfo.Normal;

            const float relativeMagneticPermeability = 1.0f;  // Assumption
            float relativeRefractionIndex = currentRefractionIndex / nextRefractionIndex;
            float cosThetaI = Vec3.Dot(incidentVector, normal);

            // #optimize This is the same term calculated in the refracted ray equation
            float radicand = 1.0f - Numbers.Squared(relativeRefractionIndex) * (1.0f - Numbers.Squared(cosThetaI));
            if (radicand < 0.0f)
            {
                // Handle total internal reflection
                return 1.0f;
            }
            float cosThetaT = Numbers.Sqrt(radicand);

            // Perpendicular ratio
            float commonPerpendicularTerm = relativeRefractionIndex * cosThetaI;
            float perpendicularCoefficient = (commonPerpendicularTerm - relativeMagneticPermeability * cosThetaT) /
                                             (commonPerpendicularTerm + relativeMagneticPermeability * cosThetaT);

            // Parallel ratio
            float commonParallelTerm = relativeRefractionIndex * cosThetaT;
            float parallelCoefficient = (relativeMagneticPermeability * cosThetaI - commonParallelTerm) /
                                        (relativeMagneticPermeability * cosThetaI + commonParallelTerm);

            return 0.5f * Numbers.Squared(perpendicularCoefficient) + Numbers.Squared(parallelCoefficient);
        }

        public static Vec3 DistanceScaledAttenuation(Vec3 attenuationFactor, float distance)
        {
            return new Vec3(Numbers.Pow(attenuationFactor.X, distance),
                            Numbers.Pow(attenuationFactor.Y, distance),
                            Numbers.Pow(attenuationFactor.Z, distance));
        }
    }
}