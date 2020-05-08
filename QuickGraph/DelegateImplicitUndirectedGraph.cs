using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    /// <summary>
    ///     A functional implicit undirected graph
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [Serializable]
	public class DelegateImplicitUndirectedGraph<TVertex, TEdge>
		: IImplicitUndirectedGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		public DelegateImplicitUndirectedGraph(
			TryFunc<TVertex, IEnumerable<TEdge>> tryGetAdjacenyEdges,
			bool allowParallelEdges)
		{
			Contract.Requires(tryGetAdjacenyEdges != null);

			TryGetAdjacencyEdgesFunc = tryGetAdjacenyEdges;
			AllowParallelEdges = allowParallelEdges;
		}

		public TryFunc<TVertex, IEnumerable<TEdge>> TryGetAdjacencyEdgesFunc { get; }

		public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer { get; } =
			EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();

		public bool IsAdjacentEdgesEmpty(TVertex v)
		{
			foreach (var edge in AdjacentEdges(v))
				return false;
			return true;
		}

		public int AdjacentDegree(TVertex v)
		{
			return AdjacentEdges(v).Count();
		}

		public IEnumerable<TEdge> AdjacentEdges(TVertex v)
		{
			IEnumerable<TEdge> result;
			if (!TryGetAdjacencyEdgesFunc(v, out result))
				return Enumerable.Empty<TEdge>();
			return result;
		}

		public TEdge AdjacentEdge(TVertex v, int index)
		{
			return AdjacentEdges(v).ElementAt(index);
		}

		public bool IsDirected => false;

		public bool AllowParallelEdges { get; }

		public bool ContainsVertex(TVertex vertex)
		{
			IEnumerable<TEdge> edges;
			return
				TryGetAdjacencyEdgesFunc(vertex, out edges);
		}

		public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
		{
			IEnumerable<TEdge> edges;
			if (TryGetAdjacentEdges(source, out edges))
				foreach (var e in edges)
					if (EdgeEqualityComparer(e, source, target))
					{
						edge = e;
						return true;
					}

			edge = default;
			return false;
		}

		public bool ContainsEdge(TVertex source, TVertex target)
		{
			TEdge edge;
			return TryGetEdge(source, target, out edge);
		}

		public bool TryGetAdjacentEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			return TryGetAdjacencyEdgesFunc(v, out edges);
		}
	}
}