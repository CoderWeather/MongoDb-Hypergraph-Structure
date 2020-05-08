using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.ConnectedComponents
{
	public sealed class IncrementalConnectedComponentsAlgorithm<TVertex, TEdge>
		: AlgorithmBase<IMutableVertexAndEdgeSet<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private Dictionary<TVertex, int> components;
		private ForestDisjointSet<TVertex> ds;

		public IncrementalConnectedComponentsAlgorithm(IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph)
			: this(null, visitedGraph)
		{
		}

		public IncrementalConnectedComponentsAlgorithm(IAlgorithmComponent host,
			IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph)
			: base(host, visitedGraph)
		{
		}

		public int ComponentCount
		{
			get
			{
				Contract.Assert(ds != null);
				return ds.SetCount;
			}
		}

		protected override void InternalCompute()
		{
			ds = new ForestDisjointSet<TVertex>(VisitedGraph.VertexCount);
			// initialize 1 set per vertex
			foreach (var v in VisitedGraph.Vertices)
				ds.MakeSet(v);

			// join existing edges
			foreach (var e in VisitedGraph.Edges)
				ds.Union(e.Source, e.Target);

			// unhook/hook to graph event
			VisitedGraph.EdgeAdded += VisitedGraph_EdgeAdded;
			VisitedGraph.EdgeRemoved += VisitedGraph_EdgeRemoved;
			VisitedGraph.VertexAdded += VisitedGraph_VertexAdded;
			VisitedGraph.VertexRemoved += VisitedGraph_VertexRemoved;
		}

        /// <summary>
        ///     Gets a copy of the connected components. Key is the number of components,
        ///     Value contains the vertex -> component index map.
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<int, IDictionary<TVertex, int>> GetComponents()
		{
			Contract.Ensures(
				Contract.Result<KeyValuePair<int, IDictionary<TVertex, int>>>().Key == ComponentCount);
			Contract.Ensures(
				Contract.Result<KeyValuePair<int, IDictionary<TVertex, int>>>().Value.Count ==
				VisitedGraph.VertexCount);
			// TODO: more contracts
			Contract.Assert(ds != null);

			var representatives = new Dictionary<TVertex, int>(ds.SetCount);
			if (components == null)
				components = new Dictionary<TVertex, int>(VisitedGraph.VertexCount);
			foreach (var v in VisitedGraph.Vertices)
			{
				var representative = ds.FindSet(v);
				int index;
				if (!representatives.TryGetValue(representative, out index))
					representatives[representative] = index = representatives.Count;
				components[v] = index;
			}

			return new KeyValuePair<int, IDictionary<TVertex, int>>(ds.SetCount, components);
		}

		private void VisitedGraph_VertexAdded(TVertex v)
		{
			ds.MakeSet(v);
		}

		private void VisitedGraph_EdgeAdded(TEdge e)
		{
			ds.Union(e.Source, e.Target);
		}

		private void VisitedGraph_VertexRemoved(TVertex e)
		{
			throw new InvalidOperationException("vertex removal not supported for incremental connected components");
		}

		private void VisitedGraph_EdgeRemoved(TEdge e)
		{
			throw new InvalidOperationException("edge removal not supported for incremental connected components");
		}
	}
}