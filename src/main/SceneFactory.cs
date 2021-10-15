/*
 * Copyright Ben Strukus
 */

using JsonObjects;
using System;
using System.Diagnostics;
using rt.Math;
using System.Collections.Generic;

namespace main
{
    public class Camera
    {
        //
    }

    public class Image
    {
        //
    }

    public class HitInfo
    {
    }

    public class Ray
    {
    }

    /// <summary>
    /// An object that a ray can intersect.
    /// </summary>
    public interface IHittable
    {
        HitInfo TryIntersect(Ray ray);
    }

    public class Quat
    {
        private float[] val = new float[4];
    }

    public class Transform
    {
        public Vec3 Position { get; set; }

        public Quat Orientation { get; set; }

        public Vec3 Scale { get; set; }
    }

    /// <summary>
    /// A basic geometric shape, intersections with rays can be considered the result of a simple
    /// mathematical formula.
    /// </summary>
    public abstract class Shape : IHittable
    {
        // Transform

        public abstract HitInfo TryIntersect(Ray ray);
    }

    /// <summary>
    /// A point and a radius, but in the case of material orientation it has that as well.
    /// </summary>
    public class Sphere : Shape
    {
        public override HitInfo TryIntersect(Ray ray)
        {
            return new HitInfo();
        }
    }

    internal class Scene
    {
        private Camera camera;
        private List<IHittable> hittables;

        public void Print()
        {
            //
        }
    }

    internal class SceneFactory
    {
        public SceneFactory()
        {
            //Singleton stuff
        }

        public Scene CreateScene(JsonObjects.SceneData sceneData)
        {
            return new Scene();
        }
    }
}