using System.Collections.Generic;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Compound
{
	public interface ICompoundLayoutContext<TVertex, TEdge, TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		new IDictionary<TVertex, Size> Sizes { get; }
		IDictionary<TVertex, Thickness> VertexBorders { get; }
		IDictionary<TVertex, CompoundVertexInnerLayoutType> LayoutTypes { get; }
	}
}