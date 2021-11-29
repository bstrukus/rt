/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
    using rt.Utility;

    public class Runner
    {
        private Render.Camera camera;
        private Render.Image image;
        private Present.Scene scene;

        // Jobs
        public Runner(Workload workload)
        {
            //
        }

        public Runner(Present.Scene scene, Render.Camera camera, Render.Image image)
        {
            this.scene = scene;

            this.camera = camera;
            this.image = image;
        }

        public void Execute()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            const int depth = 1;

            // Generate rays from the camera's eye through the projection plane
            for (int y = 0; y < this.image.Height; ++y)
            {
                for (int x = 0; x < this.image.Width; ++x)
                {
                    var interpolatedPixel = this.image.InterpolatedPixel(x, y);
                    var ray = this.camera.GenerateRay(interpolatedPixel);

                    var colorReport = this.scene.Trace(ray, depth);
                    image.SetPixel(x, y, color: colorReport.Color);
                }
            }

            stopwatch.Stop();
            Log.Info($"RUN TIME: " + string.Format("{0:0.00}", stopwatch.Elapsed.TotalSeconds) + " seconds");

            image.Save();

            image.Open();
        }
    }
}