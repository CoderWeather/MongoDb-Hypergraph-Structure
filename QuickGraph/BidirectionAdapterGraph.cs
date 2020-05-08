using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph
{
    /// <summary>
    ///     Wraps a vertex list graph (out-edges only) and caches the in-edge dictionary.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [Serializable]
	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public class BidirectionAdapterGraph<TVertex, TEdge>
		: IBidirectionalGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private static readonly TEdge[] emptyEdges = new TEdge[0];
		private readonly IVertexAndEdgeListGraph<TVertex, TEdge> baseGraph;
		private readonly Dictionary<TVertex, EdgeList<TVertex, TEdge>> inEdges;

		public BidirectionAdapterGraph(IVertexAndEdgeListGraph<TVertex, TEdge> baseGraph)
		{
			Contract.Requires(baseGraph != null);

			this.baseGraph = baseGraph;
			inEdges = new Dictionary<TVertex, EdgeList<TVertex, TEdge>>(this.baseGraph.VertexCount);
			foreach (var edge in this.baseGraph.Edges)
			{
				EdgeList<TVertex, TEdge> list;
				if (!inEdges.TryGetValue(edge.Target, out list))
					inEdges.Add(edge.Target, list = new EdgeList<TVertex, TEdge>());
				list.Add(edge);
			}
		}

		[Pure]
		public bool IsInEdgesEmpty(TVertex v)
		{
			return InDegree(v) == 0;
		}

		[Pure]
		public int InDegree(TVertex v)
		{
			EdgeList<TVertex, TEdge> edges;
			if (inEdges.TryGetValue(v, out edges))
				return edges.Count;
			return 0;
		}

		[Pure]
		public IEnumerable<TEdge> InEdges(TVertex v)
		{
			EdgeList<TVertex, TEdge> edges;
			if (inEdges.TryGetValue(v, out edges))
				return edges;
			return emptyEdges;
		}

		[Pure]
		public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			EdgeList<TVertex, TEdge> es;
			if (inEdges.TryGetValue(v, out es))
			{
				edges = es;
				return true;
			}

			edges = null;
			return false;
		}

		[Pure]
		public TEdge InEdge(TVertex v, int index)
		{
			return inEdges[v][index];
		}

		[Pure]
		public int Degree(TVertex v)
		{
			return InDegree(v) + OutDegree(v);
		}

		[Pure]
		public bool ContainsEdge(TVertex source, TVertex target)
		{
			return baseGraph.ContainsEdge(source, target);
		}

		[Pure]
		public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
		{
			return baseGraph.TryGetEdges(source, target, out edges);
		}

		[Pure]
		public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
		{
			return baseGraph.TryGetEdge(source, target, out edge);
		}

		[Pure] // InterfacePureBug
		public bool IsOutEdgesEmpty(TVertex v)
		{
			return baseGraph.IsOutEdgesEmpty(v);
		}

		[Pure]
		public int OutDegree(TVertex v)
		{
			return baseGraph.OutDegree(v);
		}

		[Pure]
		public IEnumerable<TEdge> OutEdges(TVertex v)
		{
			return baseGraph.OutEdges(v);
		}

		[Pure]
		public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			return baseGraph.TryGetOutEdges(v, out edges);
		}

		[Pure]
		public TEdge OutEdge(TVertex v, int index)
		{
			return baseGraph.OutEdge(v, index);
		}

		public bool IsDirected => baseGraph.IsDirected;

		public bool AllowParallelEdges => baseGraph.AllowParallelEdges;

		public bool IsVerticesEmpty => baseGraph.IsVerticesEmpty;

		public int VertexCount => baseGraph.VertexCount;

		public IEnumerable<TVertex> Vertices => baseGraph.Vertices;

		[Pure]
		public bool ContainsVertex(TVertex vertex)
		{
			return baseGraph.ContainsVertex(vertex);
		}

		public bool IsEdgesEmpty => baseGraph.IsEdgesEmpty;

		public int EdgeCount => baseGraph.EdgeCount;

		public virtual IEnumerable<TEdge> Edges => baseGraph.Edges;

		[Pure]
		public bool ContainsEdge(TEdge edge)
		{
			return baseGraph.ContainsEdge(edge);
		}
	}
}