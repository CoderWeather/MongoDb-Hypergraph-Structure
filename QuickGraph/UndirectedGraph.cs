using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph
{
	public delegate bool EdgeEqualityComparer<TVertex, TEdge>(TEdge edge, TVertex source, TVertex target)
		where TEdge : IEdge<TVertex>;

	[Serializable]
	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public class UndirectedGraph<TVertex, TEdge>
		: IMutableUndirectedGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private readonly VertexEdgeDictionary<TVertex, TEdge> adjacentEdges;
		private readonly EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer;
		private int edgeCapacity = 4;

		public UndirectedGraph()
			: this(true)
		{
		}

		public UndirectedGraph(bool allowParallelEdges)
			: this(allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>())
		{
			AllowParallelEdges = allowParallelEdges;
		}

		public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer)
			: this(allowParallelEdges, edgeEqualityComparer, -1)
		{
		}

		public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer,
			int vertexCapacity)
			: this(allowParallelEdges, edgeEqualityComparer, vertexCapacity, EqualityComparer<TVertex>.Default)
		{
		}

		public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer,
			int vertexCapacity, IEqualityComparer<TVertex> vertexComparer)
		{
			Contract.Requires(edgeEqualityComparer != null);
			Contract.Requires(vertexComparer != null);

			AllowParallelEdges = allowParallelEdges;
			this.edgeEqualityComparer = edgeEqualityComparer;
			if (vertexCapacity > -1)
				adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>(vertexCapacity, vertexComparer);
			else
				adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>(vertexComparer);
		}

		public int EdgeCapacity
		{
			get => edgeCapacity;
			set => edgeCapacity = value;
		}

		public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer
		{
			get
			{
				Contract.Ensures(Contract.Result<EdgeEqualityComparer<TVertex, TEdge>>() != null);
				return edgeEqualityComparer;
			}
		}

		#region IGraph<Vertex,Edge> Members

		public bool IsDirected => false;

		public bool AllowParallelEdges { get; } = true;

		#endregion

		#region IMutableUndirected<Vertex,Edge> Members

		public event VertexAction<TVertex> VertexAdded;

		protected virtual void OnVertexAdded(TVertex args)
		{
			Contract.Requires(args != null);

			var eh = VertexAdded;
			if (eh != null)
				eh(args);
		}

		public int AddVertexRange(IEnumerable<TVertex> vertices)
		{
			var count = 0;
			foreach (var v in vertices)
				if (AddVertex(v))
					count++;
			return count;
		}

		public bool AddVertex(TVertex v)
		{
			if (ContainsVertex(v))
				return false;

			var edges = EdgeCapacity < 0
				? new EdgeList<TVertex, TEdge>()
				: new EdgeList<TVertex, TEdge>(EdgeCapacity);
			adjacentEdges.Add(v, edges);
			OnVertexAdded(v);
			return true;
		}

		private IEdgeList<TVertex, TEdge> AddAndReturnEdges(TVertex v)
		{
			IEdgeList<TVertex, TEdge> edges;
			if (!adjacentEdges.TryGetValue(v, out edges))
				adjacentEdges[v] = edges = EdgeCapacity < 0
					? new EdgeList<TVertex, TEdge>()
					: new EdgeList<TVertex, TEdge>(EdgeCapacity);

			return edges;
		}

		public event VertexAction<TVertex> VertexRemoved;

		protected virtual void OnVertexRemoved(TVertex args)
		{
			Contract.Requires(args != null);

			var eh = VertexRemoved;
			if (eh != null)
				eh(args);
		}

		public bool RemoveVertex(TVertex v)
		{
			ClearAdjacentEdges(v);
			var result = adjacentEdges.Remove(v);

			if (result)
				OnVertexRemoved(v);

			return result;
		}

		public int RemoveVertexIf(VertexPredicate<TVertex> pred)
		{
			var vertices = new List<TVertex>();
			foreach (var v in Vertices)
				if (pred(v))
					vertices.Add(v);

			foreach (var v in vertices)
				RemoveVertex(v);
			return vertices.Count;
		}

		#endregion

		#region IMutableIncidenceGraph<Vertex,Edge> Members

		public int RemoveAdjacentEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
		{
			var outEdges = adjacentEdges[v];
			var edges = new List<TEdge>(outEdges.Count);
			foreach (var edge in outEdges)
				if (predicate(edge))
					edges.Add(edge);

			RemoveEdges(edges);
			return edges.Count;
		}

		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(EdgeCount >= 0);
		}

		public void ClearAdjacentEdges(TVertex v)
		{
			var edges = adjacentEdges[v].Clone();
			EdgeCount -= edges.Count;

			foreach (var edge in edges)
			{
				IEdgeList<TVertex, TEdge> aEdges;
				if (adjacentEdges.TryGetValue(edge.Target, out aEdges))
					aEdges.Remove(edge);
				if (adjacentEdges.TryGetValue(edge.Source, out aEdges))
					aEdges.Remove(edge);
			}
		}

		#endregion

		#region IMutableGraph<Vertex,Edge> Members

		public void TrimEdgeExcess()
		{
			foreach (var edges in adjacentEdges.Values)
				edges.TrimExcess();
		}

		public void Clear()
		{
			adjacentEdges.Clear();
			EdgeCount = 0;
			OnCleared(EventArgs.Empty);
		}

		public event EventHandler Cleared;

		private void OnCleared(EventArgs e)
		{
			var eh = Cleared;
			if (eh != null)
				eh(this, e);
		}

		#endregion

		#region IUndirectedGraph<Vertex,Edge> Members

		public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
		{
			foreach (var e in AdjacentEdges(source))
				if (edgeEqualityComparer(e, source, target))
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

		public TEdge AdjacentEdge(TVertex v, int index)
		{
			return adjacentEdges[v][index];
		}

		public bool IsVerticesEmpty => adjacentEdges.Count == 0;

		public int VertexCount => adjacentEdges.Count;

		public IEnumerable<TVertex> Vertices => adjacentEdges.Keys;


		[Pure]
		public bool ContainsVertex(TVertex vertex)
		{
			return adjacentEdges.ContainsKey(vertex);
		}

		#endregion

		#region IMutableEdgeListGraph<Vertex,Edge> Members

		public bool AddVerticesAndEdge(TEdge edge)
		{
			var sourceEdges = AddAndReturnEdges(edge.Source);
			var targetEdges = AddAndReturnEdges(edge.Target);

			if (!AllowParallelEdges)
				if (ContainsEdgeBetweenVertices(sourceEdges, edge))
					return false;

			sourceEdges.Add(edge);
			if (!edge.IsSelfEdge<TVertex, TEdge>())
				targetEdges.Add(edge);
			EdgeCount++;
			OnEdgeAdded(edge);

			return true;
		}

		public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
		{
			var count = 0;
			foreach (var edge in edges)
				if (AddVerticesAndEdge(edge))
					count++;
			return count;
		}

		public bool AddEdge(TEdge edge)
		{
			var sourceEdges = adjacentEdges[edge.Source];
			if (!AllowParallelEdges)
				if (ContainsEdgeBetweenVertices(sourceEdges, edge))
					return false;

			sourceEdges.Add(edge);
			if (!edge.IsSelfEdge<TVertex, TEdge>())
			{
				var targetEdges = adjacentEdges[edge.Target];
				targetEdges.Add(edge);
			}

			EdgeCount++;
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

		protected virtual void OnEdgeAdded(TEdge args)
		{
			var eh = EdgeAdded;
			if (eh != null)
				eh(args);
		}

		public bool RemoveEdge(TEdge edge)
		{
			var removed = adjacentEdges[edge.Source].Remove(edge);
			if (removed)
			{
				if (!edge.IsSelfEdge<TVertex, TEdge>())
					adjacentEdges[edge.Target].Remove(edge);
				EdgeCount--;
				Contract.Assert(EdgeCount >= 0);
				OnEdgeRemoved(edge);
				return true;
			}

			return false;
		}

		public event EdgeAction<TVertex, TEdge> EdgeRemoved;

		protected virtual void OnEdgeRemoved(TEdge args)
		{
			var eh = EdgeRemoved;
			if (eh != null)
				eh(args);
		}

		public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
		{
			List<TEdge> edges = new List<TEdge>();
			foreach (var edge in Edges)
				if (predicate(edge))
					edges.Add(edge);
			return RemoveEdges(edges);
		}

		public int RemoveEdges(IEnumerable<TEdge> edges)
		{
			var count = 0;
			foreach (var edge in edges)
				if (RemoveEdge(edge))
					count++;
			return count;
		}

		#endregion

		#region IEdgeListGraph<Vertex,Edge> Members

		public bool IsEdgesEmpty => EdgeCount == 0;

		public int EdgeCount { get; private set; }

		public IEnumerable<TEdge> Edges
		{
			get
			{
				var edgeColors = new Dictionary<TEdge, GraphColor>(EdgeCount);
				foreach (var edges in adjacentEdges.Values)
				foreach (var edge in edges)
				{
					GraphColor c;
					if (edgeColors.TryGetValue(edge, out c))
						continue;
					edgeColors.Add(edge, GraphColor.Black);
					yield return edge;
				}
			}
		}

		public bool ContainsEdge(TEdge edge)
		{
			var eqc = EdgeEqualityComparer;
			foreach (var e in AdjacentEdges(edge.Source))
				if (e.Equals(edge))
					return true;
			return false;
		}

		private bool ContainsEdgeBetweenVertices(IEnumerable<TEdge> edges, TEdge edge)
		{
			Contract.Requires(edges != null);
			Contract.Requires(edge != null);

			var source = edge.Source;
			var target = edge.Target;
			foreach (var e in edges)
				if (EdgeEqualityComparer(e, source, target))
					return true;
			return false;
		}

		#endregion

		#region IUndirectedGraph<Vertex,Edge> Members

		public IEnumerable<TEdge> AdjacentEdges(TVertex v)
		{
			return adjacentEdges[v];
		}

		public int AdjacentDegree(TVertex v)
		{
			return adjacentEdges[v].Count;
		}

		public bool IsAdjacentEdgesEmpty(TVertex v)
		{
			return adjacentEdges[v].Count == 0;
		}

		#endregion
	}
}