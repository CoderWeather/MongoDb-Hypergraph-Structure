﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.RandomWalks
{
    /// <summary>
    ///     Wilson-Propp Cycle-Popping Algorithm for Random Tree Generation.
    /// </summary>
    [Serializable]
	public sealed class CyclePoppingRandomTreeAlgorithm<TVertex, TEdge>
		: RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>
			, IVertexColorizerAlgorithm<TVertex, TEdge>
			, ITreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private Random rnd = new Random((int) DateTime.Now.Ticks);

		public CyclePoppingRandomTreeAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph)
			: this(visitedGraph, new NormalizedMarkovEdgeChain<TVertex, TEdge>())
		{
		}

		public CyclePoppingRandomTreeAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IMarkovEdgeChain<TVertex, TEdge> edgeChain)
			: this(null, visitedGraph, edgeChain)
		{
		}

		public CyclePoppingRandomTreeAlgorithm(
			IAlgorithmComponent host,
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IMarkovEdgeChain<TVertex, TEdge> edgeChain
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(edgeChain != null);
			EdgeChain = edgeChain;
		}

		public IDictionary<TVertex, GraphColor> VertexColors { get; } = new Dictionary<TVertex, GraphColor>();

		public IMarkovEdgeChain<TVertex, TEdge> EdgeChain { get; } = new NormalizedMarkovEdgeChain<TVertex, TEdge>();

        /// <summary>
        ///     Gets or sets the random number generator used in <c>RandomTree</c>.
        /// </summary>
        /// <value>
        ///     <see cref="Random" /> number generator
        /// </value>
        public Random Rnd
		{
			get => rnd;
			set
			{
				Contract.Requires(value != null);
				rnd = value;
			}
		}

		public IDictionary<TVertex, TEdge> Successors { get; } = new Dictionary<TVertex, TEdge>();

		public event EdgeAction<TVertex, TEdge> TreeEdge;

		public GraphColor GetVertexColor(TVertex v)
		{
			return VertexColors[v];
		}

		public event VertexAction<TVertex> InitializeVertex;

		private void OnInitializeVertex(TVertex v)
		{
			var eh = InitializeVertex;
			if (eh != null)
				eh(v);
		}

		public event VertexAction<TVertex> FinishVertex;

		private void OnFinishVertex(TVertex v)
		{
			var eh = FinishVertex;
			if (eh != null)
				eh(v);
		}

		private void OnTreeEdge(TEdge e)
		{
			var eh = TreeEdge;
			if (eh != null)
				eh(e);
		}

		public event VertexAction<TVertex> ClearTreeVertex;

		private void OnClearTreeVertex(TVertex v)
		{
			var eh = ClearTreeVertex;
			if (eh != null)
				eh(v);
		}

		protected override void Initialize()
		{
			base.Initialize();

			Successors.Clear();
			VertexColors.Clear();
			foreach (var v in VisitedGraph.Vertices)
			{
				VertexColors.Add(v, GraphColor.White);
				OnInitializeVertex(v);
			}
		}

		private bool NotInTree(TVertex u)
		{
			var color = VertexColors[u];
			return color == GraphColor.White;
		}

		private void SetInTree(TVertex u)
		{
			VertexColors[u] = GraphColor.Black;
			OnFinishVertex(u);
		}

		private bool TryGetSuccessor(Dictionary<TEdge, int> visited, TVertex u, out TEdge successor)
		{
			var outEdges = VisitedGraph.OutEdges(u);
			var edges = outEdges.Where(e => !visited.ContainsKey(e));
			return EdgeChain.TryGetSuccessor(edges, u, out successor);
		}

		private void Tree(TVertex u, TEdge next)
		{
			Contract.Requires(next != null);

			Successors[u] = next;
			OnTreeEdge(next);
		}

		private bool TryGetNextInTree(TVertex u, out TVertex next)
		{
			TEdge nextEdge;
			if (Successors.TryGetValue(u, out nextEdge))
			{
				next = nextEdge.Target;
				return true;
			}

			next = default;
			return false;
		}

		private bool Chance(double eps)
		{
			return rnd.NextDouble() <= eps;
		}

		private void ClearTree(TVertex u)
		{
			Successors[u] = default;
			OnClearTreeVertex(u);
		}

		public void RandomTreeWithRoot(TVertex root)
		{
			Contract.Requires(root != null);
			Contract.Requires(VisitedGraph.ContainsVertex(root));

			SetRootVertex(root);
			Compute();
		}

		protected override void InternalCompute()
		{
			TVertex rootVertex;
			if (!TryGetRootVertex(out rootVertex))
				throw new InvalidOperationException("RootVertex not specified");

			var cancelManager = Services.CancelManager;
			// process root
			ClearTree(rootVertex);
			SetInTree(rootVertex);

			foreach (var i in VisitedGraph.Vertices)
			{
				if (cancelManager.IsCancelling) break;
				// first pass exploring
				{
					var visited = new Dictionary<TEdge, int>();
					var u = i;
					TEdge successor;
					while (NotInTree(u) &&
						TryGetSuccessor(visited, u, out successor))
					{
						visited[successor] = 0;
						Tree(u, successor);
						if (!TryGetNextInTree(u, out u))
							break;
					}
				}

				// second pass, coloring
				{
					var u = i;
					while (NotInTree(u))
					{
						SetInTree(u);
						if (!TryGetNextInTree(u, out u))
							break;
					}
				}
			}
		}

		public void RandomTree()
		{
			var cancelManager = Services.CancelManager;

			double eps = 1;
			bool success;
			do
			{
				if (cancelManager.IsCancelling) break;

				eps /= 2;
				success = Attempt(eps);
			} while (!success);
		}

		private bool Attempt(double eps)
		{
			Initialize();
			var numRoots = 0;
			var cancelManager = Services.CancelManager;

			foreach (var i in VisitedGraph.Vertices)
			{
				if (cancelManager.IsCancelling) break;

				// first pass exploring
				{
					var visited = new Dictionary<TEdge, int>();
					TEdge successor;
					var u = i;
					while (NotInTree(u))
						if (Chance(eps))
						{
							ClearTree(u);
							SetInTree(u);
							++numRoots;
							if (numRoots > 1)
								return false;
						}
						else
						{
							if (!TryGetSuccessor(visited, u, out successor))
								break;
							visited[successor] = 0;
							Tree(u, successor);
							if (!TryGetNextInTree(u, out u))
								break;
						}
				}

				// second pass, coloring
				{
					var u = i;
					while (NotInTree(u))
					{
						SetInTree(u);
						if (!TryGetNextInTree(u, out u))
							break;
					}
				}
			}

			return true;
		}
	}
}