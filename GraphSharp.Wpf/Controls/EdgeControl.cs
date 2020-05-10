using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using GraphSharp.Helpers;

namespace GraphSharp.Controls
{
	public class EdgeControl : Control, IPoolObject, IDisposable
	{
		static EdgeControl()
		{
			//override the StyleKey
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EdgeControl),
				new FrameworkPropertyMetadata(typeof(EdgeControl)));
		}

		#region IDisposable Members

		public void Dispose() => Disposing?.Invoke(this);

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
			typeof(VertexControl),
			typeof(EdgeControl),
			new UIPropertyMetadata(null));

		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target",
			typeof(VertexControl),
			typeof(EdgeControl),
			new UIPropertyMetadata(null));

		public static readonly DependencyProperty RoutePointsProperty = DependencyProperty.Register("RoutePoints",
			typeof(Point[]),
			typeof(EdgeControl),
			new UIPropertyMetadata(
				null));

		public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register("Edge", typeof(object),
			typeof(EdgeControl),
			new PropertyMetadata(null));

		public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(
			typeof(EdgeControl),
			new UIPropertyMetadata(2.0));

		#endregion

		#region Properties

		public VertexControl? Source
		{
			get => (VertexControl) GetValue(SourceProperty);
			internal set => SetValue(SourceProperty, value);
		}

		public VertexControl? Target
		{
			get => (VertexControl) GetValue(TargetProperty);
			internal set => SetValue(TargetProperty, value);
		}

		public Point[]? RoutePoints
		{
			get => (Point[]) GetValue(RoutePointsProperty);
			set => SetValue(RoutePointsProperty, value);
		}

		public object? Edge
		{
			get => GetValue(EdgeProperty);
			set => SetValue(EdgeProperty, value);
		}

		public double StrokeThickness
		{
			get => (double) GetValue(StrokeThicknessProperty);
			set => SetValue(StrokeThicknessProperty, value);
		}

		#endregion

		#region IPoolObject Members

		public void Reset()
		{
			Edge = null;
			RoutePoints = null;
			Source = null;
			Target = null;
		}

		public void Terminate()
		{
			//nothing to do, there are no unmanaged resources
		}

		public event DisposingHandler Disposing;

		#endregion
	}
}