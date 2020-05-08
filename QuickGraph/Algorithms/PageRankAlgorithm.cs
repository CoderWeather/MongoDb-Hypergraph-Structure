using System;
using System.Collections.Generic;
using QuickGraph.Predicates;

namespace QuickGraph.Algorithms.Ranking
{
	[Serializable]
	public sealed class PageRankAlgorithm<TVertex, TEdge> :
		AlgorithmBase<IBidirectionalGraph<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private double damping = 0.85;

		private int maxIterations = 60;
		private double tolerance = 2 * double.Epsilon;

		public PageRankAlgorithm(IBidirectionalGraph<TVertex, TEdge> visitedGraph)
			: base(visitedGraph)
		{
		}

		public IDictionary<TVertex, double> Ranks { get; private set; } = new Dictionary<TVertex, double>();

		public double Damping
		{
			get => damping;
			set => damping = value;
		}

		public double Tolerance
		{
			get => tolerance;
			set => tolerance = value;
		}

		public int MaxIteration
		{
			get => maxIterations;
			set => maxIterations = value;
		}

		public void InitializeRanks()
		{
			Ranks.Clear();
			foreach (var v in VisitedGraph.Vertices) Ranks.Add(v, 0);
			//            this.RemoveDanglingLinks();
		}

/*
        public void RemoveDanglingLinks()
        {
            VertexCollection danglings = new VertexCollection();
            do
            {
                danglings.Clear();

                // create filtered graph
                IVertexListGraph fg = new FilteredVertexListGraph(
                    this.VisitedGraph,
                    new InDictionaryVertexPredicate(this.ranks)
                    );

                // iterate over of the vertices in the rank map
                foreach (IVertex v in this.ranks.Keys)
                {
                    // if v does not have out-edge in the filtered graph, remove
                    if (fg.OutDegree(v) == 0)
                        danglings.Add(v);
                }

                // remove from ranks
                foreach (IVertex v in danglings)
                    this.ranks.Remove(v);
                // iterate until no dangling was removed
            } while (danglings.Count != 0);
        }
*/
		protected override void InternalCompute()
		{
			var cancelManager = Services.CancelManager;
			IDictionary<TVertex, double> tempRanks = new Dictionary<TVertex, double>();

			// create filtered graph
			var fg = new FilteredBidirectionalGraph<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
				VisitedGraph,
				new InDictionaryVertexPredicate<TVertex, double>(Ranks).Test,
				e => true
			);

			var iter = 0;
			double error = 0;
			do
			{
				if (cancelManager.IsCancelling)
					return;

				// compute page ranks
				error = 0;
				foreach (var de in Ranks)
				{
					if (cancelManager.IsCancelling)
						return;

					var v = de.Key;
					var rank = de.Value;
					// compute ARi
					double r = 0;
					foreach (var e in fg.InEdges(v)) r += Ranks[e.Source] / fg.OutDegree(e.Source);

					// add sourceRank and store
					var newRank = 1 - damping + damping * r;
					tempRanks[v] = newRank;
					// compute deviation
					error += Math.Abs(rank - newRank);
				}

				// swap ranks
				var temp = Ranks;
				Ranks = tempRanks;
				tempRanks = temp;

				iter++;
			} while (error > tolerance && iter < maxIterations);
		}

		public double GetRanksSum()
		{
			double sum = 0;
			foreach (var rank in Ranks.Values) sum += rank;
			return sum;
		}

		public double GetRanksMean()
		{
			return GetRanksSum() / Ranks.Count;
		}
	}
}