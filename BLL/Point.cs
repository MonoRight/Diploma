namespace BLL
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool LeftFlag { get; set; } = false;
        public bool RightFlag { get; set; } = false;

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            LeftFlag = false;
            RightFlag = false;
        }

        public Point()
        {
        }
    }
}
