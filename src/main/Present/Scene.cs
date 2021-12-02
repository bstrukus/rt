/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Collections.Generic;

namespace rt.Present
{
    using rt.Collide;
    using rt.Execute;
    using rt.Math;
    using rt.Render;

    /// <summary>
    /// Holds information about hittable objects, lights, and other data.
    /// </summary>
    public class Scene
    {
        private const float ShadowFeelerEpsilon = 0.001f;
        private const float ReflectionNudgeEpsilon = 0.001f;
        private const float TransmissionNudgeEpsilon = 0.001f;
        private const float AirTransmissionFactor = 1.0f;

        public Vec3 AmbientColor { get; private set; }

        private readonly List<IHittable> hittables;
        private readonly List<Light> lights;

        public Scene(List<IHittable> hittables, List<Light> lights, Vec3 ambientColor)
        {
            if (Levers.GetOption(Levers.Option.LimitObjects))
            {
                this.hittables = hittables.GetRange(Levers.ObjectStart, Levers.ObjectLimit);
            }
            else
            {
                this.hittables = hittables;
            }
            this.lights = lights;

            this.AmbientColor = ambientColor;
        }

        public ColorReport Trace(Ray ray, int depth)
        {
            return Trace(ray, AirTransmissionFactor, depth);
        }

        private ColorReport Trace(Ray ray, float currRefractionIndex, int depth)
        {
            //////////////////////////////////////////////////////////////////////////
            if (depth < 0)
            {
                // Max depth reached
                return new ColorReport(Vec3.Zero);
            }

            HitInfo hitInfo = this.Project(ray);
            if (hitInfo == null || Numbers.AreEqual(hitInfo.Distance, 0.0f))
            {
                // No collision occurred
                return new ColorReport(Vec3.Zero);
            }

            //////////////////////////////////////////////////////////////////////////
            // Calculate lighting at current point
            var color = CalculateLighting(ray, hitInfo);

            //////////////////////////////////////////////////////////////////////////
            // Calculate index of refraction
            float nextRefractionIndex = currRefractionIndex == AirTransmissionFactor ?
                                        hitInfo.Material.IndexOfRefraction :
                                        AirTransmissionFactor;
            // #optimize Equations within this part of the tracing process all utilize the relative refraction index, so might
            // be worth calculating it here and passing it to the Calc functions

            //////////////////////////////////////////////////////////////////////////
            // Calculate reflection & transmission coefficients
            float reflectionCoefficient = this.CalculateReflectionCoefficient(ray, hitInfo, currRefractionIndex,
                                                                                            nextRefractionIndex);
            float transmissionCoefficient = 1.0f - reflectionCoefficient;

            // Factor in energy lost to the surface
            reflectionCoefficient *= hitInfo.Material.SpecularCoefficient;
            transmissionCoefficient *= hitInfo.Material.SpecularCoefficient;

            //////////////////////////////////////////////////////////////////////////
            // Calculate reflected lighting at current point
            if (Numbers.AreNotEqual(reflectionCoefficient, 0.0f))
            {
                var reflectedRay = this.CalculateReflectedRay(ray, hitInfo);
                color += reflectionCoefficient * this.Trace(reflectedRay, currRefractionIndex, depth - 1);
            }

            //////////////////////////////////////////////////////////////////////////
            // Calculate transmitted lighting at current point
            if (Numbers.AreNotEqual(transmissionCoefficient, 0.0f))
            {
                var transmittedRay = this.CalculateRefractedRay(ray, hitInfo, currRefractionIndex, nextRefractionIndex);
                color += transmissionCoefficient * this.Trace(transmittedRay, nextRefractionIndex, depth - 1);
            }

            //////////////////////////////////////////////////////////////////////////
            return color;
        }

        private Ray CalculateReflectedRay(Ray ray, HitInfo hitInfo)
        {
            Vec3 newOrigin = hitInfo.Point + hitInfo.Normal * ReflectionNudgeEpsilon;
            Vec3 newDirection = Calc.Reflect(-ray.Direction, hitInfo.Normal).Normalized();
            return new Ray(newOrigin, newDirection);
        }

        /* #todo There might be a lot to do here to support transmission, a few thoughts
         * - I'm not sure how my collision tests support rays that originate inside of the shape, I think ellipsoid is fine but
         *   I don't think sphere supports getting any useful information
         * - I'm not sure what to do with the surface normal if the collision originates inside of the shape, invert it?
         */

        private Ray CalculateRefractedRay(Ray ray, HitInfo hitInfo, float currentRefractionIndex, float nextRefractionIndex)
        {
            // Snell's Law of Refraction
            Vec3 newOrigin = hitInfo.Point + hitInfo.Normal * TransmissionNudgeEpsilon; // #todo Might need to invert the normal
            Vec3 newDirection = Calc.Refract(-ray.Direction, hitInfo.Normal, currentRefractionIndex, nextRefractionIndex).Normalized();
            return new Ray(newOrigin, newDirection);
        }

        private float CalculateReflectionCoefficient(Ray ray, HitInfo hitInfo, float currentRefractionIndex, float nextRefractionIndex)
        {
            // Fresnel Equations
            Vec3 incidentVector = -ray.Direction;
            Vec3 normal = hitInfo.Normal;

            const float relativeMagneticPermeability = 1.0f;  // Assumption
            float relativeRefractionIndex = currentRefractionIndex / nextRefractionIndex;
            float cosThetaI = Vec3.Dot(incidentVector, normal);

            // #optimize This is the same term calculated in the refracted ray equation
            float radicand = 1.0f - (relativeRefractionIndex * relativeRefractionIndex) * (1.0f - (cosThetaI * cosThetaI));
            if (radicand >= 0.0f)
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
            float commonParallelTerm = relativeRefractionIndex * cosThetaT; ;
            float parallelCoefficient = (relativeMagneticPermeability * cosThetaI - commonParallelTerm) /
                                        (relativeMagneticPermeability * cosThetaI + commonParallelTerm);

            return 0.5f * ((perpendicularCoefficient * perpendicularCoefficient) + (parallelCoefficient * parallelCoefficient);
        }

        private float CalculateTransmissionCoefficient(HitInfo hitInfo)
        {
            // #todo #transmission
            return 0.0f;
        }

        private ColorReport CalculateLighting(Ray ray, HitInfo hitInfo)
        {
            if (!this.HasLights())
            {
                return new ColorReport(hitInfo.Material.Color);
            }

            Vec3 finalColor = Vec3.Zero;
            if (this.DebugLightCalc(hitInfo, ray, ref finalColor))
            {
                return new ColorReport(finalColor);
            }

            Vec3 objectColor = hitInfo.Material.Color;

            // Current best lighting calculation
            // Light contribution, no shadow tests yet
            foreach (var light in this.lights)
            {
                // Shadow check
                if (this.IsPathwayToLightClear(hitInfo, light))
                {
                    // #optimize Can reuse the PointToLight vector from the shadow test
                    Vec3 pointToLight = (light.Transform.Position - hitInfo.Point).Normalized();

                    // Diffuse
                    float diffuseCoefficient = Calc.DiffuseCoefficient(hitInfo.Normal, pointToLight);
                    Vec3 diffuseReflectionTerm = diffuseCoefficient * Vec3.Multiply(objectColor, light.Color);

                    // Specular
                    Vec3 reflectionVector = Calc.Reflect(pointToLight, hitInfo.Normal).Normalized();
                    Vec3 pointToEye = (ray.Origin - hitInfo.Point).Normalized();
                    float specularCoefficient = Calc.SpecularCoefficient(reflectionVector, pointToEye,
                                                                         hitInfo.Material.SpecularCoefficient,
                                                                         hitInfo.Material.SpecularExponent);
                    Vec3 specularReflectionTerm = specularCoefficient * light.Color;

                    finalColor += diffuseReflectionTerm + specularReflectionTerm;
                }
            }
            finalColor += Vec3.Multiply(objectColor, this.AmbientColor);
            return new ColorReport(finalColor);
        }

        private bool IsPathwayToLightClear(HitInfo hitInfo, Light light)
        {
            if (Levers.GetOption(Levers.Option.DisableShadows))
            {
                return true;
            }

            Vec3 origin = hitInfo.Point;
            Vec3 surfaceNormal = hitInfo.Normal;
            Vec3 direction = (light.Transform.Position - hitInfo.Point);
            float distance = direction.Length();
            direction = direction.Normalized();

            var ray = new Ray(origin + surfaceNormal * ShadowFeelerEpsilon, direction);
            HitInfo lightTrace = this.Project(ray);
            // #optimize Can test a segment against shapes, might be faster?
            return lightTrace == null || distance < lightTrace.Distance;
        }

        private bool DebugLightCalc(HitInfo hitInfo, Ray ray, ref Vec3 outputColor)
        {
            // #todo This can be written more clearly, switch statement?
            Vec3 objectColor = hitInfo.Material.Color;

            if (Levers.GetOption(Levers.Option.BooleanTest))
            {
                outputColor = objectColor;
            }
            else if (Levers.GetOption(Levers.Option.RenderNormals))
            {
                outputColor = hitInfo.Normal.Clamped(0.0f, 1.0f);
            }
            else if (Levers.GetOption(Levers.Option.ViewVectorLighting))
            {
                Vec3 pointToEye = (ray.Origin - hitInfo.Point).Normalized();
                float diffuseCoefficient = Calc.DiffuseCoefficient(hitInfo.Normal, pointToEye);

                outputColor += objectColor * diffuseCoefficient;
            }
            else
            {
                outputColor = Vec3.Zero;
                return false;
            }
            return true;
        }

        public HitInfo Project(Ray ray)
        {
            HitInfo result = null;
            float hitDistance = float.MaxValue;

            // #todo Replace basic foreach loop with a bounding-volume hierarchy
            foreach (var hittable in this.hittables)
            {
                var hitInfo = hittable.TryIntersect(ray);
                if (hitInfo != null && hitInfo.Distance < hitDistance)
                {
                    result = hitInfo;
                    hitDistance = hitInfo.Distance;
                }
            }
            return result;
        }

        private bool HasLights()
        {
            return this.lights != null & this.lights.Count > 0;
        }
    }
}