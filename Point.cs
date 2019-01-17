namespace CGLab3
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public System.Drawing.Point ToSDPoint() => new System.Drawing.Point(X, Y);

        public override bool Equals(object obj)
        {
            if (!(obj is Point)) return false;
            var point = (Point) obj;
            return X == point.X && Y == point.Y;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return X * 1023 + Y;
            }
        }
    }
}