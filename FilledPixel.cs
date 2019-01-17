using System.Drawing;

namespace CGLab3
{
    internal class FilledPixel
    {
        public Point P { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public FilledPixel(Point p, int r, int g, int b)
        {
            P = p;
            R = r;
            G = g;
            B = b;
        }
    }
}
