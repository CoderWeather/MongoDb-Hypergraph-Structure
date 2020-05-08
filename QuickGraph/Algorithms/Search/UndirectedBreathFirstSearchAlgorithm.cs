using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    ///     A breath first search algorithm for undirected graphs
    /// </summary>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2" />
    [Serializable]
	public sealed class UndirectedBreadthFirstSearchAlgorithm<TVertex, TEdge>
		: RootedAlgorithmBase<TVertex, IUndirectedGraph<TVertex, TEdge>>
			, IUndirectedVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
			, IDistanceRecorderAlgorithm<TVertex, TEdge>
			, IVertexColorizerAlgorithm<TVertex, TEdge>
			, IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private IQueue<TVertex> vertexQueue;

		public UndirectedBreadthFirstSearchAlgorithm(IUndirectedGraph<TVertex, TEdge> g)
			: this(g, new Collections.Queue<TVertex>(), new Dictionary<TVertex, GraphColor>())
		{
		}

		public UndirectedBreadthFirstSearchAlgorithm(
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IQueue<TVertex> vertexQueue,
			IDictionary<TVertex, GraphColor> vertexColors
		)
			: this(null, visitedGraph, vertexQueue, vertexColors)
		{
		}

		public UndirectedBreadthFirstSearchAlgorithm(
			IAlgorithmComponent host,
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IQueue<TVertex> vertexQueue,
			IDictionary<TVertex, GraphColor> vertexColors
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(vertexQueue != null);
			Contract.Requires(vertexColors != null);

			VertexColors = vertexColors;
			this.vertexQueue = vertexQueue;
		}

		public IDictionary<TVertex, GraphColor> VertexColors { get; }

		public event VertexAction<TVertex> InitializeVertex;

		public event VertexAction<TVertex> DiscoverVertex;


		public event VertexAction<TVertex> StartVertex;

		public event UndirectedEdgeAction<TVertex, TEdge> TreeEdge;

		public event VertexAction<TVertex> FinishVertex;

		public GraphColor GetVertexColor(TVertex vertex)
		{
			return VertexColors[vertex];
		}

		private void OnInitializeVertex(TVertex v)
		{
			var eh = InitializeVertex;
			if (eh != null)
				eh(v);
		}

		private void OnStartVertex(TVertex v)
		{
			var eh = StartVertex;
			if (eh != null)
				eh(v);
		}

		public event VertexAction<TVertex> ExamineVertex;

		private void OnExamineVertex(TVertex v)
		{
			var eh = ExamineVertex;
			if (eh != null)
				eh(v);
		}

		private void OnDiscoverVertex(TVertex v)
		{
			var eh = DiscoverVertex;
			if (eh != null)
				eh(v);
		}

		public event EdgeAction<TVertex, TEdge> ExamineEdge;

		private void OnExamineEdge(TEdge e)
		{
			var eh = ExamineEdge;
			if (eh != null)
				eh(e);
		}

		private void OnTreeEdge(TEdge e, bool reversed)
		{
			var eh = TreeEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		public event UndirectedEdgeAction<TVertex, TEdge> NonTreeEdge;

		private void OnNonTreeEdge(TEdge e, bool reversed)
		{
			var eh = NonTreeEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		public event UndirectedEdgeAction<TVertex, TEdge> GrayTarget;

		private void OnGrayTarget(TEdge e, bool reversed)
		{
			var eh = GrayTarget;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		public event UndirectedEdgeAction<TVertex, TEdge> BlackTarget;

		private void OnBlackTarget(TEdge e, bool reversed)
		{
			var eh = BlackTarget;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		private void OnFinishVertex(TVertex v)
		{
			var eh = FinishVertex;
			if (eh != null)
				eh(v);
		}

		protected override void Initialize()
		{
			base.Initialize();

			// initialize vertex u
			var cancelManager = Services.CancelManager;
			if (cancelManager.IsCancelling)
				return;
			foreach (var v in VisitedGraph.Vertices)
			{
				VertexColors[v] = GraphColor.White;
				OnInitializeVertex(v);
			}
		}

		protected override void InternalCompute()
		{
			TVertex rootVertex;
			if (!TryGetRootVertex(out rootVertex))
				throw new InvalidOperationException("missing root vertex");
			EnqueueRoot(rootVertex);
			FlushVisitQueue();
		}

		public void Visit(TVertex s)
		{
			EnqueueRoot(s);
			FlushVisitQueue();
		}

		private void EnqueueRoot(TVertex s)
		{
			OnStartVertex(s);
			VertexColors[s] = GraphColor.Gray;
			OnDiscoverVertex(s);
			vertexQueue.Enqueue(s);
		}

		private void FlushVisitQueue()
		{
			var cancelManager = Services.CancelManager;

			while (vertexQueue.Count > 0)
			{
				if (cancelManager.IsCancelling) return;

				var u = vertexQueue.Dequeue();

				OnExamineVertex(u);
				foreach (var e in VisitedGraph.AdjacentEdges(u))
				{
					var reversed = e.Target.Equals(u);
					var v = reversed ? e.Source : e.Target;
					OnExamineEdge(e);

					var vColor = VertexColors[v];
					if (vColor == GraphColor.White)
					{
						OnTreeEdge(e, reversed);
						VertexColors[v] = GraphColor.Gray;
						OnDiscoverVertex(v);
						vertexQueue.Enqueue(v);
					}
					else
					{
						OnNonTreeEdge(e, reversed);
						if (vColor == GraphColor.Gray)
							OnGrayTarget(e, reversed);
						else
							OnBlackTarget(e, reversed);
					}
				}

				VertexColors[u] = GraphColor.Black;
				OnFinishVertex(u);
			}
		}
	}
}