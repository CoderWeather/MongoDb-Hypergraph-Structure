using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppLib.GraphModels;

namespace DesktopApp.GraphControls
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
			VertexControls.ToDictionary(
				pair => pair.Key,
				pair =>
				{
					if (IsMeasureValid is false)
						Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
					InvalidateMeasure();
					UpdateLayout();
					var (_, vControl) = pair;
					return new Size
					{
						Width = vControl.ActualWidth,
						Height = vControl.ActualHeight
					};
				}
			);

		private IDictionary<Vertex, Point> GetVertexPositions()
		{
			return VertexControls.ToDictionary(
				pair => pair.Key,
				pair => GetRelativePosition(pair.Value));
		}

		private Point GetRelativePosition(FrameworkElement vc)
		{
			return GetRelativePosition(vc, this);
		}

		private static Point GetRelativePosition(FrameworkElement vc, UIElement relativeTo)
		{
			return vc.TranslatePoint(new Point
				{
					X = vc.ActualWidth / 2.0,
					Y = vc.ActualHeight / 2.0
				},
				relativeTo);
		}
	}
}