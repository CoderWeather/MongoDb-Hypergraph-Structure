using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
	[Serializable]
	public class FilteredImplicitGraph<TVertex, TEdge, TGraph>
		: FilteredImplicitVertexSet<TVertex, TEdge, TGraph>
			, IImplicitGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
		where TGraph : IImplicitGraph<TVertex, TEdge>
	{
		public FilteredImplicitGraph(
			TGraph baseGraph,
			VertexPredicate<TVertex> vertexPredicate,
			EdgePredicate<TVertex, TEdge> edgePredicate
		)
			: base(baseGraph, vertexPredicate, edgePredicate)
		{
		}

		[Pure]
		public bool IsOutEdgesEmpty(TVertex v)
		{
			return OutDegree(v) == 0;
		}

		[Pure]
		public int OutDegree(TVertex v)
		{
			var count = 0;
			foreach (var edge in BaseGraph.OutEdges(v))
				if (TestEdge(edge))
					count++;
			return count;
		}

		[Pure]
		public IEnumerable<TEdge> OutEdges(TVertex v)
		{
			foreach (var edge in BaseGraph.OutEdges(v))
				if (TestEdge(edge))
					yield return edge;
		}

		[Pure]
		public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			IEnumerable<TEdge> baseEdges;
			if (!BaseGraph.TryGetOutEdges(v, out baseEdges))
			{
				edges = null;
				return false;
			}

			edges = OutEdges(v);
			return true;
		}

		[Pure]
		public TEdge OutEdge(TVertex v, int index)
		{
			throw new NotSupportedException();
		}
	}
}