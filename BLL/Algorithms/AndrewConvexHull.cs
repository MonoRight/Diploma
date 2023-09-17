using System.Collections.Generic;
using System.Linq;

namespace BLL.Algorithms
{
    public class AndrewConvexHull
    {
        private double Cross(Point O, Point A, Point B)
        {
            return ((A.X - O.X) * (B.Y - O.Y)) - ((A.Y - O.Y) * (B.X - O.X));
        }

        public List<Point> CreateConvexHull(List<Point> initialPoints, double maxDistBetweenPoints, double maxDepth)
        {
            if (initialPoints.Count > 1)
            {
                int n = initialPoints.Count, k = 0;
                List<Point> hullPoints = new List<Point>(new Point[2 * n]);

                initialPoints.Sort((a, b) => a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));

                // Build lower hull
                for (int i = 0; i < n; ++i)
                {
                    while (k >= 2 && Cross(hullPoints[k - 2], hullPoints[k - 1], initialPoints[i]) <= 0)
                        k--;
                    hullPoints[k++] = initialPoints[i];
                }

                // Build upper hull
                for (int i = n - 2, t = k + 1; i >= 0; i--)
                {
                    while (k >= t && Cross(hullPoints[k - 2], hullPoints[k - 1], initialPoints[i]) <= 0)
                        k--;
                    hullPoints[k++] = initialPoints[i];
                }

                hullPoints = hullPoints.Take(k - 1).ToList();
                DentInclusion dentInclusion = new DentInclusion();
                List<Point> notIncludedPoints = dentInclusion.NotIncludedPointsInConvexHull(ref initialPoints, ref hullPoints);

                return dentInclusion.AddPointsToHull(ref hullPoints, ref notIncludedPoints, maxDistBetweenPoints, maxDepth).ToList();
            }
            else if (initialPoints.Count <= 1)
            {
                return initialPoints;
            }
            else
            {
                return null;
            }
        }
    }
}
