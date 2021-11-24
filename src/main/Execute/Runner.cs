/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
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
            // Generate rays from the camera's eye through the projection plane
            for (int y = 0; y < this.image.Height; ++y)
            {
                for (int x = 0; x < this.image.Width; ++x)
                {
                    var scaledPixel = this.image.InterpolatedPixel(x, y);
                    var ray = this.camera.GenerateRay(scaledPixel);

                    var colorReport = this.scene.Trace(ray);
                    image.SetPixel(x, y, color: colorReport.Color);
                }
            }

            image.Save();

            image.Open();
        }
    }
}