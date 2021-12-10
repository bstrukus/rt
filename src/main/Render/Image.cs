/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Diagnostics;

namespace rt.Render
{
    using rt.Math;
    using rt.Utility;

    /// <summary>
    /// Represents the final output image and all its properties.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Width of image, in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of image, in pixels.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Value specifying the number of recursive ray casts allowed during scene tracing.
        /// </summary>
        public int RenderDepth { get; private set; }

        public string FileName { get; private set; }

        private PixelBuffer buffer;
        private int pixelCount;

        public Image(int width, int height, string fileName, int renderDepth)
        {
            this.Width = width;
            this.Height = height;
            this.FileName = fileName;
            this.RenderDepth = renderDepth;
            this.pixelCount = this.Width * this.Height;

            this.buffer = new PixelBuffer(this.Width, this.Height);
        }

        public void SetPixel(int x, int y, Vec3 color)
        {
            Debug.Assert(Numbers.InRange(x, 0, this.Width));
            Debug.Assert(Numbers.InRange(y, 0, this.Height));

            this.buffer.SetPixel(x, y, color);
            this.PrintProgress(x, y);
        }

        private void PrintProgress(int x, int y)
        {
            int currPixel = 1 + x + (y * this.Width);
            float percentage = 100.0f * (float)currPixel / (float)this.pixelCount;
            string progress = $"PROGRESS: {percentage.ToString("0.00")}%";
            Log.WriteInPlace(progress);
        }

        public void Save()
        {
            this.buffer.Save(this.FileName);
        }

        public void Open()
        {
            string outputFile = Dir.GetOutputFilePath(this.FileName);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo(outputFile)
                {
                    UseShellExecute = true
                }
            };
            p.Start();
        }

        public Vec2 InterpolatedPixel(int x, int y)
        {
            return new Vec2(this.InterpolatedStep(x, this.Width), this.InterpolatedStep(y, this.Height));
        }

        private float InterpolatedStep(int step, int stepCount)
        {
            // [0, 1] -> [-1, 1] => [0, 1] * 2 => [0, 2] - 1 => [-1, 1]
            float floatStep = (float)step / (float)stepCount;
            floatStep *= 2.0f;
            floatStep -= 1.0f;
            return floatStep;
        }
    }
}