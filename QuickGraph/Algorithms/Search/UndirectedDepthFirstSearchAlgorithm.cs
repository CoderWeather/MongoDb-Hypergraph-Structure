using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    ///     A depth first search algorithm for directed graph
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2" />
    [Serializable]
	public sealed class UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge> :
		RootedAlgorithmBase<TVertex, IUndirectedGraph<TVertex, TEdge>>,
		IDistanceRecorderAlgorithm<TVertex, TEdge>,
		IVertexColorizerAlgorithm<TVertex, TEdge>,
		IUndirectedVertexPredecessorRecorderAlgorithm<TVertex, TEdge>,
		IVertexTimeStamperAlgorithm<TVertex, TEdge>,
		IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private int maxDepth = int.MaxValue;

        /// <summary>
        ///     Initializes a new instance of the algorithm.
        /// </summary>
        /// <param name="visitedGraph">visited graph</param>
        public UndirectedDepthFirstSearchAlgorithm(IUndirectedGraph<TVertex, TEdge> visitedGraph)
			: this(visitedGraph, new Dictionary<TVertex, GraphColor>())
		{
		}

        /// <summary>
        ///     Initializes a new instance of the algorithm.
        /// </summary>
        /// <param name="visitedGraph">visited graph</param>
        /// <param name="colors">vertex color map</param>
        public UndirectedDepthFirstSearchAlgorithm(
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, GraphColor> colors
		)
			: this(null, visitedGraph, colors)
		{
		}

        /// <summary>
        ///     Initializes a new instance of the algorithm.
        /// </summary>
        /// <param name="host">algorithm host</param>
        /// <param name="visitedGraph">visited graph</param>
        /// <param name="colors">vertex color map</param>
        public UndirectedDepthFirstSearchAlgorithm(
			IAlgorithmComponent host,
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, GraphColor> colors
		)
			: this(host, visitedGraph, colors, e => e)
		{
		}

        /// <summary>
        ///     Initializes a new instance of the algorithm.
        /// </summary>
        /// <param name="host">algorithm host</param>
        /// <param name="visitedGraph">visited graph</param>
        /// <param name="colors">vertex color map</param>
        /// <param name="adjacentEdgeEnumerator">
        ///     Delegate that takes the enumeration of out-edges and reorders
        ///     them. All vertices passed to the method should be enumerated once and only once.
        ///     May be null.
        /// </param>
        public UndirectedDepthFirstSearchAlgorithm(
			IAlgorithmComponent host,
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, GraphColor> colors,
			Func<IEnumerable<TEdge>, IEnumerable<TEdge>> adjacentEdgeEnumerator
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(colors != null);
			Contract.Requires(adjacentEdgeEnumerator != null);

			VertexColors = colors;
			AdjacentEdgeEnumerator = adjacentEdgeEnumerator;
		}

		public IDictionary<TVertex, GraphColor> VertexColors { get; }

		public Func<IEnumerable<TEdge>, IEnumerable<TEdge>> AdjacentEdgeEnumerator { get; }

		public int MaxDepth
		{
			get => maxDepth;
			set
			{
				Contract.Requires(value > 0);
				maxDepth = value;
			}
		}

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
			Contract.Requires(v != null);

			var eh = InitializeVertex;
			if (eh != null)
				eh(v);
		}

		private void OnStartVertex(TVertex v)
		{
			Contract.Requires(v != null);

			var eh = StartVertex;
			if (eh != null)
				eh(v);
		}

		public event VertexAction<TVertex> VertexMaxDepthReached;

		private void OnVertexMaxDepthReached(TVertex v)
		{
			Contract.Requires(v != null);

			var eh = VertexMaxDepthReached;
			if (eh != null)
				eh(v);
		}

		private void OnDiscoverVertex(TVertex v)
		{
			var eh = DiscoverVertex;
			if (eh != null)
				eh(v);
		}

		public event UndirectedEdgeAction<TVertex, TEdge> ExamineEdge;

		private void OnExamineEdge(TEdge e, bool reversed)
		{
			var eh = ExamineEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		private void OnTreeEdge(TEdge e, bool reversed)
		{
			var eh = TreeEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		public event UndirectedEdgeAction<TVertex, TEdge> BackEdge;

		private void OnBackEdge(TEdge e, bool reversed)
		{
			var eh = BackEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		public event UndirectedEdgeAction<TVertex, TEdge> ForwardOrCrossEdge;

		private void OnForwardOrCrossEdge(TEdge e, bool reversed)
		{
			var eh = ForwardOrCrossEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		private void OnFinishVertex(TVertex v)
		{
			var eh = FinishVertex;
			if (eh != null)
				eh(v);
		}

		protected override void InternalCompute()
		{
			// if there is a starting vertex, start whith him:
			TVertex rootVertex;
			if (TryGetRootVertex(out rootVertex))
			{
				OnStartVertex(rootVertex);
				Visit(rootVertex);
			}
			else
			{
				var cancelManager = Services.CancelManager;
				// process each vertex 
				foreach (var u in VisitedGraph.Vertices)
				{
					if (cancelManager.IsCancelling)
						return;
					if (VertexColors[u] == GraphColor.White)
					{
						OnStartVertex(u);
						Visit(u);
					}
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

		public void Visit(TVertex root)
		{
			Contract.Requires(root != null);

			var todo = new Stack<SearchFrame>();
			var oee = AdjacentEdgeEnumerator;
			var visitedEdges = new Dictionary<TEdge, int>(VisitedGraph.EdgeCount);

			VertexColors[root] = GraphColor.Gray;
			OnDiscoverVertex(root);

			var cancelManager = Services.CancelManager;
			var enumerable = oee(VisitedGraph.AdjacentEdges(root));
			todo.Push(new SearchFrame(root, enumerable.GetEnumerator(), 0));
			while (todo.Count > 0)
			{
				if (cancelManager.IsCancelling) return;

				var frame = todo.Pop();
				var u = frame.Vertex;
				var depth = frame.Depth;

				if (depth > MaxDepth)
				{
					OnVertexMaxDepthReached(u);
					VertexColors[u] = GraphColor.Black;
					OnFinishVertex(u);
					continue;
				}

				var edges = frame.Edges;
				while (edges.MoveNext())
				{
					var e = edges.Current;
					if (cancelManager.IsCancelling) return;
					if (visitedEdges.ContainsKey(e)) continue; // edge already visited

					visitedEdges.Add(e, 0);
					var reversed = e.Target.Equals(u);
					OnExamineEdge(e, reversed);
					var v = reversed ? e.Source : e.Target;
					var c = VertexColors[v];
					switch (c)
					{
						case GraphColor.White:
							OnTreeEdge(e, reversed);
							todo.Push(new SearchFrame(u, edges, frame.Depth + 1));
							u = v;
							edges = oee(VisitedGraph.AdjacentEdges(u)).GetEnumerator();
							VertexColors[u] = GraphColor.Gray;
							OnDiscoverVertex(u);
							break;
						case GraphColor.Gray:
							OnBackEdge(e, reversed);
							break;
						case GraphColor.Black:
							OnForwardOrCrossEdge(e, reversed);
							break;
					}
				}

				VertexColors[u] = GraphColor.Black;
				OnFinishVertex(u);
			}
		}

		private readonly struct SearchFrame
		{
			public readonly TVertex Vertex;
			public readonly IEnumerator<TEdge> Edges;
			public readonly int Depth;

			public SearchFrame(
				TVertex vertex,
				IEnumerator<TEdge> edges,
				int depth)
			{
				Contract.Requires(vertex != null);
				Contract.Requires(edges != null);
				Contract.Requires(depth >= 0);

				Vertex = vertex;
				Edges = edges;
				Depth = depth;
			}
		}
	}
}