using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Controls
{
	public class VertexControl : Control, IDisposable
	{
		#region Static Constructor

		static VertexControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VertexControl),
				new FrameworkPropertyMetadata(typeof(VertexControl)));
		}

		#endregion

		#region Public Constructor

		public VertexControl()
		{
			HyperEdges = new List<HyperEdgeControl>();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
		}

		#endregion

		#region Public Properties

		public object? Vertex
		{
			get => GetValue(VertexProperty);
			set => SetValue(VertexProperty, value);
		}

		public HyperGraphCanvas RootHyperGraphCanvas
		{
			get => (HyperGraphCanvas) GetValue(RootHyperGraphCanvasProperty);
			set => SetValue(RootHyperGraphCanvasProperty, value);
		}

		public List<HyperEdgeControl> HyperEdges
		{
			get => (List<HyperEdgeControl>) GetValue(HyperEdgesProperty);
			set => SetValue(HyperEdgesProperty, value);
		}

		public Point CenterPoint
		{
			get => (Point) GetValue(CenterPointProperty);
			set => SetValue(CenterPointProperty, value);
		}

		#endregion

		#region DependencyProperties

		public static readonly DependencyProperty VertexProperty = DependencyProperty.Register(
			nameof(Vertex), typeof(object), typeof(VertexControl), new PropertyMetadata(default(Vertex)));

		public static readonly DependencyProperty RootHyperGraphCanvasProperty = DependencyProperty.Register(
			"RootHyperGraphCanvas", typeof(HyperGraphCanvas), typeof(VertexControl), new PropertyMetadata(default(Canvas)));

		public static readonly DependencyProperty HyperEdgesProperty = DependencyProperty.Register(
			"HyperEdges", typeof(List<HyperEdgeControl>), typeof(VertexControl),
			new PropertyMetadata(default(HyperEdgeControl[])));

		public static readonly DependencyProperty CenterPointProperty = DependencyProperty.Register(
			"CenterPoint", typeof(Point), typeof(VertexControl), new PropertyMetadata(default(Point)));

		#endregion
	}
}