/*
 * #copyright_placeholder Copyright Ben Strukus
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Render
{
    using rt.Math;
    using rt.Render;

    [TestClass]
    public class PixelBufferTests
    {
        [TestMethod]
        [DataRow(100, 100, "test100.bmp")]
        public void ProduceGrayScaleImageOfIncreasingValue(int width, int height, string filename)
        {
            // Arrange
            var pixelBuffer = new PixelBuffer(width, height);

            // Act
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vec3 color = this.ScaleColorByValue(Vec3.AxisX, x, width) + this.ScaleColorByValue(Vec3.AxisY, y, height);
                    pixelBuffer.SetPixel(x, y, color);
                }
            }

            pixelBuffer.Save(filename);

            // Assert
            // See output image
        }

        private Vec3 ScaleColorByValue(Vec3 color, int currVal, int maxVal)
        {
            return color * ((float)currVal / (float)maxVal);
        }
    }
}