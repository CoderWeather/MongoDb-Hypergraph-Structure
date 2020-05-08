using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Services;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.ShortestPath
{
    /// <summary>
    ///     Dijkstra single-source shortest path algorithm for directed graph
    ///     with positive distance.
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <reference-ref
    ///     idref="lawler01combinatorial" />
    [Serializable]
	public sealed class DijkstraShortestPathAlgorithm<TVertex, TEdge>
		: ShortestPathAlgorithmBase<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
			, IVertexColorizerAlgorithm<TVertex, TEdge>
			, IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
			, IDistanceRecorderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private FibonacciQueue<TVertex, double> vertexQueue;

		public DijkstraShortestPathAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights)
			: this(visitedGraph, weights, DistanceRelaxers.ShortestDistance)
		{
		}

		public DijkstraShortestPathAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights,
			IDistanceRelaxer distanceRelaxer
		)
			: this(null, visitedGraph, weights, distanceRelaxer)
		{
		}

		public DijkstraShortestPathAlgorithm(
			IAlgorithmComponent host,
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights,
			IDistanceRelaxer distanceRelaxer
		)
			: base(host, visitedGraph, weights, distanceRelaxer)
		{
		}

		public event VertexAction<TVertex> InitializeVertex;
		public event VertexAction<TVertex> DiscoverVertex;
		public event VertexAction<TVertex> StartVertex;
		public event VertexAction<TVertex> FinishVertex;
		public event VertexAction<TVertex> ExamineVertex;
		public event EdgeAction<TVertex, TEdge> ExamineEdge;

		public event EdgeAction<TVertex, TEdge> EdgeNotRelaxed;

		private void OnEdgeNotRelaxed(TEdge e)
		{
			var eh = EdgeNotRelaxed;
			if (eh != null)
				eh(e);
		}

		private void InternalExamineEdge(TEdge args)
		{
			if (Weights(args) < 0)
				throw new NegativeWeightException();
		}

		private void InternalTreeEdge(TEdge args)
		{
			var decreased = Relax(args);
			if (decreased)
			{
				OnTreeEdge(args);
				AssertHeap();
			}
			else
			{
				OnEdgeNotRelaxed(args);
			}
		}

		private void InternalGrayTarget(TEdge edge)
		{
			var decreased = Relax(edge);
			if (decreased)
			{
				vertexQueue.Update(edge.Target);
				AssertHeap();
				OnTreeEdge(edge);
			}
			else
			{
				OnEdgeNotRelaxed(edge);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			// init color, distance
			var initialDistance = DistanceRelaxer.InitialDistance;
			foreach (var u in VisitedGraph.Vertices)
			{
				VertexColors.Add(u, GraphColor.White);
				Distances.Add(u, initialDistance);
			}

			vertexQueue = new FibonacciQueue<TVertex, double>(DistancesIndexGetter());
		}

		protected override void InternalCompute()
		{
			TVertex rootVertex;
			if (TryGetRootVertex(out rootVertex))
				ComputeFromRoot(rootVertex);
			else
				foreach (var v in VisitedGraph.Vertices)
					if (VertexColors[v] == GraphColor.White)
						ComputeFromRoot(v);
		}

		private void ComputeFromRoot(TVertex rootVertex)
		{
			Contract.Requires(rootVertex != null);
			Contract.Requires(VisitedGraph.ContainsVertex(rootVertex));
			Contract.Requires(VertexColors[rootVertex] == GraphColor.White);

			VertexColors[rootVertex] = GraphColor.Gray;
			Distances[rootVertex] = 0;
			ComputeNoInit(rootVertex);
		}

		[Conditional("DEBUG")]
		private void AssertHeap()
		{
			if (vertexQueue.Count == 0) return;
			var top = vertexQueue.Peek();
			var vertices = vertexQueue.ToArray();
			for (var i = 1; i < vertices.Length; ++i)
				if (Distances[top] > Distances[vertices[i]])
					Contract.Assert(false);
		}

		public void ComputeNoInit(TVertex s)
		{
			BreadthFirstSearchAlgorithm<TVertex, TEdge> bfs = null;

			try
			{
				bfs = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(
					this,
					VisitedGraph,
					vertexQueue,
					VertexColors
				);

				bfs.InitializeVertex += InitializeVertex;
				bfs.DiscoverVertex += DiscoverVertex;
				bfs.StartVertex += StartVertex;
				bfs.ExamineEdge += ExamineEdge;
#if SUPERDEBUG
                bfs.ExamineEdge += e => this.AssertHeap();
#endif
				bfs.ExamineVertex += ExamineVertex;
				bfs.FinishVertex += FinishVertex;

				bfs.ExamineEdge += InternalExamineEdge;
				bfs.TreeEdge += InternalTreeEdge;
				bfs.GrayTarget += InternalGrayTarget;

				bfs.Visit(s);
			}
			finally
			{
				if (bfs != null)
				{
					bfs.InitializeVertex -= InitializeVertex;
					bfs.DiscoverVertex -= DiscoverVertex;
					bfs.StartVertex -= StartVertex;
					bfs.ExamineEdge -= ExamineEdge;
					bfs.ExamineVertex -= ExamineVertex;
					bfs.FinishVertex -= FinishVertex;

					bfs.ExamineEdge -= InternalExamineEdge;
					bfs.TreeEdge -= InternalTreeEdge;
					bfs.GrayTarget -= InternalGrayTarget;
				}
			}
		}
	}
}