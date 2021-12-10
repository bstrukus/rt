/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Newtonsoft.Json;
using System.Collections.Generic;
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
        public List<string> SceneFiles { get; set; }

        [JsonProperty("sceneDir")]
        public string SceneDir { get; set; }

        [JsonProperty("renderDepth")]
        public int RenderDepth { get; set; }

        public override bool IsValid()
        {
            return this.Width > 0 && this.RenderDepth >= 0 &&
                //!string.IsNullOrEmpty(this.SceneFile) &&
                this.SceneFiles != null && this.SceneFiles.Count > 0 && this.ValidateSceneFiles() &&
                (this.IsSceneConversion || this.IsSceneRenderable);
        }

        private bool ValidateSceneFiles()
        {
            foreach (var scenefile in this.SceneFiles)
            {
                if (string.IsNullOrEmpty(scenefile))
                {
                    return false;
                }
            }
            return true;
        }

        public new void PrintData(int spaceCount)
        {
            base.PrintData(spaceCount);

            base.Print("Width", this.Width);
            base.Print("Render Depth", this.RenderDepth);
            //base.Print("Output", this.GetOutputFilename());
            //base.Print("Scene", this.SceneFile);
            base.Print("Scene Directory", this.SceneDir);
        }

        public bool IsSceneConversion => false;//this.AreFileExtensionsSame(this.SceneFile, OldSceneFileExtension);
        public bool IsSceneRenderable => true;//this.AreFileExtensionsSame(this.SceneFile, SceneFileExtension);

        private bool AreFileExtensionsSame(string filenameWithExtension, string extension)
        {
            var tokens = filenameWithExtension.Split('.');
            return string.Compare(tokens[1], extension) == 0;
        }

        public string GetOutputFilename(string sceneFile)
        {
            // #todo Move this to Dir
            var tokens = sceneFile.Split('.');
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