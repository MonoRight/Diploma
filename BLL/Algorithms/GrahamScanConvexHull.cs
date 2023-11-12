using BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Algorithms
{
    public class GrahamScanConvexHull
    {
        public List<Point> GrahamScanCompute(List<Point> initialPoints, double maxDistBetweenPoints, double maxDepth)
        {
            if (initialPoints.Count < 2)
            {
                return initialPoints;
            }

            // Find point with minimum y; if more than one, minimize x also.
            int iMin = Enumerable.Range(0, initialPoints.Count).Aggregate((jMin, jCur) =>
            {
                if (initialPoints[jCur].Y < initialPoints[jMin].Y)
                {
                    return jCur;
                }
                if (initialPoints[jCur].Y > initialPoints[jMin].Y)
                {
                    return jMin;
                }
                if (initialPoints[jCur].X < initialPoints[jMin].X)
                {
                    return jCur;
                }

                return jMin;
            });
            // Sort them by polar angle from iMin, 
            IEnumerable<Point> sortQuery = Enumerable.Range(0, initialPoints.Count)
                .Where((i) => i != iMin) // Skip the min point
                .Select((i) => new KeyValuePair<double, Point>(Math.Atan2(initialPoints[i].Y - initialPoints[iMin].Y, initialPoints[i].X - initialPoints[iMin].X), initialPoints[i]))
                .OrderBy((pair) => pair.Key)
                .Select((pair) => pair.Value);
            List<Point> hullPoints = new List<Point>(initialPoints.Count)
            {
                initialPoints[iMin]     // Add minimum point
            };
            hullPoints.AddRange(sortQuery);          // Add the sorted points.

            int M = 0;
            for (int i = 1, N = hullPoints.Count; i < N; i++)
            {
                bool keepNewPoint = true;
                if (M == 0)
                {
                    // Find at least one point not coincident with points[0]
                    keepNewPoint = !NearlyEqual(hullPoints[0], hullPoints[i]);
                }
                else
                {
                    while (true)
                    {
                        RemovalFlag flag = WhichToRemoveFromBoundary(hullPoints[M - 1], hullPoints[M], hullPoints[i]);
                        if (flag == RemovalFlag.None)
                        {
                            break;
                        }
                        else if (flag == RemovalFlag.MidPoint)
                        {
                            if (M > 0)
                            {
                                M--;
                            }

                            if (M == 0)
                            {
                                break;
                            }
                        }
                        else if (flag == RemovalFlag.EndPoint)
                        {
                            keepNewPoint = false;
                            break;
                        }
                        else
                        {
                            throw new GrahamScanConvexHullException("Unknown RemovalFlag");
                        }
                    }
                }
                if (keepNewPoint)
                {
                    M++;
                    Swap(hullPoints, M, i);
                }
            }

            // points[M] is now the last point in the boundary.  Remove the remainder.
            hullPoints.RemoveRange(M + 1, hullPoints.Count - M - 1);

            DentInclusion dentInclusion = new DentInclusion();
            List<Point> notIncludedPoints = dentInclusion.NotIncludedPointsInConvexHull(ref initialPoints, ref hullPoints);

            return dentInclusion.AddPointsToHull(ref hullPoints, ref notIncludedPoints, maxDistBetweenPoints, maxDepth).ToList();
        }

        private void Swap<T>(IList<T> list, int i, int j)
        {
            if (i != j)
            {
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public double RelativeTolerance => 1e-10;

        public bool NearlyEqual(Point a, Point b)
        {
            return NearlyEqual(a.X, b.X) && NearlyEqual(a.Y, b.Y);
        }

        public bool NearlyEqual(double a, double b)
        {
            return NearlyEqual(a, b, RelativeTolerance);
        }

        public bool NearlyEqual(double a, double b, double epsilon)
        {
            // See here: http://floating-point-gui.de/errors/comparison/
            if (a == b)
            { // shortcut, handles infinities
                return true;
            }

            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);
            double sum = absA + absB;
            if (diff < 4 * double.Epsilon || sum < 4 * double.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return true;
            }

            // use relative error
            return diff / (absA + absB) < epsilon;
        }

        private double CCW(Point p1, Point p2, Point p3)
        {
            // Compute (p2 - p1) X (p3 - p1)
            double cross1 = (p2.X - p1.X) * (p3.Y - p1.Y);
            double cross2 = (p2.Y - p1.Y) * (p3.X - p1.X);
            if (NearlyEqual(cross1, cross2))
            {
                return 0;
            }

            return cross1 - cross2;
        }

        private enum RemovalFlag
        {
            None,
            MidPoint,
            EndPoint
        };

        private RemovalFlag WhichToRemoveFromBoundary(Point p1, Point p2, Point p3)
        {
            double cross = CCW(p1, p2, p3);
            if (cross < 0)
            {
                // Remove p2
                return RemovalFlag.MidPoint;
            }

            if (cross > 0)
            {
                // Remove none.
                return RemovalFlag.None;
            }
            // Check for being reversed using the dot product off the difference vectors.
            double dotp = ((p3.X - p2.X) * (p2.X - p1.X)) + ((p3.Y - p2.Y) * (p2.Y - p1.Y));
            if (NearlyEqual(dotp, 0.0))
            {
                // Remove p2
                return RemovalFlag.MidPoint;
            }

            if (dotp < 0)
            {
                // Remove p3
                return RemovalFlag.EndPoint;
            }
            else
            {
                // Remove p2
                return RemovalFlag.MidPoint;
            }
        }
    }
}