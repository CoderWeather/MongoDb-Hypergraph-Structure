using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
	[Serializable]
	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public sealed class UndirectedBidirectionalGraph<TVertex, TEdge> :
		IUndirectedGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		public UndirectedBidirectionalGraph(IBidirectionalGraph<TVertex, TEdge> visitedGraph)
		{
			Contract.Requires(visitedGraph != null);

			VisitedGraph = visitedGraph;
		}

		public IBidirectionalGraph<TVertex, TEdge> VisitedGraph { get; }

		public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer { get; } =
			EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();

		#region IUndirectedGraph<Vertex,Edge> Members

		[Pure]
		public IEnumerable<TEdge> AdjacentEdges(TVertex v)
		{
			foreach (var e in VisitedGraph.OutEdges(v))
				yield return e;
			foreach (var e in VisitedGraph.InEdges(v))
			{
				// we skip selfedges here since
				// we already did those in the outedge run
				if (e.Source.Equals(e.Target))
					continue;
				yield return e;
			}
		}

		[Pure]
		public int AdjacentDegree(TVertex v)
		{
			return VisitedGraph.Degree(v);
		}

		[Pure]
		public bool IsAdjacentEdgesEmpty(TVertex v)
		{
			return VisitedGraph.IsOutEdgesEmpty(v) && VisitedGraph.IsInEdgesEmpty(v);
		}

		[Pure]
		public TEdge AdjacentEdge(TVertex v, int index)
		{
			throw new NotSupportedException();
		}

		[Pure]
		public bool ContainsEdge(TVertex source, TVertex target)
		{
			throw new NotSupportedException();
		}

		public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IVertexSet<Vertex,Edge> Members

		public bool IsVerticesEmpty => VisitedGraph.IsVerticesEmpty;

		public int VertexCount => VisitedGraph.VertexCount;

		public IEnumerable<TVertex> Vertices => VisitedGraph.Vertices;

		[Pure]
		public bool ContainsVertex(TVertex vertex)
		{
			return VisitedGraph.ContainsVertex(vertex);
		}

		#endregion

		#region IEdgeListGraph<Vertex,Edge> Members

		public bool IsEdgesEmpty => VisitedGraph.IsEdgesEmpty;

		public int EdgeCount => VisitedGraph.EdgeCount;

		public IEnumerable<TEdge> Edges => VisitedGraph.Edges;

		[Pure]
		public bool ContainsEdge(TEdge edge)
		{
			return VisitedGraph.ContainsEdge(edge);
		}

		#endregion

		#region IGraph<Vertex,Edge> Members

		public bool IsDirected => false;

		public bool AllowParallelEdges => VisitedGraph.AllowParallelEdges;

		#endregion
	}
}