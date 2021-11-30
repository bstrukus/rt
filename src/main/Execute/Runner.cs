/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
    using rt.Utility;

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
            this.StartTimer();

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

            this.EndTimer();
            this.FinalizeImage();
        }

        private void FinalizeImage()
        {
            this.image.Save();

            this.image.Open();
        }

        private void StartTimer()
        {
            this.stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        private void EndTimer()
        {
            this.stopwatch.Stop();
            Log.Info($"RUN TIME: " + string.Format("{0:0.00}", this.stopwatch.Elapsed.TotalSeconds) + " Mississippis");
        }
    }
}