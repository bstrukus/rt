/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Present
{
    using rt.Collide;
    using rt.Math;
    using rt.Render;
    using System.Collections.Generic;

    /// <summary>
    /// Holds information about hittable objects, lights, and other data.
    /// </summary>
    public class Scene
    {
        public Vec3 AmbientColor { get; private set; }

        private readonly List<IHittable> hittables;
        private readonly List<Light> lights;

        public Scene(List<IHittable> hittables, List<Light> lights)
        {
            this.hittables = hittables;
            this.lights = lights;

            // #todo Read Scene.AmbientColor in from data, assume zero for now!
            this.AmbientColor = Vec3.Zero;
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

            Vec3 objectColor = hitInfo.Material.Color;
            Vec3 finalColor = Vec3.Zero;

            // Light contribution, no shadow tests yet
            foreach (var light in this.lights)
            {
                Vec3 pointToLight = (light.Transform.Position - hitInfo.Point).Normalized();
                float diffuseCoefficient = Calc.DiffuseCoefficient(hitInfo.Normal, pointToLight);
                finalColor += objectColor * diffuseCoefficient * light.Color.X;
            }

            // #todo #debug-lever Create flag to draw normals as a debugging option
            //color = hitInfo.Normal.Clamped(0.0f, 1.0f);

            return new ColorReport(finalColor);
        }

        public HitInfo Project(Ray ray)
        {
            HitInfo result = null;
            float hitDistance = float.MaxValue;

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