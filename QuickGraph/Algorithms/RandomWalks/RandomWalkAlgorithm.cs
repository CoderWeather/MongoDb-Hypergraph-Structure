using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.RandomWalks
{
	[Serializable]
	public sealed class RandomWalkAlgorithm<TVertex, TEdge>
		: ITreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private IEdgeChain<TVertex, TEdge> edgeChain;
		private EdgePredicate<TVertex, TEdge> endPredicate;

		public RandomWalkAlgorithm(IImplicitGraph<TVertex, TEdge> visitedGraph)
			: this(visitedGraph, new NormalizedMarkovEdgeChain<TVertex, TEdge>())
		{
		}

		public RandomWalkAlgorithm(
			IImplicitGraph<TVertex, TEdge> visitedGraph,
			IEdgeChain<TVertex, TEdge> edgeChain
		)
		{
			Contract.Requires(visitedGraph != null);
			Contract.Requires(edgeChain != null);

			VisitedGraph = visitedGraph;
			this.edgeChain = edgeChain;
		}

		public IImplicitGraph<TVertex, TEdge> VisitedGraph { get; }

		public IEdgeChain<TVertex, TEdge> EdgeChain
		{
			get => edgeChain;
			set
			{
				Contract.Requires(value != null);

				edgeChain = value;
			}
		}

		public EdgePredicate<TVertex, TEdge> EndPredicate
		{
			get => endPredicate;
			set => endPredicate = value;
		}

		public event EdgeAction<TVertex, TEdge> TreeEdge;

		public event VertexAction<TVertex> StartVertex;

		private void OnStartVertex(TVertex v)
		{
			if (StartVertex != null)
				StartVertex(v);
		}

		public event VertexAction<TVertex> EndVertex;

		private void OnEndVertex(TVertex v)
		{
			if (EndVertex != null)
				EndVertex(v);
		}

		private void OnTreeEdge(TEdge e)
		{
			if (TreeEdge != null)
				TreeEdge(e);
		}

		private bool TryGetSuccessor(TVertex u, out TEdge successor)
		{
			return EdgeChain.TryGetSuccessor(VisitedGraph, u, out successor);
		}

		public void Generate(TVertex root)
		{
			Contract.Requires(root != null);

			Generate(root, 100);
		}

		public void Generate(TVertex root, int walkCount)
		{
			Contract.Requires(root != null);

			var count = 0;
			TEdge e = default;
			var v = root;

			OnStartVertex(root);
			while (count < walkCount && TryGetSuccessor(v, out e))
			{
				// if dead end stop
				if (e == null)
					break;
				// if end predicate, test
				if (endPredicate != null && endPredicate(e))
					break;
				OnTreeEdge(e);
				v = e.Target;
				// upgrade count
				++count;
			}

			OnEndVertex(v);
		}
	}
}