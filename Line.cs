using System;
using System.Collections.Generic;
using System.Drawing;

namespace CGLab3
{
    internal class Line
    {
        public Point FirstPoint { get; }
        public Point LastPoint { get; }
        public List<Point> PointsOfLine { get; set; }

        public Line(Point firstPoint, Point lastPoint)
        {
            FirstPoint = new Point(firstPoint.X, firstPoint.Y);
            LastPoint = new Point(lastPoint.X, lastPoint.Y);
        }

        public double GetLength() =>
            Math.Sqrt((LastPoint.X - FirstPoint.X) * (LastPoint.X - FirstPoint.X) +
                      (LastPoint.Y - FirstPoint.Y) * (LastPoint.Y - FirstPoint.Y));
        
        public static double GetLength(Point p1, Point p2) =>
            Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) +
                      (p2.Y - p1.Y) * (p2.Y - p1.Y));

        public static Line operator +(Line line, Point point) =>
            new Line(new Point(line.FirstPoint.X + point.X, line.FirstPoint.Y + point.Y),
                new Point(line.LastPoint.X + point.X, line.LastPoint.Y + point.Y));
    }
}