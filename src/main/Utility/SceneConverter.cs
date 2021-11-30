/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace rt.Utility
{
    using rt.Data;

    internal class SceneConverter
    {
        private const char Delimiter = ' ';

        public static void Convert(string filename)
        {
            // Find the actual file
            string fullFilePath = Dir.GetOldSceneFilePath(filename);
            Log.Info(fullFilePath);

            var sceneData = new SceneData();

            try
            {
                // Open it
                using StreamReader sr = new StreamReader(fullFilePath);

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (ShouldIgnoreLine(line))
                    {
                        line = sr.ReadLine();
                        continue;
                    }

                    // Process Line
                    line = line.Replace(", ", ",");
                    while (line.Contains("  "))
                    {
                        line = line.Replace("  ", " ");
                    }

                    var tag = line.Split(Delimiter)[0];

                    Log.Info(tag);

                    if (string.Compare(tag, "SPHERE", true) == 0)
                    {
                        sceneData.AddShape(ReadSphere(line, sr.ReadLine()));
                    }
                    else if (string.Compare(tag, "BOX", true) == 0)
                    {
                        sceneData.AddShape(ReadBox(line, sr.ReadLine()));
                    }
                    else if (string.Compare(tag, "POLYGON", true) == 0)
                    {
                        sceneData.AddShape(ReadPolygon(line, sr.ReadLine()));
                    }
                    else if (string.Compare(tag, "ELLIPSOID", true) == 0)
                    {
                        sceneData.AddShape(ReadEllipsoid(line, sr.ReadLine()));
                    }
                    else if (string.Compare(tag, "LIGHT", true) == 0)
                    {
                        sceneData.AddLight(ReadPointLight(line));
                    }
                    else if (string.Compare(tag, "CAMERA", true) == 0)
                    {
                        sceneData.Camera = (ReadCamera(line));
                    }
                    else if (string.Compare(tag, "AIR", true) == 0)
                    {
                        sceneData.Air = ReadAir(line);
                    }
                    else if (string.Compare(tag, "AMBIENT", true) == 0)
                    {
                        sceneData.Ambient = ReadAmbient(line);
                    }

                    line = sr.ReadLine();
                }

                if (!sceneData.IsValid())
                {
                    Log.Error("Error loading in CS500 file format");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
            }

            if (Save(sceneData, filename))
            {
                Log.Info("File successfully saved.");
            }
            else
            {
                Log.Error("Something went wrong.");
            }
        }

        private static bool Save(SceneData sceneData, string outputFilename)
        {
            string newFilename = ReplaceFileExtension(outputFilename, ".json");
            string outputFilePath = Dir.GetSceneFilePath(newFilename);
            using (StreamWriter file = File.CreateText(outputFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, sceneData);
                return true;
            }
        }

        private static bool ShouldIgnoreLine(string line)
        {
            line = line.Trim();
            return string.IsNullOrEmpty(line) || line[0] == '#';
        }

        private static string ReplaceFileExtension(string filename, string extension)
        {
            string[] fileNameParts = filename.Split('.');
            return fileNameParts[0] + extension;
        }

        #region Read Objects

        private static SphereData ReadSphere(string sphereLine, string materialLine)
        {
            Debug.Assert(!string.IsNullOrEmpty(sphereLine));

            var tokens = sphereLine.Split(Delimiter);

            var sphereData = new SphereData
            {
                Radius = double.Parse(tokens[2]),
                Transform = ReadSimpleTransform(tokens[1]),
                Material = ReadMaterial(materialLine)
            };

            return sphereData;
        }

        private static BoxData ReadBox(string boxLine, string materialLine)
        {
            Debug.Assert(!string.IsNullOrEmpty(boxLine));

            var tokens = boxLine.Split(Delimiter);

            var boxData = new BoxData
            {
                Transform = null,
                Corner = ReadVector(tokens[1]),
                LengthVector = ReadVector(tokens[2]),
                WidthVector = ReadVector(tokens[3]),
                HeightVector = ReadVector(tokens[4]),
                Material = ReadMaterial(materialLine)
            };

            return boxData;
        }

        private static PolygonData ReadPolygon(string polygonLine, string materialLine)
        {
            Debug.Assert(!string.IsNullOrEmpty(polygonLine));

            var tokens = polygonLine.Split(Delimiter);

            int pointCount = int.Parse(tokens[1]);
            List<List<double>> vertices = new List<List<double>>(pointCount);
            for (int i = 2; i < tokens.Length; ++i)
            {
                vertices.Add(ReadVector(tokens[i]));
            }

            var polygonData = new PolygonData
            {
                Vertices = vertices,
                Material = ReadMaterial(materialLine)
            };

            return polygonData;
        }

        private static EllipsoidData ReadEllipsoid(string ellipsoidLine, string materialLine)
        {
            Debug.Assert(!string.IsNullOrEmpty(ellipsoidLine));

            var tokens = ellipsoidLine.Split(Delimiter);

            var ellipsoidData = new EllipsoidData
            {
                Center = ReadVector(tokens[1]),
                AxisU = ReadVector(tokens[2]),
                AxisV = ReadVector(tokens[3]),
                AxisW = ReadVector(tokens[4]),
                Material = ReadMaterial(materialLine)
            };

            return ellipsoidData;
        }

        #endregion Read Objects

        private static MaterialData ReadMaterial(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Trim().Split(Delimiter);

            return new MaterialData
            {
                Diffuse = ReadVector(tokens[0]),
                SpecularCoefficient = double.Parse(tokens[1]),
                SpecularExponent = double.Parse(tokens[2]),
                TransmissionAttenuation = ReadVector(tokens[3]),
                ElectricPermittivity = double.Parse(tokens[4]),
                MagneticPermeability = double.Parse(tokens[5])
            };
        }

        private static PointLightData ReadPointLight(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Split(Delimiter);

            return new PointLightData
            {
                Transform = ReadSimpleTransform(tokens[1]),
                Color = ReadVector(tokens[2]),
                Radius = double.Parse(tokens[3])
            };
        }

        private static CameraData ReadCamera(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var eyeDirectionData = data.Split(Delimiter)[4];

            return new CameraData
            {
                EyeDirection = ReadVector(eyeDirectionData),
                ProjectionPlane = ReadProjectionPlane(data)
            };
        }

        private static ProjectionPlaneData ReadProjectionPlane(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Split(Delimiter);

            return new ProjectionPlaneData
            {
                Center = ReadVector(tokens[1]),
                UAxis = ReadVector(tokens[2]),
                VAxis = ReadVector(tokens[3]),
            };
        }

        private static AmbientData ReadAmbient(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Split(Delimiter);

            return new AmbientData
            {
                Color = ReadVector(tokens[1])
            };
        }

        private static AirData ReadAir(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Split(Delimiter);

            return new AirData
            {
                ElectricPermittivity = double.Parse(tokens[1]),
                MagneticPermeability = double.Parse(tokens[2]),
                AttenuationFactors = ReadVector(tokens[3])
            };
        }

        private static TransformData ReadSimpleTransform(string position)
        {
            Debug.Assert(!string.IsNullOrEmpty(position));

            var transformData = new TransformData
            {
                Position = ReadVector(position)
            };

            transformData.Orientation.Add(0.0f);
            transformData.Orientation.Add(0.0f);
            transformData.Orientation.Add(0.0f);
            transformData.Orientation.Add(1.0f);

            transformData.Scale.Add(1.0f);
            transformData.Scale.Add(1.0f);
            transformData.Scale.Add(1.0f);

            return transformData;
        }

        private static List<double> ReadVector(string vector)
        {
            vector = vector.Trim('(', ')');
            var tokens = vector.Split(',');

            List<double> parsedVector = new List<double>(tokens.Length);
            foreach (var value in tokens)
            {
                parsedVector.Add(double.Parse(value));
            }

            return parsedVector;
        }
    }
}