using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
	[Serializable]
	public sealed class ReversedResidualEdgePredicate<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		public ReversedResidualEdgePredicate(
			IDictionary<TEdge, double> residualCapacities,
			IDictionary<TEdge, TEdge> reversedEdges)
		{
			Contract.Requires(residualCapacities != null);
			Contract.Requires(reversedEdges != null);

			ResidualCapacities = residualCapacities;
			ReversedEdges = reversedEdges;
		}

        /// <summary>
        ///     Residual capacities map
        /// </summary>
        public IDictionary<TEdge, double> ResidualCapacities { get; }

        /// <summary>
        ///     Reversed edges map
        /// </summary>
        public IDictionary<TEdge, TEdge> ReversedEdges { get; }

		public bool Test(TEdge e)
		{
			Contract.Requires(e != null);
			return 0 < ResidualCapacities[ReversedEdges[e]];
		}
	}
}