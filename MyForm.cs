using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGLab3
{
    internal class MyForm : Form
    {
        private static readonly Size TopPictureBoxSize = new Size(431, 541);
        private static readonly Size BottomPictureBoxSize = new Size(431, 246);

        private static readonly PictureBox TopLeftPictureBox = new PictureBox
            {Location = new System.Drawing.Point(0, 0), Size = TopPictureBoxSize, BackColor = Color.White};

        private static readonly PictureBox TopRightPictureBox = new PictureBox
            {Location = new System.Drawing.Point(431, 0), Size = TopPictureBoxSize, BackColor = Color.White};

        private static readonly PictureBox BottomRightPictureBox =
            new PictureBox {Location = new System.Drawing.Point(431, 541), Size = BottomPictureBoxSize};

        private static readonly PictureBox BottomLeftPictureBox = new PictureBox
            {Location = new System.Drawing.Point(0, 541), Size = BottomPictureBoxSize};

        private static readonly List<Triangle> Triangles = new List<Triangle>
        {
            new Triangle(new Point(43, 7), new Point(55, 25), new Point(31, 25)),
            new Triangle(new Point(62, 38), new Point(69, 50), new Point(53, 50)),
            new Triangle(new Point(37, 50), new Point(27, 35), new Point(17, 50)),
            new Triangle(new Point(43, 69), new Point(17, 26), new Point(69, 26)),
           // new Triangle(new Point(43, 7), new Point(69, 50), new Point(17, 50)),


        };

        public MyForm()
        {
            TopLeftPictureBox.Paint += DrawGouraud;
            TopRightPictureBox.Paint += DrawBar;
            DrawImages();

            Controls.Add(TopLeftPictureBox);
            Controls.Add(TopRightPictureBox);
            Controls.Add(BottomLeftPictureBox);
            Controls.Add(BottomRightPictureBox);
        }

        private static void DrawGouraud(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DrawNet(g, TopLeftPictureBox.Width, TopLeftPictureBox.Height);

            foreach (var triangle in Triangles)
            {
                DrawBresenhamLine(g, triangle.L1, triangle);
                DrawBresenhamLine(g, triangle.L2, triangle);
                DrawBresenhamLine(g, triangle.L3, triangle);
                triangle.FilledPoints[triangle.P1] = Color.Red;
                triangle.FilledPoints[triangle.P2] = Color.Green;
                triangle.FilledPoints[triangle.P3] = Color.Blue;
            }

            FillTriangleLines(g);
            foreach (var triangle in Triangles)
            {
                foreach (var point in triangle.FilledPoints)
                {
                    g.FillRectangle(new SolidBrush(point.Value), new Rectangle(new System.Drawing.Point(
                            point.Key.X * 5 + 1,
                            point.Key.Y * 5 + 1),
                        new Size(4, 4)));
                }
            }
        }

        private static void DrawBar(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DrawNet(g, TopRightPictureBox.Width, TopRightPictureBox.Height);

            foreach (var triangle in Triangles)
            {
                FillTriangleBar(triangle, g);
            }
        }

        private static void DrawNet(Graphics g, int width, int height)
        {
            for (var x = 0; x < width; x += 5)
                g.DrawLine(new Pen(Brushes.Black), new System.Drawing.Point(x, 0),
                    new System.Drawing.Point(x, height - 1));

            for (var y = 0; y < height; y += 5)
                g.DrawLine(new Pen(Brushes.Black), new System.Drawing.Point(0, y),
                    new System.Drawing.Point(width - 1, y));
        }

        private static void FillTriangleLines(Graphics g)
        {
            foreach (var triangle in Triangles)
            {
                var lines = new List<Line>
                {
                    triangle.L1,
                    triangle.L2,
                    triangle.L3
                };
                FillLines(triangle, lines, g);
                FillLines(triangle, CreateHorizontal(triangle), g);
            }
        }

        private static void FillLines(Triangle triangle, IEnumerable<Line> lines, Graphics g)
        {
            foreach (var line in lines)
            {
                var d = (int) line.GetLength();
                for (var i = 1; i < line.PointsOfLine.Count - 1; i++)
                {
                    if (!triangle.FilledPoints.ContainsKey(line.LastPoint) ||
                        !triangle.FilledPoints.ContainsKey(line.FirstPoint))
                        continue;
                    var t = Line.GetLength(line.PointsOfLine[i], line.LastPoint) / d;
                    var shade = Color.FromArgb(
                        (int) ((1 - t) * triangle.FilledPoints[line.LastPoint].R + t *
                               triangle.FilledPoints[line.FirstPoint].R),
                        (int) ((1 - t) * triangle.FilledPoints[line.LastPoint].G + t *
                               triangle.FilledPoints[line.FirstPoint].G),
                        (int) ((1 - t) * triangle.FilledPoints[line.LastPoint].B + t *
                               triangle.FilledPoints[line.FirstPoint].B));
                    triangle.FilledPoints[line.PointsOfLine[i]] = shade;
                }
            }
        }

        private static void FillTriangleBar(Triangle triangle, Graphics g)
        {
            foreach (var point in triangle.FilledPoints)
            {
                var tuple = GetBarCoords(point.Key, triangle);
                var shade = Color.FromArgb(
                    (int) Math.Round(tuple.Item1 *
                                     (triangle.FilledPoints[triangle.P1].R + triangle.FilledPoints[triangle.P2].R +
                                      triangle.FilledPoints[triangle.P3].R)),
                    (int) Math.Round(tuple.Item2 *
                                     (triangle.FilledPoints[triangle.P1].G + triangle.FilledPoints[triangle.P2].G +
                                      triangle.FilledPoints[triangle.P3].G)),
                    (int) Math.Round(tuple.Item3 *
                                     (triangle.FilledPoints[triangle.P1].B + triangle.FilledPoints[triangle.P2].B +
                                      triangle.FilledPoints[triangle.P3].B)));
                g.FillRectangle(new SolidBrush(shade),
                    new Rectangle(new System.Drawing.Point(point.Key.X * 5 + 1,
                            point.Key.Y * 5 + 1),
                        new Size(4, 4)));
            }
        }

        private static Tuple<double, double, double> GetBarCoords(Point point, Triangle triangle)
        {
            var delta = (triangle.P2.X - triangle.P1.X) * (triangle.P3.Y - triangle.P1.Y) -
                        (triangle.P2.Y - triangle.P1.Y) * (triangle.P3.X - triangle.P1.X);
            var a = (triangle.P2.X * triangle.P3.Y - triangle.P3.X * triangle.P2.Y +
                     point.X * (triangle.P2.Y - triangle.P3.Y) +
                     point.Y * (triangle.P3.X - triangle.P2.X)) * 1.0 / delta;
            var b = (triangle.P3.X * triangle.P1.Y - triangle.P1.X * triangle.P3.Y +
                     point.X * (triangle.P3.Y - triangle.P1.Y) +
                     point.Y * (triangle.P1.X - triangle.P3.X)) * 1.0 / delta;
            if (a < 0)
                a = 0;
            if (b < 0)
                b = 0;
            var c = 1 - a - b;
            if (c < 0)
                c = 0;
            return Tuple.Create(a, b, c);
        }

        private static IEnumerable<Line> CreateHorizontal(Triangle triangle)
        {
            var lines = new List<Line>();
            for (var i = 0; i < TopLeftPictureBox.Height; i++)
            {
                if (!triangle.PointsOnHorizontal.ContainsKey(i))
                    continue;
                triangle.PointsOnHorizontal[i] =
                    triangle.PointsOnHorizontal[i].OrderBy(x => x.X).ToList();
                for (var j = 0; j < triangle.PointsOnHorizontal[i].Count - 1; j++)
                {
                    lines.Add(new Line(triangle.PointsOnHorizontal[i][j],
                        triangle.PointsOnHorizontal[i][j + 1]));
                }
            }

            for (var i = 0; i < lines.Count; i++)
            {
                lines[i].PointsOfLine = new List<Point>();
                for (var j = Math.Max(lines[i].LastPoint.X, lines[i].FirstPoint.X);
                    j >= Math.Min(lines[i].LastPoint.X, lines[i].FirstPoint.X);
                    j--)
                {
                    foreach (var tr in Triangles)
                    {
                        if (!tr.IsPointInTriangle(new Point(j, lines[i].FirstPoint.Y))) continue;
                        lines[i].PointsOfLine.Add(new Point(j, lines[i].FirstPoint.Y));
                        break;
                    }
                }

                if (lines[i].PointsOfLine.Count > 2)
                    continue;
                lines.Remove(lines[i]);
                i--;
            }

            return lines;
        }

        private static void DrawBresenhamLine(Graphics g, Line line, Triangle triangle)
        {
            var a = line.LastPoint.Y - line.FirstPoint.Y;
            var b = line.FirstPoint.X - line.LastPoint.X;
            line.PointsOfLine = new List<Point>();
            var signA = 1;
            var signB = 1;
            if (a < 0)
                signA = -1;
            if (b < 0)
                signB = -1;
            var f = 0;
            var point = new Point(line.FirstPoint.X, line.FirstPoint.Y);
            line.PointsOfLine.Add(new Point(point.X, point.Y));
            if (!triangle.PointsOnHorizontal.ContainsKey(point.Y))
                triangle.PointsOnHorizontal[point.Y] = new List<Point>();
            triangle.PointsOnHorizontal[point.Y].Add(new Point(point.X, point.Y));

            while (point.X != line.LastPoint.X || point.Y != line.LastPoint.Y)
            {
                if (Math.Abs(a) < Math.Abs(b))
                {
                    f += a * signA;
                    if (f > 0)
                    {
                        f -= b * signB;
                        point.Y += signA;
                    }

                    point.X -= signB;
                    line.PointsOfLine.Add(new Point(point.X, point.Y));
                    if (!triangle.PointsOnHorizontal.ContainsKey(point.Y))
                        triangle.PointsOnHorizontal[point.Y] = new List<Point>();
                    triangle.PointsOnHorizontal[point.Y].Add(new Point(point.X, point.Y));
                }
                else
                {
                    f += b * signB;
                    if (f > 0)
                    {
                        f -= a * signA;
                        point.X -= signB;
                    }

                    point.Y += signA;
                    line.PointsOfLine.Add(new Point(point.X, point.Y));
                    if (!triangle.PointsOnHorizontal.ContainsKey(point.Y))
                        triangle.PointsOnHorizontal[point.Y] = new List<Point>();
                    triangle.PointsOnHorizontal[point.Y].Add(new Point(point.X, point.Y));
                }
            }
        }
        
        private static void DrawImages()
        {
            var bitmap =
                new Bitmap("C:\\correct.bmp");
            const float scaleFactor = 1.2f;
            var newWidth = (int) (bitmap.Width * scaleFactor);
            var newHeight = (int) (bitmap.Height * scaleFactor);
            var newBitmap = new Bitmap(newWidth, newHeight, bitmap.PixelFormat);

            var divX = (float) (bitmap.Width - 1) / newWidth;
            var divY = (float) (bitmap.Height - 1) / newHeight;
            for (var i = 0; i < newWidth; i++)
            {
                for (var j = 0; j < newHeight; j++)
                {
                    var gx = i * divX;
                    var gy = j * divY;
                    var gxi = (int) gx;
                    var gyi = (int) gy;
                    var c00 = bitmap.GetPixel(gxi, gyi);
                    var c10 = bitmap.GetPixel(gxi + 1, gyi);
                    var c01 = bitmap.GetPixel(gxi, gyi + 1);
                    var c11 = bitmap.GetPixel(gxi + 1, gyi + 1);

                    var red = (int) Interpolate(c00.R, c10.R, c01.R, c11.R, gx - gxi, gy - gyi);
                    var green = (int) Interpolate(c00.G, c10.G, c01.G, c11.G, gx - gxi, gy - gyi);
                    var blue = (int) Interpolate(c00.B, c10.B, c01.B, c11.B, gx - gxi, gy - gyi);
                    var rgb = Color.FromArgb(red, green, blue);
                    newBitmap.SetPixel(i, j, rgb);
                }
            }

            BottomLeftPictureBox.Image = bitmap;
            BottomRightPictureBox.Image = newBitmap;
            BottomLeftPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            BottomRightPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }
        
        private static float Interpolate(float c00, float c10, float c01, float c11, float tx, float ty) => 
            Interpolate(Interpolate(c00, c10, tx), Interpolate(c01, c11, tx), ty);

        private static float Interpolate(float a, float b, float q) => a + (b - a) * q;
    }
}