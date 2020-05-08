using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    ///     A depth and height first search algorithm for directed graphs
    /// </summary>
    /// <remarks>
    ///     This is a modified version of the classic DFS algorithm
    ///     where the search is performed both in depth and height.
    /// </remarks>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2" />
    [Serializable]
	public sealed class BidirectionalDepthFirstSearchAlgorithm<TVertex, TEdge> :
		RootedAlgorithmBase<TVertex, IBidirectionalGraph<TVertex, TEdge>>,
		IVertexColorizerAlgorithm<TVertex, TEdge>,
		ITreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private int maxDepth = int.MaxValue;

		public BidirectionalDepthFirstSearchAlgorithm(IBidirectionalGraph<TVertex, TEdge> g)
			: this(g, new Dictionary<TVertex, GraphColor>())
		{
		}

		public BidirectionalDepthFirstSearchAlgorithm(
			IBidirectionalGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, GraphColor> colors
		)
			: this(null, visitedGraph, colors)
		{
		}

		public BidirectionalDepthFirstSearchAlgorithm(
			IAlgorithmComponent host,
			IBidirectionalGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, GraphColor> colors
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(colors != null);

			VertexColors = colors;
		}

		public IDictionary<TVertex, GraphColor> VertexColors { get; }

		public int MaxDepth
		{
			get => maxDepth;
			set => maxDepth = value;
		}

		public event EdgeAction<TVertex, TEdge> TreeEdge;

		public GraphColor GetVertexColor(TVertex vertex)
		{
			return VertexColors[vertex];
		}

		public event VertexAction<TVertex> InitializeVertex;

		private void OnInitializeVertex(TVertex v)
		{
			var eh = InitializeVertex;
			if (eh != null)
				eh(v);
		}

		public event VertexAction<TVertex> StartVertex;

		private void OnStartVertex(TVertex v)
		{
			var eh = StartVertex;
			if (eh != null)
				eh(v);
		}

		public event VertexAction<TVertex> DiscoverVertex;

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

		public event EdgeAction<TVertex, TEdge> BackEdge;

		private void OnBackEdge(TEdge e)
		{
			var eh = BackEdge;
			if (eh != null)
				eh(e);
		}

		public event EdgeAction<TVertex, TEdge> ForwardOrCrossEdge;

		private void OnForwardOrCrossEdge(TEdge e)
		{
			var eh = ForwardOrCrossEdge;
			if (eh != null)
				eh(e);
		}

		public event VertexAction<TVertex> FinishVertex;

		private void OnFinishVertex(TVertex v)
		{
			var eh = FinishVertex;
			if (eh != null)
				eh(v);
		}

		protected override void InternalCompute()
		{
			// put all vertex to white
			Initialize();

			// if there is a starting vertex, start whith him:
			TVertex rootVertex;
			if (TryGetRootVertex(out rootVertex))
			{
				OnStartVertex(rootVertex);
				Visit(rootVertex, 0);
			}

			// process each vertex 
			var cancelManager = Services.CancelManager;
			foreach (var u in VisitedGraph.Vertices)
			{
				if (cancelManager.IsCancelling) return;
				if (VertexColors[u] == GraphColor.White)
				{
					OnStartVertex(u);
					Visit(u, 0);
				}
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			VertexColors.Clear();
			foreach (var u in VisitedGraph.Vertices)
			{
				VertexColors[u] = GraphColor.White;
				OnInitializeVertex(u);
			}
		}

		public void Visit(TVertex u, int depth)
		{
			Contract.Requires(u != null);

			if (depth > maxDepth)
				return;

			VertexColors[u] = GraphColor.Gray;
			OnDiscoverVertex(u);

			var cancelManager = Services.CancelManager;
			TVertex v = default;
			foreach (var e in VisitedGraph.OutEdges(u))
			{
				if (cancelManager.IsCancelling) return;

				OnExamineEdge(e);
				v = e.Target;
				ProcessEdge(depth, v, e);
			}

			foreach (var e in VisitedGraph.InEdges(u))
			{
				if (cancelManager.IsCancelling) return;

				OnExamineEdge(e);
				v = e.Source;
				ProcessEdge(depth, v, e);
			}

			VertexColors[u] = GraphColor.Black;
			OnFinishVertex(u);
		}

		private void ProcessEdge(int depth, TVertex v, TEdge e)
		{
			var c = VertexColors[v];
			if (c == GraphColor.White)
			{
				OnTreeEdge(e);
				Visit(v, depth + 1);
			}
			else if (c == GraphColor.Gray)
			{
				OnBackEdge(e);
			}
			else
			{
				OnForwardOrCrossEdge(e);
			}
		}
	}
}