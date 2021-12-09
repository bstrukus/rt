/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Math
{
    public class Quat
    {
        public static Quat Identity = new Quat(0.0f, 0.0f, 0.0f, 1.0f);

        public Quat(float x, float y, float z, float w)
        {
            this.val[0] = x;
            this.val[1] = y;
            this.val[2] = z;
            this.val[3] = w;
        }

        private readonly float[] val = new float[4];
    }
}