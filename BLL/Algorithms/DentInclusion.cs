using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Algorithms
{
    public class DentInclusion
    {
		public LinkedList<Point> AddPointsToHull(ref List<Point> inHull, ref List<Point> notInHull, double maxDistBetweenPoints, double maxDepth)
		{
			LinkedList<Point> linkedListPoints = ReturnLinked(ref inHull);
			int i = 0;
			double dist;
			while (true)
			{
				for (LinkedListNode<Point> node = linkedListPoints.First; node != null; node = node.Next)
				{
					if (node.Next == null)
					{
						if (DistanceBetweenTwoPoints(node.Value, linkedListPoints.First.Value) <= maxDistBetweenPoints)
						{
							node.Value.RightFlag = true;
							linkedListPoints.First.Value.LeftFlag = true;
						}
						else
						{
							Point middle = FindMiddlePointBetweenTwoPoints(node.Value, linkedListPoints.First.Value);
							Point addedPoint = null;
							double newdist = 999;
							i = 0;
							foreach (var point in notInHull)
							{
								if (DistanceBetweenTwoPoints(middle, point) <= maxDepth)
								{
									dist = DistanceBetweenTwoPoints(middle, point);
									if (dist < newdist)
									{
										newdist = dist;
										addedPoint = new Point()
										{
											X = point.X,
											Y = point.Y,
											Z = point.Z,
											LeftFlag = point.LeftFlag,
											RightFlag = point.RightFlag
										};
										i = notInHull.IndexOf(point);
									}
								}
							}
							if (addedPoint != null)
							{
								notInHull.RemoveAt(i);
								linkedListPoints.AddAfter(node, addedPoint);
							}
							else
							{
								node.Value.RightFlag = true;
								linkedListPoints.First.Value.LeftFlag = true;
							}
						}
					}
					if (node.Next != null && (!node.Value.LeftFlag || !node.Value.RightFlag))
					{
						if (DistanceBetweenTwoPoints(node.Value, node.Next.Value) <= maxDistBetweenPoints)
						{
							node.Value.RightFlag = true;
							node.Next.Value.LeftFlag = true;
						}
						else
						{
							Point middle = FindMiddlePointBetweenTwoPoints(node.Value, node.Next.Value);
							Point addedPoint = null;
							double newdist = 999;
							i = 0;
							foreach (var point in notInHull)
							{
								if (DistanceBetweenTwoPoints(middle, point) <= maxDepth)
								{
									dist = DistanceBetweenTwoPoints(middle, point);
									if (dist < newdist)
									{
										newdist = dist;
										addedPoint = new Point()
										{
											X = point.X,
											Y = point.Y,
											Z = point.Z,
											LeftFlag = point.LeftFlag,
											RightFlag = point.RightFlag
										};
										i = notInHull.IndexOf(point);
									}
								}
							}
							if (addedPoint != null)
							{
								notInHull.RemoveAt(i);
								linkedListPoints.AddAfter(node, addedPoint);
							}
							else
							{
								node.Value.RightFlag = true;
								node.Next.Value.LeftFlag = true;
							}
						}
					}
				}
				if (CheckedAllNodes(ref linkedListPoints))
				{
					break;
				}
			}
			return linkedListPoints;
		}

		/// <summary>
		/// Checks all points for adding new points between them to add new bulge to convex hull
		/// </summary>
		/// <param name="linkedlist">List of points in convex hull with bulges.</param>
		/// <returns> Returns true if all points were checked and there no reason to add more points, false - opposite</returns>
		private bool CheckedAllNodes(ref LinkedList<Point> hullPointsWithBulges)
		{
			foreach (var point in hullPointsWithBulges)
			{
				if (!point.RightFlag || !point.LeftFlag)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Finding the point between points where the max distance is exaggerated
		/// </summary>
		/// <param name="a">First point</param>
		/// <param name="b">Second point</param>
		/// <returns> Returns the point between points where the max distance is exaggerated</returns>
		private Point FindMiddlePointBetweenTwoPoints(Point a, Point b)
		{
			double x = (a.X + b.X) * 0.5;
			double y = (a.Y + b.Y) * 0.5;
			return new Point() { X = x, Y = y };
		}

		/// <summary>
		/// Gets the distance between two consecutive points the in the convex hull
		/// </summary>
		/// <param name="a">First point</param>
		/// <param name="b">Second point</param>
		/// <returns> Returns the distance between two consecutive points the in the convex hull</returns>
		private double DistanceBetweenTwoPoints(Point a, Point b)
		{
			return Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
		}

		/// <summary>
		/// Converts List of the points in the convex hull to List
		/// </summary>
		/// <param name="inHull">List of points in the convex hull</param>
		/// <returns> Returns LinkedList of points in the convex hull</returns>
		private LinkedList<Point> ReturnLinked(ref List<Point> inHull)
		{
			LinkedList<Point> linkedListPoints = new LinkedList<Point>(inHull.ToArray());
			return linkedListPoints;
		}

		/// <summary>
		/// Gets List of the points that are not included in the convex hull
		/// </summary>
		/// <param name="allPoints">List of all points</param>
		/// <param name="inHull">List of points in the convex hull</param>
		/// <returns> Returns List of the points that are not included in the convex hull</returns>
		public List<Point> NotIncludedPointsInConvexHull(ref List<Point> allPoints, ref List<Point> inHull)
		{
			List<Point> notIncludedPoints = new List<Point>();
			foreach (var allpoint in allPoints)
			{
				if (!inHull.Contains(allpoint))
				{
					notIncludedPoints.Add(allpoint);
				}
			}
			return notIncludedPoints;
		}
	}
}
