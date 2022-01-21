using System;
using System.Collections.Generic;

namespace BLL.Algorithms
{
    public static class GaussSquare
    {
        public static double Square { get; private set; }

        public static double FindSquare(List<Point> pointList)
        {
            int n = pointList.Count;
            Square = 0;
            Point FirstPoint = pointList[0];
            pointList.Add(FirstPoint);

            for (int i = 0; i < n; i++)
            {
                Square += (pointList[i].X * pointList[i + 1].Y) - (pointList[i + 1].X * pointList[i].Y);
            }
            Square = 0.5 * Math.Abs(Square);

            return Square;
        }
    }
}
