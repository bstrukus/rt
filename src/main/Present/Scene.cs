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

        public ColorReport Trace(Ray ray)
        {
            HitInfo hitInfo = this.Project(ray);
            if (hitInfo == null)
            {
                return new ColorReport(this.AmbientColor);
            }

            if (!this.HasLights())
            {
                return new ColorReport(hitInfo.Material.Color);
            }

            return CalculateLighting(hitInfo, ray);
        }

        private ColorReport CalculateLighting(HitInfo hitInfo, Ray ray)
        {
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
                Vec3 pointToLight = (light.Transform.Position - hitInfo.Point).Normalized();
                // Detect shadows along the pointToLight vector
                // If pathway not obstructed, calculate diffuse
                if (this.IsPathwayToLightClear(hitInfo.Point, hitInfo.Normal, pointToLight))
                {
                    float diffuseCoefficient = Calc.DiffuseCoefficient(hitInfo.Normal, pointToLight);
                    finalColor += Vec3.Multiply(objectColor, light.Color) * diffuseCoefficient;
                }
                else
                {
                    // Just ambient?
                    finalColor += Vec3.Zero;
                }
            }
            return new ColorReport(finalColor);
        }

        private bool IsPathwayToLightClear(Vec3 origin, Vec3 surfaceNormal, Vec3 direction)
        {
            const float ShadowFeelerEpsilon = 0.0001f;
            var ray = new Ray(origin + surfaceNormal * ShadowFeelerEpsilon, direction);
            return this.Project(ray) == null;
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