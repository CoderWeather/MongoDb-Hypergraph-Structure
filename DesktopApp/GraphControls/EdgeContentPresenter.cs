using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopApp.GraphControls
{
	public class EdgeContentPresenter : ContentPresenter
	{
		public EdgeContentPresenter()
		{
			LayoutUpdated += EdgeContentPresenter_LayoutUpdated;
		}

		private static HyperEdgeControl? GetHyperEdgeControl(DependencyObject parent)
		{
			while (parent != null)
				if (parent is HyperEdgeControl control)
					return control;
				else
					parent = VisualTreeHelper.GetParent(parent);
			return null;
		}

		private static double GetAngleBetweenPoints(Point point1, Point point2)
		{
			return Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);
		}

		private static double GetDistanceBetweenPoints(Point point1, Point point2)
		{
			return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
		}

		private static double GetLabelDistance(double edgeLength)
		{
			return edgeLength / 2;
		}

		private void EdgeContentPresenter_LayoutUpdated(object? sender, EventArgs e)
		{
			if (!IsLoaded)
				return;

			var hyperEdgeControl = GetHyperEdgeControl(VisualParent);
			if (hyperEdgeControl == null)
				return;

			var p = hyperEdgeControl.RoutePoints.Last();
			Arrange(new Rect(p, DesiredSize));

			// var source = edgeControl.Source;
			// var p1 = new Point(HyperGraphCanvas.GetX(source), HyperGraphCanvas.GetY(source));
			// var target = edgeControl.Target;
			// var p2 = new Point(HyperGraphCanvas.GetX(target), HyperGraphCanvas.GetY(target));

			// double edgeLength;
			// var routePoints = hyperEdgeControl.RoutePoints;

			// the edge is a single segment (p1,p2)
			// edgeLength = GetLabelDistance(GetDistanceBetweenPoints(p1, p2));
			// else
			// {
			// // the edge has one or more segments
			// // compute the total length of all the segments
			// edgeLength = 0;
			// for (var i = 0; i <= routePoints.Length; ++i)
			// 	if (i == 0)
			// 		edgeLength += GetDistanceBetweenPoints(p1, routePoints[0]);
			// 	else if (i == routePoints.Length)
			// 		edgeLength += GetDistanceBetweenPoints(routePoints[routePoints.Length - 1], p2);
			// 	else
			// 		edgeLength += GetDistanceBetweenPoints(routePoints[i - 1], routePoints[i]);
			// // find the line segment where the half distance is located
			// edgeLength = GetLabelDistance(edgeLength);
			// var newP1 = p1;
			// var newP2 = p2;
			// for (var i = 0; i <= routePoints.Length; ++i)
			// {
			// 	double lengthOfSegment;
			// 	if (i == 0)
			// 		lengthOfSegment = GetDistanceBetweenPoints(newP1 = p1, newP2 = routePoints[0]);
			// 	else if (i == routePoints.Length)
			// 		lengthOfSegment = GetDistanceBetweenPoints(newP1 = routePoints[routePoints.Length - 1], newP2 = p2);
			// 	else
			// 		lengthOfSegment = GetDistanceBetweenPoints(newP1 = routePoints[i - 1], newP2 = routePoints[i]);
			// 	if (lengthOfSegment >= edgeLength)
			// 		break;
			// 	edgeLength -= lengthOfSegment;
			// }
			// // redefine our edge points
			// p1 = newP1;
			// p2 = newP2;
			// }

			// align the point so that it  passes through the center of the label content
			// var p = p1;
			// var desiredSize = DesiredSize;
			// p.Offset(-desiredSize.Width / 2, -desiredSize.Height / 2);
			//
			// // move it "edgLength" on the segment
			// var angleBetweenPoints = GetAngleBetweenPoints(p1, p2);
			// //p.Offset(edgeLength * Math.Cos(angleBetweenPoints), -edgeLength * Math.Sin(angleBetweenPoints));
			// float x = 12.5f, y = 12.5f;
			// var sin = Math.Sin(angleBetweenPoints);
			// var cos = Math.Cos(angleBetweenPoints);
			// var sign = sin * cos / Math.Abs(sin * cos);
			// p.Offset(x * sin * sign + edgeLength * cos, y * cos * sign - edgeLength * sin);
			// Arrange(new Rect(p, DesiredSize));
		}
	}
}