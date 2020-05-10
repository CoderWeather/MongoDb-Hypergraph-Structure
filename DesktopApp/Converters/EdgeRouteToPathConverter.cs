using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DesktopApp.Converters
{
	public class EdgeRouteToPathConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			#region Get the inputs

			//get the route points
			var routePoints = values[0] != DependencyProperty.UnsetValue ? (Point[]) values[1] : null;

			#endregion

			var hasRouteInfo = routePoints != null && routePoints.Length > 0;

			// Create the path
			//
			// var p1 = GraphConverterUtils.CalculateAttachPoint(sourcePos, sourceSize,
			// 	hasRouteInfo ? routePoints[0] : targetPos);
			// var p2 = GraphConverterUtils.CalculateAttachPoint(targetPos, targetSize,
			// 	hasRouteInfo ? routePoints[routePoints.Length - 1] : sourcePos);

			Debug.Assert(routePoints != null, nameof(routePoints) + " != null");

			// var segments = new PathSegment[1 + (hasRouteInfo ? routePoints.Length : 0)];
			// if (hasRouteInfo)
			// 	//append route points
			// 	for (var i = 0; i < routePoints.Length; i++)
			// 		segments[i] = new LineSegment(routePoints[i], true);
			//
			// var pLast = hasRouteInfo ? routePoints[^1] : p1;
			// var v = pLast - p2;
			// v = v / v.Length * 5;
			// var n = new Vector(-v.Y, v.X) * 0.3;
			//
			// segments[^1] = new LineSegment(p2 + v, true);
			//
			// var pfc = new PathFigureCollection(2);
			// pfc.Add(new PathFigure(p1, segments, false));
			// pfc.Add(new PathFigure(p2,
			// 	new PathSegment[]
			// 	{
			// 		new LineSegment(p2 + v - n, true),
			// 		new LineSegment(p2 + v + n, true)
			// 	}, true));
			var hyperEdgeCenterPoint = routePoints.Last();
			var pfc = new PathFigureCollection(routePoints.Length);
			if (hasRouteInfo)
				pfc.Add(new PathFigure(hyperEdgeCenterPoint,
					routePoints
					   .Take(routePoints.Length - 1)
					   .Select(p => new LineSegment(p, true)),
					false));

			return pfc;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}