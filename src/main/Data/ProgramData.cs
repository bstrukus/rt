/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Newtonsoft.Json;
using rt.Utility;
using System.Diagnostics;

namespace rt.Data
{
    public class ProgramData
    {
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }

        [JsonProperty("sceneFile")]
        public string SceneFile { get; set; }

        public string GetOutputFilename()
        {
            var tokens = this.SceneFile.Split('.');
            Debug.Assert(tokens.Length == 2);

            var fileExtension = tokens[1];
            if (string.Compare(fileExtension, "txt") == 0)
            {
                Log.Info("Converting!");
                fileExtension = "json";
            }
            else if (string.Compare(fileExtension, "json") == 0)
            {
                Log.Info("Rendering!");
                fileExtension = "bmp";
            }
            return $"{tokens[0]}.{fileExtension}";
        }
    }
}