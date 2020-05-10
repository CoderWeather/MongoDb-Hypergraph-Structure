using System;
using System.Windows;

namespace DesktopApp.Converters
{
	public static class GraphConverterUtils
	{
		public static Point CalculateAttachPoint(Point source, Size sourceSize, Point target)
		{
			var sides = new double[4];
			sides[0] = (source.X - sourceSize.Width / 2.0 - target.X) / (source.X - target.X);
			sides[1] = (source.Y - sourceSize.Height / 2.0 - target.Y) / (source.Y - target.Y);
			sides[2] = (source.X + sourceSize.Width / 2.0 - target.X) / (source.X - target.X);
			sides[3] = (source.Y + sourceSize.Height / 2.0 - target.Y) / (source.Y - target.Y);

			double fi = 0;
			for (var i = 0; i < 4; i++)
				if (sides[i] <= 1)
					fi = Math.Max(fi, sides[i]);

			return target + fi * (source - target);
		}
	}
}