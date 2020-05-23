using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HyperGraphSharp.Controls;
using HyperGraphSharp.Extensions;

namespace HyperGraphSharp.Converters
{
	public class EdgeRouteToPathConverter : IMultiValueConverter
	{
		private static Point CalculateAttachPoint(Point s, Size sourceSize, Point t)
		{
			var sides = new double[4];
			sides[0] = (s.X - sourceSize.Width / 2.0 - t.X) / (s.X - t.X);
			sides[1] = (s.Y - sourceSize.Height / 2.0 - t.Y) / (s.Y - t.Y);
			sides[2] = (s.X + sourceSize.Width / 2.0 - t.X) / (s.X - t.X);
			sides[3] = (s.Y + sourceSize.Height / 2.0 - t.Y) / (s.Y - t.Y);

			double fi = 0;
			for (var i = 0; i < 4; i++)
				if (sides[i] <= 1)
					fi = Math.Max(fi, sides[i]);

			return t + fi * (s - t);
		}

		private static Point GetAttachPoint(Point s, Size size, Point t)
		{
			var angle = s.AngleWith(t);

			var halfWidth = size.Width / 2d;
			var halfHeight = size.Height / 2d;

			var x = s.X + halfWidth * Math.Cos(angle);
			var y = s.Y + halfHeight * -Math.Sin(angle);

			return new Point(x, y);
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var vertexControls = values[0] != DependencyProperty.UnsetValue
				? (VertexControl[]) values[0]
				: null;

			Debug.Assert(vertexControls != null);

			var verticesInfo = vertexControls
			   .Select(vc =>
				{
					vc.InvalidateMeasure();
					var x = HyperGraphCanvas.GetX(vc);
					var y = HyperGraphCanvas.GetY(vc);
					return new
					{
						Size = new Size(vc.ActualWidth, vc.ActualHeight),
						Center = new Point
						{
							X = double.IsNaN(x) ? 0.0 : x,
							Y = double.IsNaN(y) ? 0.0 : y
						}
					};
				})
			   .ToArray();

			var hyperEdgeCenter = new Point
			{
				X = verticesInfo.Average(p => p.Center.X),
				Y = verticesInfo.Average(p => p.Center.Y)
			};

			// var origo = new Point(0d, 0d);

			// var attachPointsToCenter = verticesInfo
			//    .Select(vi => GetAttachPoint(vi.Center, vi.Size, origo))
			//    .ToArray();

			// var attachPoints = verticesInfo
			//    .Select(vi => GetAttachPoint(vi.Center, vi.Size, hyperEdgeCenter))
			// 	// .Select(vi => CalculateAttachPoint(vi.Center, vi.Size, hyperEdgeCenter))
			//    .ToArray();

			// var newHyperEdgeCenter = new Point
			// {
			// 	X = attachPointsToCenter.Average(p => p.X),
			// 	Y = attachPointsToCenter.Average(p => p.Y)
			// };

			// var lines = attachPointsToCenter
			//    .SelectMany(p => new[]
			// 	{
			// 		new LineSegment(p, false),
			// 		new LineSegment(newHyperEdgeCenter, true),
			// 	})
			//    .ToArray();

			var linesToVertexCenters = verticesInfo
			   .SelectMany(vi => new[]
				{
					new LineSegment(vi.Center, false),
					new LineSegment(hyperEdgeCenter, true),
				})
			   .ToArray();

			// var tracingFigures = verticesInfo
			//    .Select(vi => new LineSegment(vi.Center, true));

			return new PathFigureCollection(new[]
			{
				// new PathFigure(hyperEdgeCenter, lines, true),
				new PathFigure(hyperEdgeCenter, linesToVertexCenters, false), 
				// new PathFigure(verticesInfo.First().Center, tracingFigures, true),
			});
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			// ignore
			throw new NotImplementedException();
		}
	}
}