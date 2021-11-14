﻿/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Utility
{
    using rt.Data;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    internal class SceneConverter
    {
        public static void Convert(string filename)
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
                    if (ShouldIgnoreLine(line))
                    {
                        line = sr.ReadLine();
                        continue;
                    }

                    var tag = line.Split(' ')[0];

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
                        // #todo Store Air data somewhere
                        ReadAir(line);
                    }
                    else if (string.Compare(tag, "AMBIENT", true) == 0)
                    {
                        // #todo Store Ambient data somewhere
                        ReadAmbient(line);
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
        }

        private static bool ShouldIgnoreLine(string line)
        {
            line = line.Trim();
            return string.IsNullOrEmpty(line) || line[0] == '#';
        }

        private static string ReplaceFileExtension(string filename)
        {
            string[] fileNameParts = filename.Split('.');
            return fileNameParts[0] + ".json";
        }

        #region Read Objects

        private static SphereData ReadSphere(string sphereLine, string materialLine)
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

        private static BoxData ReadBox(string boxLine, string materialLine)
        {
            // #todo Read in Box shape
            Debug.Assert(!string.IsNullOrEmpty(boxLine));

            var tokens = boxLine.Split(' ');

            //             var sphereData = new SphereData
            //             {
            //                 Radius = double.Parse(tokens[2]),
            //                 Transform = ReadSimpleTransform(tokens[1]),
            //                 Material = ReadMaterial(materialLine)
            //             };

            return null;
        }

        private static PolygonData ReadPolygon(string polygonLine, string materialLine)
        {
            // #todo Read in Polygon shape
            Debug.Assert(!string.IsNullOrEmpty(polygonLine));

            var tokens = polygonLine.Split(' ');

            //             var sphereData = new SphereData
            //             {
            //                 Radius = double.Parse(tokens[2]),
            //                 Transform = ReadSimpleTransform(tokens[1]),
            //                 Material = ReadMaterial(materialLine)
            //             };

            return null;
        }

        private static EllipsoidData ReadEllipsoid(string ellipsoidLine, string materialLine)
        {
            // #todo Read in Ellipsoid shape
            Debug.Assert(!string.IsNullOrEmpty(ellipsoidLine));

            var tokens = ellipsoidLine.Split(' ');

            //             var sphereData = new SphereData
            //             {
            //                 Radius = double.Parse(tokens[2]),
            //                 Transform = ReadSimpleTransform(tokens[1]),
            //                 Material = ReadMaterial(materialLine)
            //             };

            return null;
        }

        #endregion Read Objects

        private static MaterialData ReadMaterial(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Trim().Split(' ');

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

            var tokens = data.Split(' ');

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

            var eyeDirectionData = data.Split(' ')[4];

            return new CameraData
            {
                EyeDirection = ReadVector(eyeDirectionData),
                ProjectionPlane = ReadProjectionPlane(data)
            };
        }

        private static ProjectionPlaneData ReadProjectionPlane(string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            var tokens = data.Split(' ');

            return new ProjectionPlaneData
            {
                Center = ReadVector(tokens[1]),
                UAxis = ReadVector(tokens[2]),
                VAxis = ReadVector(tokens[3]),
            };
        }

        private static AmbientData ReadAmbient(string data)
        {
            // #todo Read in Ambient data
            Debug.Assert(!string.IsNullOrEmpty(data));
            return null;
        }

        private static AirData ReadAir(string data)
        {
            // #todo Read in Air data
            Debug.Assert(!string.IsNullOrEmpty(data));
            return null;
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