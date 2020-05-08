using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;

namespace QuickGraph.Algorithms
{
	[Serializable]
	public sealed class CentralityApproximationAlgorithm<TVertex, TEdge> :
		AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private IDictionary<TVertex, double> centralities = new Dictionary<TVertex, double>();
		private DijkstraShortestPathAlgorithm<TVertex, TEdge> dijkstra;
		private int maxIterationCount = 50;
		private VertexPredecessorRecorderObserver<TVertex, TEdge> predecessorRecorder;
		private Random rand = new Random();

		public CentralityApproximationAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> distances
		)
			: base(visitedGraph)
		{
			Contract.Requires(distances != null);

			dijkstra = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(
				VisitedGraph,
				distances,
				DistanceRelaxers.ShortestDistance
			);
			predecessorRecorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
			predecessorRecorder.Attach(dijkstra);
		}

		public Func<TEdge, double> Distances => dijkstra.Weights;

		public Random Rand
		{
			get => rand;
			set => rand = value;
		}

		public int MaxIterationCount
		{
			get => maxIterationCount;
			set => maxIterationCount = value;
		}

		protected override void Initialize()
		{
			base.Initialize();
			centralities.Clear();
			foreach (var v in VisitedGraph.Vertices)
				centralities.Add(v, 0);
		}

		protected override void InternalCompute()
		{
			if (VisitedGraph.VertexCount == 0)
				return;

			// compute temporary values
			var n = VisitedGraph.VertexCount;
			for (var i = 0; i < MaxIterationCount; ++i)
			{
				var v = RandomGraphFactory.GetVertex(VisitedGraph, Rand);
				dijkstra.Compute(v);

				foreach (var u in VisitedGraph.Vertices)
				{
					double d;
					if (dijkstra.TryGetDistance(u, out d))
						centralities[u] += n * d / (MaxIterationCount * (n - 1));
				}
			}

			// update
			foreach (var v in centralities.Keys)
				centralities[v] = 1.0 / centralities[v];
		}
	}
}