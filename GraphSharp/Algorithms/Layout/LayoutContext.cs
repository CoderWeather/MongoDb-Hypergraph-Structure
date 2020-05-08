using System.Collections.Generic;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout
{
	public class LayoutContext<TVertex, TEdge, TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		public LayoutContext(TGraph graph, IDictionary<TVertex, Point> positions, IDictionary<TVertex, Size> sizes,
			LayoutMode mode)
		{
			Graph = graph;
			Positions = positions;
			Sizes = sizes;
			Mode = mode;
		}

		public IDictionary<TVertex, Point> Positions { get; }

		public IDictionary<TVertex, Size> Sizes { get; }

		public TGraph Graph { get; }

		public LayoutMode Mode { get; }
	}
}