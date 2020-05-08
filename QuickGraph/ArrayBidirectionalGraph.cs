using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    /// <summary>
    ///     An immutable directed graph data structure efficient for large sparse
    ///     graph representation where out-edge need to be enumerated only.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [Serializable]
	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public sealed class ArrayBidirectionalGraph<TVertex, TEdge>
		: IBidirectionalGraph<TVertex, TEdge>
			, ICloneable
		where TEdge : IEdge<TVertex>
	{
		private readonly Dictionary<TVertex, InOutEdges> vertexEdges;

        /// <summary>
        ///     Constructs a new ArrayBidirectionalGraph instance from a
        ///     IBidirectionalGraph instance
        /// </summary>
        /// <param name="visitedGraph"></param>
        public ArrayBidirectionalGraph(
			IBidirectionalGraph<TVertex, TEdge> visitedGraph
		)
		{
			Contract.Requires(visitedGraph != null);

			vertexEdges = new Dictionary<TVertex, InOutEdges>(visitedGraph.VertexCount);
			EdgeCount = visitedGraph.EdgeCount;
			foreach (var vertex in visitedGraph.Vertices)
			{
				var outEdges = visitedGraph.OutEdges(vertex).ToArray();
				var inEdges = visitedGraph.InEdges(vertex).ToArray();
				vertexEdges.Add(vertex, new InOutEdges(outEdges, inEdges));
			}
		}

		private ArrayBidirectionalGraph(
			Dictionary<TVertex, InOutEdges> vertexEdges,
			int edgeCount
		)
		{
			Contract.Requires(vertexEdges != null);
			Contract.Requires(edgeCount >= 0);

			this.vertexEdges = vertexEdges;
			EdgeCount = edgeCount;
		}

		#region IImplicitVertexSet<TVertex> Members

		public bool ContainsVertex(TVertex vertex)
		{
			return vertexEdges.ContainsKey(vertex);
		}

		#endregion

		[Serializable]
		private readonly struct InOutEdges
		{
			private readonly TEdge[] _outEdges;
			private readonly TEdge[] _inEdges;

			public InOutEdges(TEdge[] outEdges, TEdge[] inEdges)
			{
				_outEdges = outEdges != null && outEdges.Length > 0 ? outEdges : null;
				_inEdges = inEdges != null && inEdges.Length > 0 ? inEdges : null;
			}

			public bool TryGetOutEdges(out TEdge[] edges)
			{
				edges = _outEdges;
				return edges != null;
			}

			public bool TryGetInEdges(out TEdge[] edges)
			{
				edges = _inEdges;
				return edges != null;
			}
		}

		#region IIncidenceGraph<TVertex,TEdge> Members

		public bool ContainsEdge(TVertex source, TVertex target)
		{
			TEdge edge;
			return TryGetEdge(source, target, out edge);
		}

		public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
		{
			InOutEdges es;
			if (vertexEdges.TryGetValue(source, out es))
			{
				List<TEdge> _edges = null;
				TEdge[] outEdges;
				if (es.TryGetOutEdges(out outEdges))
					for (var i = 0; i < outEdges.Length; i++)
						if (outEdges[i].Target.Equals(target))
						{
							if (_edges == null)
								_edges = new List<TEdge>(outEdges.Length - i);
							_edges.Add(outEdges[i]);
						}

				edges = _edges;
				return edges != null;
			}

			edges = null;
			return false;
		}

		public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
		{
			InOutEdges io;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(source, out io) &&
				io.TryGetOutEdges(out edges))
				for (var i = 0; i < edges.Length; i++)
					if (edges[i].Target.Equals(target))
					{
						edge = edges[i];
						return true;
					}

			edge = default;
			return false;
		}

		#endregion

		#region IImplicitGraph<TVertex,TEdge> Members

		public bool IsOutEdgesEmpty(TVertex v)
		{
			InOutEdges io;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(v, out io) &&
				io.TryGetOutEdges(out edges))
				return false;
			return true;
		}

		public int OutDegree(TVertex v)
		{
			InOutEdges io;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(v, out io) &&
				io.TryGetOutEdges(out edges))
				return edges.Length;
			return 0;
		}

		public IEnumerable<TEdge> OutEdges(TVertex v)
		{
			IEnumerable<TEdge> result;
			if (TryGetInEdges(v, out result))
				return result;
			return Enumerable.Empty<TEdge>();
		}

		public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			InOutEdges io;
			TEdge[] aedges;
			if (vertexEdges.TryGetValue(v, out io) &&
				io.TryGetOutEdges(out aedges))
			{
				edges = aedges;
				return true;
			}

			edges = null;
			return false;
		}

		public TEdge OutEdge(TVertex v, int index)
		{
			var io = vertexEdges[v];

			TEdge[] edges;
			if (!io.TryGetOutEdges(out edges))
				Contract.Assert(false);

			return edges[index];
		}

		#endregion

		#region IGraph<TVertex,TEdge> Members

		public bool IsDirected => true;

		public bool AllowParallelEdges => true;

		#endregion

		#region IVertexSet<TVertex> Members

		public bool IsVerticesEmpty => vertexEdges.Count == 0;

		public int VertexCount => vertexEdges.Count;

		public IEnumerable<TVertex> Vertices => vertexEdges.Keys;

		#endregion

		#region IEdgeSet<TVertex,TEdge> Members

		public bool IsEdgesEmpty => EdgeCount == 0;

		public int EdgeCount { get; }

		public IEnumerable<TEdge> Edges
		{
			get
			{
				foreach (var io in vertexEdges.Values)
				{
					TEdge[] edges;
					if (io.TryGetOutEdges(out edges))
						for (var i = 0; i < edges.Length; i++)
							yield return edges[i];
				}
			}
		}

		public bool ContainsEdge(TEdge edge)
		{
			InOutEdges io;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(edge.Source, out io) &&
				io.TryGetOutEdges(out edges))
				for (var i = 0; i < edges.Length; i++)
					if (edges[i].Equals(edge))
						return true;
			return false;
		}

		#endregion

		#region ICloneable Members

        /// <summary>
        ///     Returns self since this class is immutable
        /// </summary>
        /// <returns></returns>
        public ArrayBidirectionalGraph<TVertex, TEdge> Clone()
		{
			return this;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		#region IBidirectionalGraph<TVertex,TEdge> Members

		public bool IsInEdgesEmpty(TVertex v)
		{
			InOutEdges io;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(v, out io) &&
				io.TryGetInEdges(out edges))
				return false;
			return true;
		}

		public int InDegree(TVertex v)
		{
			InOutEdges io;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(v, out io) &&
				io.TryGetInEdges(out edges))
				return edges.Length;
			return 0;
		}

		public IEnumerable<TEdge> InEdges(TVertex v)
		{
			IEnumerable<TEdge> result;
			if (TryGetInEdges(v, out result))
				return result;
			return Enumerable.Empty<TEdge>();
		}

		public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			InOutEdges io;
			TEdge[] aedges;
			if (vertexEdges.TryGetValue(v, out io) &&
				io.TryGetInEdges(out aedges))
			{
				edges = aedges;
				return true;
			}

			edges = null;
			return false;
		}

		public TEdge InEdge(TVertex v, int index)
		{
			var io = vertexEdges[v];

			TEdge[] edges;
			if (!io.TryGetOutEdges(out edges))
				Contract.Assert(false);

			return edges[index];
		}

		public int Degree(TVertex v)
		{
			return InDegree(v) + OutDegree(v);
		}

		#endregion
	}
}