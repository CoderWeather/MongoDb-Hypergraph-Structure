using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    ///     A breath first search algorithm for directed graphs
    /// </summary>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2" />
    public sealed class BreadthFirstSearchAlgorithm<TVertex, TEdge>
		: RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>
			, IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
			, IDistanceRecorderAlgorithm<TVertex, TEdge>
			, IVertexColorizerAlgorithm<TVertex, TEdge>
			, ITreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private readonly IQueue<TVertex> vertexQueue;

		public BreadthFirstSearchAlgorithm(IVertexListGraph<TVertex, TEdge> g)
			: this(g, new Collections.Queue<TVertex>(), new Dictionary<TVertex, GraphColor>())
		{
		}

		public BreadthFirstSearchAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IQueue<TVertex> vertexQueue,
			IDictionary<TVertex, GraphColor> vertexColors
		)
			: this(null, visitedGraph, vertexQueue, vertexColors)
		{
		}

		public BreadthFirstSearchAlgorithm(
			IAlgorithmComponent host,
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IQueue<TVertex> vertexQueue,
			IDictionary<TVertex, GraphColor> vertexColors
		)
			: this(host, visitedGraph, vertexQueue, vertexColors, e => e)
		{
		}

		public BreadthFirstSearchAlgorithm(
			IAlgorithmComponent host,
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IQueue<TVertex> vertexQueue,
			IDictionary<TVertex, GraphColor> vertexColors,
			Func<IEnumerable<TEdge>, IEnumerable<TEdge>> outEdgeEnumerator
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(vertexQueue != null);
			Contract.Requires(vertexColors != null);
			Contract.Requires(outEdgeEnumerator != null);

			VertexColors = vertexColors;
			this.vertexQueue = vertexQueue;
			OutEdgeEnumerator = outEdgeEnumerator;
		}

		public Func<IEnumerable<TEdge>, IEnumerable<TEdge>> OutEdgeEnumerator { get; }

		public IDictionary<TVertex, GraphColor> VertexColors { get; }

		public event VertexAction<TVertex> InitializeVertex;

		public event VertexAction<TVertex> DiscoverVertex;

		public GraphColor GetVertexColor(TVertex vertex)
		{
			return VertexColors[vertex];
		}

		public event VertexAction<TVertex> StartVertex;

		public event EdgeAction<TVertex, TEdge> TreeEdge;

		public event VertexAction<TVertex> FinishVertex;

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

		private void OnTreeEdge(TEdge e)
		{
			var eh = TreeEdge;
			if (eh != null)
				eh(e);
		}

		public event EdgeAction<TVertex, TEdge> NonTreeEdge;

		private void OnNonTreeEdge(TEdge e)
		{
			var eh = NonTreeEdge;
			if (eh != null)
				eh(e);
		}

		public event EdgeAction<TVertex, TEdge> GrayTarget;

		private void OnGrayTarget(TEdge e)
		{
			var eh = GrayTarget;
			if (eh != null)
				eh(e);
		}

		public event EdgeAction<TVertex, TEdge> BlackTarget;

		private void OnBlackTarget(TEdge e)
		{
			var eh = BlackTarget;
			if (eh != null)
				eh(e);
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

			var cancelManager = Services.CancelManager;
			if (cancelManager.IsCancelling)
				return;
			// initialize vertex u
			foreach (var v in VisitedGraph.Vertices)
			{
				VertexColors[v] = GraphColor.White;
				OnInitializeVertex(v);
			}
		}

		protected override void InternalCompute()
		{
			if (VisitedGraph.VertexCount == 0)
				return;

			TVertex rootVertex;
			if (!TryGetRootVertex(out rootVertex))
				// enqueue roots
				foreach (var root in VisitedGraph.Roots())
					EnqueueRoot(root);
			else // enqueue select root only
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
			var oee = OutEdgeEnumerator;

			while (vertexQueue.Count > 0)
			{
				if (cancelManager.IsCancelling) return;

				var u = vertexQueue.Dequeue();
				OnExamineVertex(u);
				foreach (var e in oee(VisitedGraph.OutEdges(u)))
				{
					var v = e.Target;
					OnExamineEdge(e);

					var vColor = VertexColors[v];
					if (vColor == GraphColor.White)
					{
						OnTreeEdge(e);
						VertexColors[v] = GraphColor.Gray;
						OnDiscoverVertex(v);
						vertexQueue.Enqueue(v);
					}
					else
					{
						OnNonTreeEdge(e);
						if (vColor == GraphColor.Gray)
							OnGrayTarget(e);
						else
							OnBlackTarget(e);
					}
				}

				VertexColors[u] = GraphColor.Black;
				OnFinishVertex(u);
			}
		}
	}
}