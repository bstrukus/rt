/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
    using rt.Utility;
    using System;

    public class Runner
    {
        private readonly Render.Camera camera;
        private readonly Render.Image image;
        private readonly Present.Scene scene;

        private System.Diagnostics.Stopwatch stopwatch;

        public Runner(Present.Scene scene, Render.Camera camera, Render.Image image)
        {
            this.scene = scene;

            this.camera = camera;
            this.image = image;
        }

        public void Execute()
        {
            this.InitRender();

            // Generate rays from the camera's eye through the projection plane
            for (int y = 0; y < this.image.Height; ++y)
            {
                for (int x = 0; x < this.image.Width; ++x)
                {
                    var interpolatedPixel = this.image.InterpolatedPixel(x, y);
                    var ray = this.camera.GenerateRay(interpolatedPixel);

                    var colorReport = this.scene.Trace(ray, this.image.RenderDepth);
                    this.image.SetPixel(x, y, color: colorReport.Color);
                }
            }

            this.FinishRender();
        }

        private void InitRender()
        {
            // Start our render timer
            this.stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Log.Write("");  // Create a newline to help print progress correctly
        }

        private void FinishRender()
        {
            // End the render timer
            this.stopwatch.Stop();
            Log.Info($"RUN TIME: " + string.Format("{0:0.00}", this.stopwatch.Elapsed.TotalSeconds) + " Mississippis");

            // Save image
            this.image.Save();
            this.image.Open();
        }
    }
}