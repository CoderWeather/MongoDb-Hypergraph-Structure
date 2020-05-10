using System;
using System.Windows;
using System.Windows.Controls;
using GraphSharp.Controls;

namespace DesktopApp.GraphControls
{
	public class HyperGraphCanvas : Panel
	{
		#region Static Constructor

		static HyperGraphCanvas()
		{
			TranslationProperty = TranslationPropertyKey.DependencyProperty;
		}

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty XProperty = DependencyProperty.Register(
			"X", typeof(double), typeof(HyperGraphCanvas),
			new FrameworkPropertyMetadata(double.NaN,
				FrameworkPropertyMetadataOptions.AffectsMeasure |
				FrameworkPropertyMetadataOptions.AffectsArrange |
				FrameworkPropertyMetadataOptions.AffectsRender |
				FrameworkPropertyMetadataOptions.AffectsParentMeasure |
				FrameworkPropertyMetadataOptions.AffectsParentArrange |
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				X_PropertyChanged));

		public double X
		{
			get => (double) GetValue(XProperty);
			set => SetValue(XProperty, value);
		}

		public static readonly DependencyProperty YProperty = DependencyProperty.Register(
			"Y", typeof(double), typeof(HyperGraphCanvas),
			new FrameworkPropertyMetadata(double.NaN,
				FrameworkPropertyMetadataOptions.AffectsMeasure |
				FrameworkPropertyMetadataOptions.AffectsArrange |
				FrameworkPropertyMetadataOptions.AffectsRender |
				FrameworkPropertyMetadataOptions.AffectsParentMeasure |
				FrameworkPropertyMetadataOptions.AffectsParentArrange |
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				Y_PropertyChanged));

		public double Y
		{
			get => (double) GetValue(YProperty);
			set => SetValue(YProperty, value);
		}

		public static readonly DependencyProperty TranslationProperty;

		protected static readonly DependencyPropertyKey TranslationPropertyKey =
			DependencyProperty.RegisterReadOnly("Translation", typeof(Vector), typeof(HyperGraphCanvas),
				new UIPropertyMetadata(new Vector()));

		public Vector Translation
		{
			get => (Vector) GetValue(TranslationProperty);
			protected set => SetValue(TranslationPropertyKey, value);
		}

		#endregion

		#region Attached Dependency Property Registrations

		private static void X_PropertyChanged(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs eventArgs)
		{
			var deltaX = (double) eventArgs.NewValue - (double) eventArgs.OldValue;
			PositionChanged(dependencyObject, deltaX, 0);
		}

		private static void Y_PropertyChanged(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs eventArgs)
		{
			var deltaY = (double) eventArgs.NewValue - (double) eventArgs.OldValue;
			PositionChanged(dependencyObject, 0, deltaY);
		}

		private static void PositionChanged(DependencyObject dependencyObject, double deltaX, double deltaY)
		{
			if (dependencyObject is UIElement uiElement)
				uiElement.RaiseEvent(new PositionChangedEventArgs(PositionChangedEvent, uiElement, deltaX, deltaY));
		}

		#endregion

		#region Attached Properties

		[AttachedPropertyBrowsableForChildren]
		public static double GetX(DependencyObject obj)
		{
			return (double) obj.GetValue(XProperty);
		}

		public static void SetX(DependencyObject obj, double value)
		{
			obj.SetValue(XProperty, value);
		}

		[AttachedPropertyBrowsableForChildren]
		public static double GetY(DependencyObject obj)
		{
			return (double) obj.GetValue(YProperty);
		}

		public static void SetY(DependencyObject obj, double value)
		{
			obj.SetValue(YProperty, value);
		}

		#endregion

		#region Attached Routed Events

		public static readonly RoutedEvent PositionChangedEvent =
			EventManager.RegisterRoutedEvent("PositionChanged", RoutingStrategy.Bubble,
				typeof(PositionChangedEventHandler), typeof(HyperGraphCanvas));

		public static void AddPositionChangedHandler(DependencyObject d, RoutedEventHandler handler)
		{
			if (d is UIElement e)
				e.AddHandler(PositionChangedEvent, handler);
		}

		public static void RemovePositionChangedHandler(DependencyObject d, RoutedEventHandler handler)
		{
			if (d is UIElement e)
				e.RemoveHandler(PositionChangedEvent, handler);
		}

		#endregion

		#region Measure & Arrange

		private Point _topLeft;

		private Point _bottomRight;

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var translate = new Vector(-_topLeft.X, -_topLeft.Y);
			var graphSize = _bottomRight - _topLeft;

			if (double.IsNaN(graphSize.X) || double.IsNaN(graphSize.Y) ||
				double.IsInfinity(graphSize.X) || double.IsInfinity(graphSize.Y))
				translate = new Vector(0, 0);

			Translation = translate;

			graphSize = InternalChildren.Count > 0
				? new Vector(double.NegativeInfinity, double.NegativeInfinity)
				: new Vector(0, 0);

			//translate with the topLeft
			foreach (UIElement? child in InternalChildren)
			{
				if (child is null) continue;

				var x = GetX(child!);
				var y = GetY(child!);
				if (double.IsNaN(x) || double.IsNaN(y))
				{
					x = double.IsNaN(x) ? translate.X : x;
					y = double.IsNaN(y) ? translate.Y : y;
				}
				else
				{
					x += translate.X;
					y += translate.Y;

					x -= child!.DesiredSize.Width * 0.5;
					y -= child!.DesiredSize.Height * 0.5;
				}

				child.Arrange(new Rect(new Point(x, y), child.DesiredSize));

				graphSize.X = Math.Max(0, Math.Max(graphSize.X, x + child.DesiredSize.Width));
				graphSize.Y = Math.Max(0, Math.Max(graphSize.Y, y + child.DesiredSize.Height));
			}

			return new Size(graphSize.X, graphSize.Y);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			_topLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
			_bottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);

			foreach (UIElement? child in InternalChildren)
			{
				if (child is null) continue;

				//measure the child
				child.Measure(constraint);

				//get the position of the vertex
				var left = GetX(child);
				var top = GetY(child);

				var halfWidth = child.DesiredSize.Width * 0.5;
				var halfHeight = child.DesiredSize.Height * 0.5;

				if (double.IsNaN(left) || double.IsNaN(top))
				{
					left = halfWidth;
					top = halfHeight;
				}

				//get the top left corner point
				_topLeft.X = Math.Min(_topLeft.X, left - halfWidth - Origo.X);
				_topLeft.Y = Math.Min(_topLeft.Y, top - halfHeight - Origo.Y);

				//calculate the bottom right corner point
				_bottomRight.X = Math.Max(_bottomRight.X, left + halfWidth - Origo.X);
				_bottomRight.Y = Math.Max(_bottomRight.Y, top + halfHeight - Origo.Y);
			}

			var graphSize = (Size) (_bottomRight - _topLeft);
			graphSize.Width = Math.Max(0, graphSize.Width);
			graphSize.Height = Math.Max(0, graphSize.Height);

			if (double.IsNaN(graphSize.Width) || double.IsNaN(graphSize.Height) ||
				double.IsInfinity(graphSize.Width) || double.IsInfinity(graphSize.Height))
				return new Size(0, 0);

			return graphSize;
		}

		#endregion

		#region Working Region

		/// <summary>
		///     Gets or sets the virtual origo of the canvas.
		/// </summary>
		public Point Origo
		{
			get => (Point) GetValue(OrigoProperty);
			set => SetValue(OrigoProperty, value);
		}

		public static readonly DependencyProperty OrigoProperty =
			DependencyProperty.Register("Origo", typeof(Point), typeof(HyperGraphCanvas),
				new FrameworkPropertyMetadata(
					new Point(),
					FrameworkPropertyMetadataOptions.AffectsMeasure |
					FrameworkPropertyMetadataOptions.AffectsArrange |
					FrameworkPropertyMetadataOptions.AffectsRender |
					FrameworkPropertyMetadataOptions.AffectsParentMeasure |
					FrameworkPropertyMetadataOptions.AffectsParentArrange));

		#endregion
	}
}