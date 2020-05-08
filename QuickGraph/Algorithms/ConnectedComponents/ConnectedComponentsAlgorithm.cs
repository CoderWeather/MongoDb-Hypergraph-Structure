using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.ConnectedComponents
{
	[Serializable]
	public sealed class ConnectedComponentsAlgorithm<TVertex, TEdge> :
		AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>,
		IConnectedComponentAlgorithm<TVertex, TEdge, IUndirectedGraph<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		public ConnectedComponentsAlgorithm(IUndirectedGraph<TVertex, TEdge> g)
			: this(g, new Dictionary<TVertex, int>())
		{
		}

		public ConnectedComponentsAlgorithm(
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, int> components)
			: this(null, visitedGraph, components)
		{
		}

		public ConnectedComponentsAlgorithm(
			IAlgorithmComponent host,
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, int> components)
			: base(host, visitedGraph)
		{
			Contract.Requires(components != null);

			Components = components;
		}

		public IDictionary<TVertex, int> Components { get; }

		public int ComponentCount { get; private set; }

		private void StartVertex(TVertex v)
		{
			++ComponentCount;
		}

		private void DiscoverVertex(TVertex v)
		{
			Components[v] = ComponentCount;
		}

		protected override void InternalCompute()
		{
			Components.Clear();
			if (VisitedGraph.VertexCount == 0)
			{
				ComponentCount = 0;
				return;
			}

			ComponentCount = -1;
			UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
			try
			{
				dfs = new UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>(
					this,
					VisitedGraph,
					new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount)
				);

				dfs.StartVertex += StartVertex;
				dfs.DiscoverVertex += DiscoverVertex;
				dfs.Compute();
				++ComponentCount;
			}
			finally
			{
				if (dfs != null)
				{
					dfs.StartVertex -= StartVertex;
					dfs.DiscoverVertex -= DiscoverVertex;
				}
			}
		}
	}
}