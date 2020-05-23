using System.Windows;

namespace HyperGraphSharp.Controls
{
	public class PositionChangedEventArgs : RoutedEventArgs
	{
		public PositionChangedEventArgs(RoutedEvent evt, object source, double deltaX, double deltaY)
			: base(evt, source)
		{
			DeltaX = deltaX;
			DeltaY = deltaY;
		}

		public double DeltaX { get; }
		public double DeltaY { get; }
	}
}