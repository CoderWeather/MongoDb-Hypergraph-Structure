using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph
{
	[Serializable]
	[DebuggerDisplay("EdgeCount = {EdgeCount}")]
	public class EdgeListGraph<TVertex, TEdge>
		: IEdgeListGraph<TVertex, TEdge>
			, IMutableEdgeListGraph<TVertex, TEdge>
			, ICloneable
		where TEdge : IEdge<TVertex>
	{
		private readonly EdgeEdgeDictionary<TVertex, TEdge> edges = new EdgeEdgeDictionary<TVertex, TEdge>();

		public EdgeListGraph()
		{
		}

		public EdgeListGraph(bool isDirected, bool allowParralelEdges)
		{
			IsDirected = isDirected;
			AllowParallelEdges = allowParralelEdges;
		}

		public bool IsEdgesEmpty => edges.Count == 0;

		public int EdgeCount => edges.Count;

		public IEnumerable<TEdge> Edges => edges.Keys;

		[Pure]
		public bool ContainsEdge(TEdge edge)
		{
			return edges.ContainsKey(edge);
		}

		public bool IsDirected { get; } = true;

		public bool AllowParallelEdges { get; } = true;

		public bool AddEdge(TEdge edge)
		{
			if (ContainsEdge(edge))
				return false;
			edges.Add(edge, edge);
			OnEdgeAdded(edge);
			return true;
		}

		public int AddEdgeRange(IEnumerable<TEdge> edges)
		{
			var count = 0;
			foreach (var edge in edges)
				if (AddEdge(edge))
					count++;
			return count;
		}

		public event EdgeAction<TVertex, TEdge> EdgeAdded;

		public bool RemoveEdge(TEdge edge)
		{
			if (edges.Remove(edge))
			{
				OnEdgeRemoved(edge);
				return true;
			}

			return false;
		}

		public event EdgeAction<TVertex, TEdge> EdgeRemoved;

		public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
		{
			var edgesToRemove = new List<TEdge>();
			foreach (var edge in Edges)
				if (predicate(edge))
					edgesToRemove.Add(edge);

			foreach (var edge in edgesToRemove)
				edges.Remove(edge);
			return edgesToRemove.Count;
		}

		public void Clear()
		{
			var edges = this.edges.Clone();
			this.edges.Clear();
			foreach (var edge in edges.Keys)
				OnEdgeRemoved(edge);
			OnCleared(EventArgs.Empty);
		}

		public event EventHandler Cleared;

		public bool AddVerticesAndEdge(TEdge edge)
		{
			return AddEdge(edge);
		}

		public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
		{
			var count = 0;
			foreach (var edge in edges)
				if (AddVerticesAndEdge(edge))
					count++;
			return count;
		}

		protected virtual void OnEdgeAdded(TEdge args)
		{
			var eh = EdgeAdded;
			if (eh != null)
				eh(args);
		}

		protected virtual void OnEdgeRemoved(TEdge args)
		{
			var eh = EdgeRemoved;
			if (eh != null)
				eh(args);
		}

		private void OnCleared(EventArgs e)
		{
			var eh = Cleared;
			if (eh != null)
				eh(this, e);
		}

		#region ICloneable Members

		private EdgeListGraph(
			bool isDirected,
			bool allowParralelEdges,
			EdgeEdgeDictionary<TVertex, TEdge> edges)
		{
			Contract.Requires(edges != null);

			IsDirected = isDirected;
			AllowParallelEdges = allowParralelEdges;
			this.edges = edges;
		}

		public EdgeListGraph<TVertex, TEdge> Clone()
		{
			return new EdgeListGraph<TVertex, TEdge>(
				IsDirected,
				AllowParallelEdges,
				edges.Clone()
			);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		#region IVertexSet<TVertex> Members

		[Pure] public bool IsVerticesEmpty => edges.Count == 0;

		[Pure] public int VertexCount => GetVertexCounts().Count;

		[Pure] public IEnumerable<TVertex> Vertices => GetVertexCounts().Keys;

		private Dictionary<TVertex, int> GetVertexCounts()
		{
			var vertices = new Dictionary<TVertex, int>(EdgeCount * 2);
			foreach (var e in Edges)
			{
				vertices[e.Source]++;
				vertices[e.Target]++;
			}

			return vertices;
		}

		[Pure]
		public bool ContainsVertex(TVertex vertex)
		{
			foreach (var e in Edges)
				if (e.Source.Equals(vertex) ||
					e.Target.Equals(vertex))
					return true;

			return false;
		}

		#endregion
	}
}