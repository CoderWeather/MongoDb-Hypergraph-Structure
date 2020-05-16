using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Controls
{
    public partial class HyperGraphLayout
    {
        protected IDictionary<Vertex, Point> RelativeVertexPositions =>
            VertexControls.ToDictionary(
                pair => pair.Key,
                pair => GetRelativePosition(pair.Value));

        protected IDictionary<Vertex, Point> LatestVertexPositions =>
            VertexControls.ToDictionary(
                pair => pair.Key,
                pair =>
                {
                    var (_, vertexControl) = pair;
                    var x = GetX(vertexControl);
                    var y = GetY(vertexControl);
                    return new Point
                    {
                        X = double.IsNaN(x) ? 0.0 : x,
                        Y = double.IsNaN(y) ? 0.0 : y
                    };
                });

        private IDictionary<Vertex, Size> ActualVertexSizes =>
            GetLatestVertexSizes();

        private IDictionary<Vertex, Size> GetLatestVertexSizes()
        {
            if (IsMeasureValid is false)
                Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var vertexSizes = new Dictionary<Vertex, Size>(VertexControls.Count);

            foreach (var (vertex, vControl) in VertexControls)
            {
                vertexSizes[vertex] = new Size
                {
                    Width = vControl.ActualWidth, Height = vControl.ActualHeight
                };
            }

            return vertexSizes;
        }

        private IDictionary<Vertex, Point> GetRelativeVertexPositions() =>
            VertexControls.ToDictionary(
                pair => pair.Key,
                pair => GetRelativePosition(pair.Value));

        private Point GetRelativePosition(FrameworkElement vc) =>
            GetRelativePosition(vc, this);

        private static Point GetRelativePosition(FrameworkElement vc, UIElement relativeTo) =>
            vc.TranslatePoint(new Point
                {
                    X = vc.ActualWidth / 2.0,
                    Y = vc.ActualHeight / 2.0
                },
                relativeTo);
    }
}