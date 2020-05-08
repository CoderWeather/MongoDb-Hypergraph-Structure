using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.ShortestPath
{
	[Serializable]
	public abstract class ShortestPathAlgorithmBase<TVertex, TEdge, TGraph>
		: RootedAlgorithmBase<TVertex, TGraph>
			, ITreeBuilderAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexSet<TVertex>
	{
		protected ShortestPathAlgorithmBase(
			IAlgorithmComponent host,
			TGraph visitedGraph,
			Func<TEdge, double> weights
		)
			: this(host, visitedGraph, weights, DistanceRelaxers.ShortestDistance)
		{
		}

		protected ShortestPathAlgorithmBase(
			IAlgorithmComponent host,
			TGraph visitedGraph,
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
        public event EdgeAction<TVertex, TEdge> TreeEdge;

		public GraphColor GetVertexColor(TVertex vertex)
		{
			Contract.Assert(Distances != null);

			return
				VertexColors[vertex];
		}

		public bool TryGetDistance(TVertex vertex, out double distance)
		{
			Contract.Requires(vertex != null);
			Contract.Assert(Distances != null);

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
        protected virtual void OnTreeEdge(TEdge e)
		{
			var eh = TreeEdge;
			if (eh != null)
				eh(e);
		}

		protected bool Relax(TEdge e)
		{
			Contract.Requires(e != null);

			var source = e.Source;
			var target = e.Target;
			if (source.Equals(target))
				return false;

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