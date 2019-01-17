using System.Collections.Generic;
using System.Drawing;

namespace CGLab3
{
    internal class Triangle
    {
        public Point P1 { get; }
        public Point P2 { get; }
        public Point P3 { get; }
        
        public Line L1 { get; }
        public Line L2 { get; }
        public Line L3 { get; }
        
        public readonly Dictionary<Point, Color> FilledPoints = new Dictionary<Point, Color>();
        
        public readonly Dictionary<int, List<Point>> PointsOnHorizontal = new Dictionary<int, List<Point>>();

        public Triangle(Point p1, Point p2, Point p3)
        {
            P1 = new Point(p1.X, p1.Y);
            P2 = new Point(p2.X, p2.Y);
            P3 = new Point(p3.X, p3.Y);
            L1 = new Line(P1, P2);
            L2 = new Line(P2, P3);
            L3 = new Line(P3, P1);
        }

        public bool IsPointInTriangle(Point p)
        {
            if (L1.PointsOfLine.Contains(p))
                return true;
            if (L2.PointsOfLine.Contains(p))
                return true;
            if (L3.PointsOfLine.Contains(p))
                return true;
            var a = (P1.X - p.X) * (P2.Y - P1.Y) - (P2.X - P1.X) * (P1.Y - p.Y);
            var b = (P2.X - p.X) * (P3.Y - P2.Y) - (P3.X - P2.X) * (P2.Y - p.Y);
            var c = (P3.X - p.X) * (P1.Y - P3.Y) - (P1.X - P3.X) * (P3.Y - p.Y);
            return a > 0 && b > 0 && c > 0 || a < 0 && b < 0 && c < 0;
        }
    }
}
