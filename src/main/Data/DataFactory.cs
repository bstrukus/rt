/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Data
{
    using rt.Collide;
    using rt.Math;
    using rt.Present;
    using rt.Render;
    using rt.Utility;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Consumes a data file, outputs components of the ray tracer
    /// </summary>
    public class DataFactory
    {
        private SceneData sceneData;

        public bool Load(string filename)
        {
            using StreamReader file = File.OpenText(filename);
            string fileContents = file.ReadToEnd();
            //Log.Info($"FILE", fileContents}\r\n");

            this.sceneData = Newtonsoft.Json.JsonConvert.DeserializeObject<SceneData>(fileContents);
            if (this.sceneData == null || !this.sceneData.IsValid())
            {
                Log.Error("Error loading Scene");
                return false;
            }

            // #todo Make this togglable, debug option
            this.sceneData.PrintData(0);
            return true;
        }

        public Scene CreateScene()
        {
            //////////////////////////////////////////////////////////////////////////
            var shapeCollection = this.sceneData.Shapes;

            // Go through the list of Spheres & Boxes to create a list of IHittables
            List<IHittable> hittables = new List<IHittable>();
            if (shapeCollection.HasSpheres() && false)
            {
                foreach (var sphereData in shapeCollection.Spheres)
                {
                    hittables.Add(new Sphere(
                        CreateTransform(sphereData.Transform),
                        CreateMaterial(sphereData.Material),
                        (float)sphereData.Radius
                        ));
                }
            }

            if (shapeCollection.HasBoxes())
            {
                foreach (var boxData in shapeCollection.Boxes)
                {
                    if (boxData.HasTransform())
                    {
                        hittables.Add(new Box(
                            CreateTransform(boxData.Transform),
                            CreateMaterial(boxData.Material)
                            ));
                    }
                    else if (boxData.HasCornerAndDirectionVectors())
                    {
                        hittables.Add(new Box(
                            CreateVec3(boxData.Corner),
                            CreateVec3(boxData.LengthVector),
                            CreateVec3(boxData.WidthVector),
                            CreateVec3(boxData.HeightVector),
                            CreateMaterial(boxData.Material)
                            ));
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            var lightCollection = this.sceneData.Lights;
            List<Light> lights = new List<Light>();
            if (lightCollection.IsValid())
            {
                foreach (var pointLightData in lightCollection.PointLights)
                {
                    lights.Add(new PointLight(
                        CreateTransform(pointLightData.Transform),
                        CreateVec3(pointLightData.Color)
                        ));
                }
            }

            return new Scene(hittables, lights);
        }

        public Camera CreateCamera()
        {
            var cameraData = this.sceneData.Camera;

            return new Camera(
                eyeDirection: CreateVec3(cameraData.EyeDirection),
                plane: this.CreateProjectionPlane());
        }

        public Image CreateImage()
        {
            var imageData = this.sceneData.Image;
            var camera = this.CreateCamera();

            Debug.Assert(imageData != null);
            Debug.Assert(camera != null);

            float aspectRatio = camera.AspectRatio;
            int width = imageData.Width;
            int height = (int)((float)width / aspectRatio);

            return new Image(width, height, imageData.FileName);
        }

        public static bool ValidateList(List<double> vector, int expectedCount, bool shouldBeNormalizedValues = true)
        {
            if (vector != null && vector.Count == expectedCount)
            {
                if (shouldBeNormalizedValues)
                {
                    foreach (var value in vector)
                    {
                        if (!Numbers.InRange(value, 0.0f, 1.0f))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private ProjectionPlane CreateProjectionPlane()
        {
            var projectionPlaneData = this.sceneData.Camera.ProjectionPlane;

            return new ProjectionPlane(
                center: CreateVec3(projectionPlaneData.Center),
                horizontalAxis: CreateVec3(projectionPlaneData.UAxis),
                verticalAxis: CreateVec3(projectionPlaneData.VAxis));
        }

        private static Transform CreateTransform(TransformData data)
        {
            return new Transform(
                position: CreateVec3(data.Position),
                orientation: CreateQuat(data.Orientation),
                scale: CreateVec3(data.Scale));
        }

        private static Material CreateMaterial(MaterialData data)
        {
            return new Material(
                color: CreateVec3(data.Diffuse));
        }

        private static Math.Vec3 CreateVec3(List<double> vec3)
        {
            Debug.Assert(vec3.Count == 3);

            return new Math.Vec3((float)vec3[0], (float)vec3[1], (float)vec3[2]);
        }

        private static Math.Quat CreateQuat(List<double> quat)
        {
            Debug.Assert(quat.Count == 4);

            return new Math.Quat((float)quat[0], (float)quat[1], (float)quat[2], (float)quat[3]);
        }
    }
}