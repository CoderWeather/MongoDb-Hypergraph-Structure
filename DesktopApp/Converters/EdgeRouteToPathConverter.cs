using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HyperGraphSharp.Controls;

namespace DesktopApp.Converters
{
    public class EdgeRouteToPathConverter : IMultiValueConverter
    {
        private static double GetAngleBetweenPoints(Point point1, Point point2) => 
            Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);

        private static double GetDistanceBetweenPoints(Point point1, Point point2) => 
            Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));

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

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var vertexControls = values[0] != DependencyProperty.UnsetValue
                ? (VertexControl[]) values[0]
                : null;

            Debug.Assert(vertexControls != null);

            var verticesInfo = vertexControls
                .Select(vc => new
                {
                    Size = new Size(vc.ActualWidth, vc.ActualHeight),
                    Center = new Point
                    {
                        X = double.IsNaN(HyperGraphCanvas.GetX(vc)) ? 0.0 : HyperGraphCanvas.GetX(vc),
                        Y = double.IsNaN(HyperGraphCanvas.GetY(vc)) ? 0.0 : HyperGraphCanvas.GetY(vc)
                    }
                })
                .ToArray();

            var hyperEdgeCenter = new Point
            {
                X = verticesInfo.Average(p => p.Center.X),
                Y = verticesInfo.Average(p => p.Center.Y)
            };
            var attachPoints = verticesInfo
                // .Select(vi => GetAttachPoint(vi.Center, vi.Size, hyperEdgeCenter))
                .Select(vi => CalculateAttachPoint(vi.Center, vi.Size, hyperEdgeCenter))
                // .Select(vi => vi.Center)
                // .Append(hyperEdgeCenter)
                .ToArray();

            var lines = attachPoints
                .SelectMany(p => new[]
                {
                    new LineSegment(p, true),
                    new LineSegment(hyperEdgeCenter, true),
                })
                .ToArray();

            var tracingFigures = verticesInfo
                .Select(vi => new LineSegment(vi.Center, true));

            return new PathFigureCollection(new[]
            {
                new PathFigure(hyperEdgeCenter, lines, false),
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