using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.ShortestPath
{
	[Serializable]
	public abstract class UndirectedShortestPathAlgorithmBase<TVertex, TEdge>
		: RootedAlgorithmBase<TVertex, IUndirectedGraph<TVertex, TEdge>>
			, IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		protected UndirectedShortestPathAlgorithmBase(
			IAlgorithmComponent host,
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights
		)
			: this(host, visitedGraph, weights, DistanceRelaxers.ShortestDistance)
		{
		}

		protected UndirectedShortestPathAlgorithmBase(
			IAlgorithmComponent host,
			IUndirectedGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights,
			IDistanceRelaxer distanceRelaxer
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(weights != null);
			Contract.Requires(distanceRelaxer != null);

			Weights = weights;
			DistanceRelaxer = distanceRelaxer;
		}

		public Dictionary<TVertex, GraphColor> VertexColors { get; private set; }

		public Dictionary<TVertex, double> Distances { get; private set; }

		public Func<TEdge, double> Weights { get; }

		public IDistanceRelaxer DistanceRelaxer { get; }

        /// <summary>
        ///     Invoked when the distance label for the target vertex is decreased.
        ///     The edge that participated in the last relaxation for vertex v is
        ///     an edge in the shortest paths tree.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> TreeEdge;

		public GraphColor GetVertexColor(TVertex vertex)
		{
			return VertexColors[vertex];
		}

		public bool TryGetDistance(TVertex vertex, out double distance)
		{
			Contract.Requires(vertex != null);
			return Distances.TryGetValue(vertex, out distance);
		}

		protected Func<TVertex, double> DistancesIndexGetter()
		{
			return AlgorithmExtensions.GetIndexer(Distances);
		}

		protected override void Initialize()
		{
			base.Initialize();
			VertexColors = new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount);
			Distances = new Dictionary<TVertex, double>(VisitedGraph.VertexCount);
		}

        /// <summary>
        ///     Raises the <see cref="TreeEdge" /> event.
        /// </summary>
        /// <param name="e">edge that raised the event</param>
        /// <param name="reversed"></param>
        protected virtual void OnTreeEdge(TEdge e, bool reversed)
		{
			var eh = TreeEdge;
			if (eh != null)
				eh(this, new UndirectedEdgeEventArgs<TVertex, TEdge>(e, reversed));
		}

		protected bool Relax(TEdge e, TVertex source, TVertex target)
		{
			Contract.Requires(e != null);
			Contract.Requires(source != null);
			Contract.Requires(target != null);
			Contract.Requires(
				e.Source.Equals(source) && e.Target.Equals(target)
			 || e.Source.Equals(target) && e.Target.Equals(source)
			);

			var du = Distances[source];
			var dv = Distances[target];
			var we = Weights(e);

			var relaxer = DistanceRelaxer;
			var duwe = relaxer.Combine(du, we);
			if (relaxer.Compare(duwe, dv) < 0)
			{
				Distances[target] = duwe;
				return true;
			}

			return false;
		}
	}
}