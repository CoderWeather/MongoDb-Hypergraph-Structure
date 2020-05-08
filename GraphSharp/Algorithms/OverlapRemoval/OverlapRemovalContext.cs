using System.Collections.Generic;
using System.Windows;

namespace GraphSharp.Algorithms.OverlapRemoval
{
	public class OverlapRemovalContext<TVertex> : IOverlapRemovalContext<TVertex>
	{
		public OverlapRemovalContext(IDictionary<TVertex, Rect> rectangles)
		{
			Rectangles = rectangles;
		}

		public IDictionary<TVertex, Rect> Rectangles { get; }
	}
}