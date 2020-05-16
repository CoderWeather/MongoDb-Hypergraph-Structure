using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HyperGraphSharp.Controls
{
	public class HyperEdgeControl : Control, IDisposable
	{
		#region Static Constructor

		static HyperEdgeControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperEdgeControl),
				new FrameworkPropertyMetadata(typeof(HyperEdgeControl)));
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
		}

		#endregion

		#region Public Constructor

		#endregion

		#region Public Properties

		public VertexControl[] Vertices
		{
			get => (VertexControl[]) GetValue(VerticesProperty);
			set => SetValue(VerticesProperty, value);
		}

		public Point[]? RoutePoints
		{
			get => (Point[]) GetValue(RoutePointsProperty);
			set => SetValue(RoutePointsProperty, value);
		}

		public object? HyperEdge
		{
			get => (object?) GetValue(HyperEdgeProperty);
			set => SetValue(HyperEdgeProperty, value);
		}

		public double StrokeThickness
		{
			get => (double) GetValue(StrokeThicknessProperty);
			set => SetValue(StrokeThicknessProperty, value);
		}

		#endregion

		#region DependencyProperties

		public static readonly DependencyProperty VerticesProperty = DependencyProperty.Register(
			"Vertices", typeof(VertexControl[]), typeof(HyperEdgeControl),
			new PropertyMetadata(default(IEnumerable<VertexControl>)));

		public static readonly DependencyProperty RoutePointsProperty = DependencyProperty.Register(
			"RoutePoints", typeof(Point[]), typeof(HyperEdgeControl), new PropertyMetadata(default(Point[])));

		public static readonly DependencyProperty HyperEdgeProperty = DependencyProperty.Register(
			"HyperEdge", typeof(object), typeof(HyperEdgeControl), new PropertyMetadata(default(object?)));

		public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(
			typeof(HyperEdgeControl), new UIPropertyMetadata(1.0));

		#endregion
	}
}