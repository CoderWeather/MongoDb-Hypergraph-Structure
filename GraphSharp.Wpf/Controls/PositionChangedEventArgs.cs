using System.Windows;

namespace GraphSharp.Controls
{
	public class PositionChangedEventArgs : RoutedEventArgs
	{
		public PositionChangedEventArgs(RoutedEvent evt, object source, double xChange, double yChange)
			: base(evt, source)
		{
			XChange = xChange;
			YChange = yChange;
		}

		public double XChange { get; }
		public double YChange { get; }
	}
}