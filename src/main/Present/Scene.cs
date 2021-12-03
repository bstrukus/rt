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
            float reflectionCoefficient = Calc.ReflectionCoefficient(ray, hitInfo, currRefractionIndex, nextRefractionIndex);
            float transmissionCoefficient = 1.0f - reflectionCoefficient;

            // Factor in energy lost to the surface
            reflectionCoefficient *= hitInfo.Material.SpecularCoefficient;
            transmissionCoefficient *= hitInfo.Material.SpecularCoefficient;

            //////////////////////////////////////////////////////////////////////////
            // Calculate reflected lighting at current point
            if (Numbers.AreNotEqual(reflectionCoefficient, 0.0f))
            {
                var reflectedRay = Calc.ReflectedRay(ray, hitInfo);
                color += reflectionCoefficient * this.Trace(reflectedRay, currRefractionIndex, depth - 1);
            }

            //////////////////////////////////////////////////////////////////////////
            // Calculate transmitted lighting at current point
            if (Numbers.AreNotEqual(transmissionCoefficient, 0.0f))
            {
                var transmittedRay = Calc.RefractedRay(ray, hitInfo, currRefractionIndex, nextRefractionIndex);
                color += transmissionCoefficient * this.Trace(transmittedRay, nextRefractionIndex, depth - 1);
            }

            //////////////////////////////////////////////////////////////////////////
            return color;
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