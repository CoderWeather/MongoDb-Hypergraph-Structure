using System.Collections.Generic;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Contextual
{
	public class ContextualLayoutContext<TVertex, TEdge, TGraph> : LayoutContext<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		public ContextualLayoutContext(TGraph graph, TVertex selectedVertex, IDictionary<TVertex, Point> positions,
			IDictionary<TVertex, Size> sizes)
			: base(graph, positions, sizes, LayoutMode.Simple)
		{
			SelectedVertex = selectedVertex;
		}

		public TVertex SelectedVertex { get; }
	}
}