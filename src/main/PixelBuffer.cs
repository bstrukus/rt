/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using System.Drawing;

namespace rt
{
    public class PixelBuffer
    {
        private Bitmap bitmap;

        public PixelBuffer(int width, int height)
        {
            this.bitmap = new Bitmap(width, height);
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
            this.bitmap.Save(filename);
        }
    }
}