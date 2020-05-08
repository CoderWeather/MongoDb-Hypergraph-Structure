using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
	[Serializable]
	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public sealed class ReversedBidirectionalGraph<TVertex, TEdge> :
		IBidirectionalGraph<TVertex, SReversedEdge<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		public ReversedBidirectionalGraph(IBidirectionalGraph<TVertex, TEdge> originalGraph)
		{
			Contract.Requires(originalGraph != null);
			OriginalGraph = originalGraph;
		}

		public IBidirectionalGraph<TVertex, TEdge> OriginalGraph { get; }

		public bool IsVerticesEmpty => OriginalGraph.IsVerticesEmpty;

		public bool IsDirected => OriginalGraph.IsDirected;

		public bool AllowParallelEdges => OriginalGraph.AllowParallelEdges;

		public int VertexCount => OriginalGraph.VertexCount;

		public IEnumerable<TVertex> Vertices => OriginalGraph.Vertices;

		[Pure]
		public bool ContainsVertex(TVertex vertex)
		{
			return OriginalGraph.ContainsVertex(vertex);
		}

		public bool ContainsEdge(TVertex source, TVertex target)
		{
			return OriginalGraph.ContainsEdge(target, source);
		}

		public bool TryGetEdge(
			TVertex source,
			TVertex target,
			out SReversedEdge<TVertex, TEdge> edge)
		{
			TEdge oedge;
			if (OriginalGraph.TryGetEdge(target, source, out oedge))
			{
				edge = new SReversedEdge<TVertex, TEdge>(oedge);
				return true;
			}

			edge = default;
			return false;
		}

		public bool TryGetEdges(
			TVertex source,
			TVertex target,
			out IEnumerable<SReversedEdge<TVertex, TEdge>> edges)
		{
			IEnumerable<TEdge> oedges;
			if (OriginalGraph.TryGetEdges(target, source, out oedges))
			{
				List<SReversedEdge<TVertex, TEdge>> list = new List<SReversedEdge<TVertex, TEdge>>();
				foreach (var oedge in oedges)
					list.Add(new SReversedEdge<TVertex, TEdge>(oedge));
				edges = list;
				return true;
			}

			edges = null;
			return false;
		}

		[Pure]
		public bool IsOutEdgesEmpty(TVertex v)
		{
			return OriginalGraph.IsInEdgesEmpty(v);
		}

		[Pure]
		public int OutDegree(TVertex v)
		{
			return OriginalGraph.InDegree(v);
		}

		[Pure]
		public IEnumerable<SReversedEdge<TVertex, TEdge>> InEdges(TVertex v)
		{
			return EdgeExtensions.ReverseEdges<TVertex, TEdge>(OriginalGraph.OutEdges(v));
		}

		[Pure]
		public SReversedEdge<TVertex, TEdge> InEdge(TVertex v, int index)
		{
			var edge = OriginalGraph.OutEdge(v, index);
			if (edge == null)
				return default;
			return new SReversedEdge<TVertex, TEdge>(edge);
		}

		[Pure]
		public bool IsInEdgesEmpty(TVertex v)
		{
			return OriginalGraph.IsOutEdgesEmpty(v);
		}

		[Pure]
		public int InDegree(TVertex v)
		{
			return OriginalGraph.OutDegree(v);
		}

		[Pure]
		public IEnumerable<SReversedEdge<TVertex, TEdge>> OutEdges(TVertex v)
		{
			return EdgeExtensions.ReverseEdges<TVertex, TEdge>(OriginalGraph.InEdges(v));
		}

		[Pure]
		public bool TryGetInEdges(TVertex v, out IEnumerable<SReversedEdge<TVertex, TEdge>> edges)
		{
			IEnumerable<TEdge> outEdges;
			if (OriginalGraph.TryGetOutEdges(v, out outEdges))
			{
				edges = EdgeExtensions.ReverseEdges<TVertex, TEdge>(outEdges);
				return true;
			}

			edges = null;
			return false;
		}

		[Pure]
		public bool TryGetOutEdges(TVertex v, out IEnumerable<SReversedEdge<TVertex, TEdge>> edges)
		{
			IEnumerable<TEdge> inEdges;
			if (OriginalGraph.TryGetInEdges(v, out inEdges))
			{
				edges = EdgeExtensions.ReverseEdges<TVertex, TEdge>(inEdges);
				return true;
			}

			edges = null;
			return false;
		}

		[Pure]
		public SReversedEdge<TVertex, TEdge> OutEdge(TVertex v, int index)
		{
			var edge = OriginalGraph.InEdge(v, index);
			if (edge == null)
				return default;
			return new SReversedEdge<TVertex, TEdge>(edge);
		}

		public IEnumerable<SReversedEdge<TVertex, TEdge>> Edges
		{
			get
			{
				foreach (var edge in OriginalGraph.Edges)
					yield return new SReversedEdge<TVertex, TEdge>(edge);
			}
		}

		[Pure]
		public bool ContainsEdge(SReversedEdge<TVertex, TEdge> edge)
		{
			return OriginalGraph.ContainsEdge(edge.OriginalEdge);
		}

		[Pure]
		public int Degree(TVertex v)
		{
			return OriginalGraph.Degree(v);
		}

		public bool IsEdgesEmpty => OriginalGraph.IsEdgesEmpty;

		public int EdgeCount => OriginalGraph.EdgeCount;
	}
}