using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.Exploration
{
	public sealed class CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>
		: RootedAlgorithmBase<TVertex, IMutableVertexAndEdgeSet<TVertex, TEdge>>
			, ITreeBuilderAlgorithm<TVertex, TEdge>
		where TVertex : ICloneable, IComparable<TVertex>
		where TEdge : IEdge<TVertex>
	{
		private readonly Queue<TVertex> unexploredVertices = new Queue<TVertex>();

		public CloneableVertexGraphExplorerAlgorithm(
			IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph
		)
			: this(null, visitedGraph)
		{
		}

		public CloneableVertexGraphExplorerAlgorithm(
			IAlgorithmComponent host,
			IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph
		)
			: base(host, visitedGraph)
		{
		}

		public IList<ITransitionFactory<TVertex, TEdge>> TransitionFactories { get; } =
			new List<ITransitionFactory<TVertex, TEdge>>();

		public VertexPredicate<TVertex> AddVertexPredicate { get; set; } = v => true;

		public VertexPredicate<TVertex> ExploreVertexPredicate { get; set; } = v => true;

		public EdgePredicate<TVertex, TEdge> AddEdgePredicate { get; set; } = e => true;

		public Predicate<CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>> FinishedPredicate { get; set; } =
			new DefaultFinishedPredicate().Test;

		public IEnumerable<TVertex> UnexploredVertices => unexploredVertices;

		public bool FinishedSuccessfully { get; private set; }

		public event EdgeAction<TVertex, TEdge> TreeEdge;

		public event VertexAction<TVertex> DiscoverVertex;

		private void OnDiscoverVertex(TVertex v)
		{
			Contract.Requires(v != null);

			VisitedGraph.AddVertex(v);
			unexploredVertices.Enqueue(v);

			var eh = DiscoverVertex;
			if (eh != null)
				eh(v);
		}

		private void OnTreeEdge(TEdge e)
		{
			Contract.Requires(e != null);

			var eh = TreeEdge;
			if (eh != null)
				eh(e);
		}

		public event EdgeAction<TVertex, TEdge> BackEdge;

		private void OnBackEdge(TEdge e)
		{
			Contract.Requires(e != null);
			var eh = BackEdge;
			if (eh != null)
				eh(e);
		}

		public event EdgeAction<TVertex, TEdge> EdgeSkipped;

		private void OnEdgeSkipped(TEdge e)
		{
			Contract.Requires(e != null);
			var eh = EdgeSkipped;
			if (eh != null)
				eh(e);
		}

		protected override void InternalCompute()
		{
			TVertex rootVertex;
			if (!TryGetRootVertex(out rootVertex))
				throw new InvalidOperationException("RootVertex is not specified");

			VisitedGraph.Clear();
			unexploredVertices.Clear();
			FinishedSuccessfully = false;

			if (!AddVertexPredicate(rootVertex))
				throw new ArgumentException("StartVertex does not satisfy AddVertexPredicate");
			OnDiscoverVertex(rootVertex);

			while (unexploredVertices.Count > 0)
			{
				// are we done yet ?
				if (!FinishedPredicate(this))
				{
					FinishedSuccessfully = false;
					return;
				}

				var current = unexploredVertices.Dequeue();
				var clone = (TVertex) current.Clone();

				// let's make sure we want to explore this one
				if (!ExploreVertexPredicate(clone))
					continue;

				foreach (ITransitionFactory<TVertex, TEdge> transitionFactory in TransitionFactories)
					GenerateFromTransitionFactory(clone, transitionFactory);
			}

			FinishedSuccessfully = true;
		}

		private void GenerateFromTransitionFactory(
			TVertex current,
			ITransitionFactory<TVertex, TEdge> transitionFactory
		)
		{
			if (!transitionFactory.IsValid(current))
				return;

			foreach (var transition in transitionFactory.Apply(current))
			{
				if (
					!AddVertexPredicate(transition.Target)
				 || !AddEdgePredicate(transition))
				{
					OnEdgeSkipped(transition);
					continue;
				}

				var backEdge = VisitedGraph.ContainsVertex(transition.Target);
				if (!backEdge)
					OnDiscoverVertex(transition.Target);

				VisitedGraph.AddEdge(transition);
				if (backEdge)
					OnBackEdge(transition);
				else
					OnTreeEdge(transition);
			}
		}

		public sealed class DefaultFinishedPredicate
		{
			public DefaultFinishedPredicate()
			{
			}

			public DefaultFinishedPredicate(
				int maxVertexCount,
				int maxEdgeCount)
			{
				MaxVertexCount = maxVertexCount;
				MaxEdgeCount = maxEdgeCount;
			}

			public int MaxVertexCount { get; set; } = 1000;

			public int MaxEdgeCount { get; set; } = 1000;

			public bool Test(CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge> t)
			{
				if (t.VisitedGraph.VertexCount > MaxVertexCount)
					return false;
				if (t.VisitedGraph.EdgeCount > MaxEdgeCount)
					return false;
				return true;
			}
		}
	}
}