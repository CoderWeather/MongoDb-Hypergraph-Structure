using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    /// <summary>
    ///     An immutable undirected graph data structure based on arrays.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [Serializable]
	[DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
	public sealed class ArrayUndirectedGraph<TVertex, TEdge>
		: IUndirectedGraph<TVertex, TEdge>
			, ICloneable
		where TEdge : IEdge<TVertex>
	{
		private readonly Dictionary<TVertex, TEdge[]> vertexEdges;

		public ArrayUndirectedGraph(
			IUndirectedGraph<TVertex, TEdge> graph)
		{
			Contract.Requires(graph != null);

			EdgeEqualityComparer = graph.EdgeEqualityComparer;
			EdgeCount = graph.EdgeCount;
			vertexEdges = new Dictionary<TVertex, TEdge[]>(graph.VertexCount);
			foreach (var v in graph.Vertices)
			{
				var edges = graph.AdjacentEdges(v).ToArray();
				vertexEdges.Add(v, edges);
			}
		}

		#region IImplicitVertexSet<TVertex> Members

		public bool ContainsVertex(TVertex vertex)
		{
			return vertexEdges.ContainsKey(vertex);
		}

		#endregion

		#region IImplicitUndirectedGraph<TVertex,TEdge> Members

		public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer { get; }

		public IEnumerable<TEdge> AdjacentEdges(TVertex v)
		{
			var edges = vertexEdges[v];
			return edges != null ? edges : Enumerable.Empty<TEdge>();
		}

		public int AdjacentDegree(TVertex v)
		{
			var edges = vertexEdges[v];
			return edges != null ? edges.Length : 0;
		}

		public bool IsAdjacentEdgesEmpty(TVertex v)
		{
			return vertexEdges[v] != null;
		}

		public TEdge AdjacentEdge(TVertex v, int index)
		{
			return vertexEdges[v][index];
		}

		public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
		{
			var edges = vertexEdges[source];
			if (edges != null)
				for (var i = 0; i < edges.Length; i++)
					if (EdgeEqualityComparer(edges[i], source, target))
					{
						edge = edges[i];
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

		#endregion

		#region IGraph<TVertex,TEdge> Members

		public bool IsDirected => false;

		public bool AllowParallelEdges => true;

		#endregion

		#region IEdgeSet<TVertex,TEdge> Members

		public bool IsEdgesEmpty => EdgeCount > 0;

		public int EdgeCount { get; }

		public IEnumerable<TEdge> Edges
		{
			get
			{
				foreach (var edges in vertexEdges.Values)
					if (edges != null)
						for (var i = 0; i < edges.Length; i++)
							yield return edges[i];
			}
		}

		public bool ContainsEdge(TEdge edge)
		{
			var source = edge.Source;
			TEdge[] edges;
			if (vertexEdges.TryGetValue(source, out edges))
				for (var i = 0; i < edges.Length; i++)
					if (edges[i].Equals(edge))
						return true;
			return false;
		}

		#endregion

		#region IVertexSet<TVertex> Members

		public bool IsVerticesEmpty => vertexEdges.Count == 0;

		public int VertexCount => vertexEdges.Count;

		public IEnumerable<TVertex> Vertices => vertexEdges.Keys;

		#endregion

		#region ICloneable Members

        /// <summary>
        ///     Returns self
        /// </summary>
        /// <returns></returns>
        public ArrayUndirectedGraph<TVertex, TEdge> Clone()
		{
			return this;
		}

		object ICloneable.Clone()
		{
			return this;
		}

		#endregion
	}
}