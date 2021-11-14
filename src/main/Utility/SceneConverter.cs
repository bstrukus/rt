/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using rt.Data;
    using System.Diagnostics;
    using System.IO;

    internal class SceneConverter
    {
        static public void Convert(string filename)
        {
            // Find the actual file
            string fullFilePath = Dir.GetOldSceneFilePath(filename);
            Log.Info(fullFilePath);

            Log.Info($"New File: {ReplaceFileExtension(filename)}");

            var sceneData = new SceneData();

            try
            {
                // Open it
                using StreamReader sr = new StreamReader(fullFilePath);

                string line = sr.ReadLine();
                while (line != null)
                {
                    var tag = line.Split(' ')[0];

                    if (string.Compare(tag, "SPHERE", true) == 0)
                    {
                        sceneData.AddShape(ReadSphere(line, sr.ReadLine()));
                    }
                    else if (string.Compare(tag, "LIGHT", true) == 0)
                    {
                        sceneData.AddLight(ReadPointLight(line));
                    }
                    else if (string.Compare(tag, "CAMERA", true) == 0)
                    {
                        sceneData.Camera = (ReadCamera(line));
                    }

                    line = sr.ReadLine();
                }

            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        static private string ReplaceFileExtension(string filename)
        {
            string[] fileNameParts = filename.Split('.');
            return fileNameParts[0] + ".json";
        }


        static private SphereData ReadSphere(string sphereLine, string materialLine)
        {
            Debug.Assert(!string.IsNullOrEmpty(sphereLine));

            var tokens = sphereLine.Split(' ');

            var sphereData = new SphereData
            {
                Radius = double.Parse(tokens[2]),
                Transform = ReadSimpleTransform(tokens[1]),
                Material = ReadMaterial(materialLine)
            };

            return sphereData;
        }

        static private MaterialData ReadMaterial(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            return new MaterialData();
        }

        static private PointLightData ReadPointLight(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            return new PointLightData();
        }

        static private CameraData ReadCamera(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));
            return new CameraData();
        }

        static private AmbientData ReadAmbient(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));
            return null;
        }

        static private AirData ReadAir(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));
            return null;
        }

        static private TransformData ReadSimpleTransform(string position)
        {
            Debug.Assert(!string.IsNullOrEmpty(position));
            
            position = position.Trim('(', ')');
            var tokens = position.Split(',');

            var transformData = new TransformData();

            transformData.Position.Add(float.Parse(tokens[0]));
            transformData.Position.Add(float.Parse(tokens[1]));
            transformData.Position.Add(float.Parse(tokens[2]));

            transformData.Orientation.Add(0.0f);
            transformData.Orientation.Add(0.0f);
            transformData.Orientation.Add(0.0f);
            transformData.Orientation.Add(1.0f);

            transformData.Scale.Add(1.0f);
            transformData.Scale.Add(1.0f);
            transformData.Scale.Add(1.0f);

            return transformData;
        }
    }
}
