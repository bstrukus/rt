/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace rt.Data
{
    using rt.Collide;
    using rt.Collide.Shapes;
    using rt.Execute;
    using rt.Math;
    using rt.Present;
    using rt.Render;
    using rt.Utility;

    /// <summary>
    /// Consumes a data file, outputs components of the ray tracer
    /// </summary>
    public class DataFactory
    {
        private SceneData sceneData;

        public ProgramData LoadConfig(string filename)
        {
            using StreamReader file = File.OpenText(filename);
            string fileContents = file.ReadToEnd();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramData>(fileContents);
        }

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

            if (Levers.GetOption(Levers.Option.PrintSceneLoading))
            {
                this.sceneData.PrintData(0);
            }
            return true;
        }

        public Scene CreateScene()
        {
            //////////////////////////////////////////////////////////////////////////
            var shapeCollection = this.sceneData.Shapes;

            // Go through the list of Spheres & Boxes to create a list of IHittables
            List<IHittable> hittables = new List<IHittable>();
            this.LoadSpheres(ref hittables);
            this.LoadBoxes(ref hittables);
            this.LoadPolygons(ref hittables);
            this.LoadEllipsoids(ref hittables);

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

        private int LoadSpheres(ref List<IHittable> hittables)
        {
            if (this.sceneData.Shapes.HasSpheres())
            {
                foreach (var sphereData in this.sceneData.Shapes.Spheres)
                {
                    hittables.Add(new Sphere(
                        CreateTransform(sphereData.Transform),
                        CreateMaterial(sphereData.Material),
                        (float)sphereData.Radius
                        ));
                }
                return this.sceneData.Shapes.Spheres.Count;
            }
            return 0;
        }

        private int LoadBoxes(ref List<IHittable> hittables)
        {
            if (this.sceneData.Shapes.HasBoxes())
            {
                foreach (var boxData in this.sceneData.Shapes.Boxes)
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
                return this.sceneData.Shapes.Boxes.Count;
            }
            return 0;
        }

        private int LoadPolygons(ref List<IHittable> hittables)
        {
            if (this.sceneData.Shapes.HasPolygons())
            {
                foreach (var polygonData in this.sceneData.Shapes.Polygons)
                {
                    hittables.Add(new Polygon(
                        CreateVertexList(polygonData.Vertices),
                        CreateMaterial(polygonData.Material)
                        ));
                }
                return this.sceneData.Shapes.Polygons.Count;
            }
            return 0;
        }

        private int LoadEllipsoids(ref List<IHittable> hittables)
        {
            if (this.sceneData.Shapes.HasEllipsoids())
            {
                foreach (var ellipsoidData in this.sceneData.Shapes.Ellipsoids)
                {
                    hittables.Add(new Ellipsoid(
                        CreateVec3(ellipsoidData.Center),
                        CreateVec3(ellipsoidData.AxisU),
                        CreateVec3(ellipsoidData.AxisV),
                        CreateVec3(ellipsoidData.AxisW),
                        CreateMaterial(ellipsoidData.Material)
                        ));
                }
                return this.sceneData.Shapes.Ellipsoids.Count;
            }
            return 0;
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

        private static List<Vec3> CreateVertexList(List<List<double>> vertexData)
        {
            List<Vec3> vertexList = new List<Vec3>(vertexData.Count);
            foreach (var vertex in vertexData)
            {
                vertexList.Add(CreateVec3(vertex));
            }
            return vertexList;
        }
    }
}