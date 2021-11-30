/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System.Diagnostics;

namespace rt.Data
{
    using rt.Utility;

    public class ConfigData : DataBase
    {
        private const string SceneFileExtension = "json";
        private const string OldSceneFileExtension = "txt";
        private const string DefaultOutputImageExtension = "bmp";

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }

        [JsonProperty("sceneFile")]
        public string SceneFile { get; set; }

        [JsonProperty("sceneDir")]
        public string SceneDir { get; set; }

        [JsonProperty("renderDepth")]
        public int RenderDepth { get; set; }

        public override bool IsValid()
        {
            return this.Width > 0 && this.RenderDepth >= 0 &&
                !string.IsNullOrEmpty(this.SceneFile) &&
                (this.IsSceneConversion || this.IsSceneRenderable);
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            base.Print("Width", this.Width);
            base.Print("Render Depth", this.RenderDepth);
            base.Print("Output", this.GetOutputFilename());
            base.Print("Scene", this.SceneFile);
            base.Print("Scene Directory", this.SceneDir);
        }

        public bool IsSceneConversion => this.AreFileExtensionsSame(this.SceneFile, OldSceneFileExtension);
        public bool IsSceneRenderable => this.AreFileExtensionsSame(this.SceneFile, SceneFileExtension);

        private bool AreFileExtensionsSame(string filenameWithExtension, string extension)
        {
            var tokens = filenameWithExtension.Split('.');
            return string.Compare(tokens[1], extension) == 0;
        }

        public string GetSceneFullFilePath()
        {
            // #todo Move this to Dir
            if (string.IsNullOrEmpty(this.SceneDir))
            {
                // Use hardcoded file info
                return Utility.Dir.GetSceneFilePath(this.SceneFile);
            }
            else
            {
                // #todo Build out scene filepath from config info
                Log.Warning("NOT IMPLEMENTED - Input file info was provided, but is not currently being used.");
                return null;
            }
        }

        public string GetOutputFilename()
        {
            // #todo Move this to Dir
            var tokens = this.SceneFile.Split('.');
            Debug.Assert(tokens.Length == 2);

            var fileExtension = tokens[1];
            if (string.Compare(fileExtension, OldSceneFileExtension) == 0)
            {
                Log.Info("Converting!");
                fileExtension = SceneFileExtension;
            }
            else if (string.Compare(fileExtension, SceneFileExtension) == 0)
            {
                Log.Info("Rendering!");
                fileExtension = DefaultOutputImageExtension;
            }

            string filename = tokens[0];

            if (!string.IsNullOrEmpty(this.Output))
            {
                // #todo Use the output filename, need to figure out if it has a file extension and what it could be
                Log.Warning("NOT IMPLEMENTED - Output file info was provided, but is not currently being used.");
            }

            return $"{filename}.{fileExtension}";
        }
    }
}