/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Drawing;

namespace rt.Render
{
    public class PixelBuffer
    {
        private Bitmap bitmap;

        public PixelBuffer(int width, int height)
        {
            this.bitmap = new Bitmap(width, height);
        }

        public void SetPixel(int x, int y, Math.Vec3 color)
        {
            int[] intColor =
            {
                (int)(color.X * 255.0f),
                (int)(color.Y * 255.0f),
                (int)(color.Z * 255.0f),
            };
            this.bitmap.SetPixel(x, y, Color.FromArgb(intColor[0], intColor[1], intColor[2]));
        }

        public void Fill()
        {
            for (int x = 0; x < this.bitmap.Width; ++x)
            {
                for (int y = 0; y < this.bitmap.Height; ++y)
                {
                    this.bitmap.SetPixel(x, y, Color.FromArgb(x, x, x));
                }
            }
        }

        public void Save(string filename)
        {
            // Bitmap's [0, 0] is in the upper-left, but ray casts start in the lower-left. FLIP!
            RotateFlipType flipType =
                RotateFlipType.RotateNoneFlipY; // If just the y-axis is flipped, this should be all I need

            this.bitmap.RotateFlip(flipType);
            this.bitmap.Save(filename);
        }
    }
}