using System.Windows;

namespace GraphSharp.Algorithms.OverlapRemoval
{
	/// <summary>
	///     A System.Windows.Rect egy struktъra, ezйrt a heap-en tбrolуdik. Bizonyos esetekben ez nem
	///     szerencsйs, нgy szьksйg van erre a wrapper osztбlyra. Mivel ez class, ezйrt nem
	///     йrtйk szerinti бtadбs van.
	/// </summary>
	public class RectangleWrapper<TObject>
		where TObject : class
	{
		public Rect Rectangle;

		/// <summary>
		/// </summary>
		/// <param name="rectangle"></param>
		/// <param name="id">
		///     Az adott tйglalap azonosнtуja (az overlap-removal vйgйn tudnunk kell, hogy
		///     melyik tйglalap melyik objektumhoz tartozik. Az azonosнtбs megoldhatу lesz id alapjбn.
		/// </param>
		public RectangleWrapper(Rect rectangle, TObject id)
		{
			Rectangle = rectangle;
			Id = id;
		}

		public TObject Id { get; }

		public double CenterX => Rectangle.Left + Rectangle.Width / 2;

		public double CenterY => Rectangle.Top + Rectangle.Height / 2;

		public Point Center => new Point(CenterX, CenterY);
	}
}