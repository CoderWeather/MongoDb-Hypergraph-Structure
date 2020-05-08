using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickGraph.Algorithms.RandomWalks
{
	[Serializable]
	public sealed class NormalizedMarkovEdgeChain<TVertex, TEdge> :
		MarkovEdgeChainBase<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		public override bool TryGetSuccessor(IImplicitGraph<TVertex, TEdge> g, TVertex u, out TEdge successor)
		{
			var outDegree = g.OutDegree(u);
			if (outDegree > 0)
			{
				var index = Rand.Next(0, outDegree);
				successor = g.OutEdge(u, index);
				return true;
			}

			successor = default;
			return false;
		}

		public override bool TryGetSuccessor(IEnumerable<TEdge> edges, TVertex u, out TEdge successor)
		{
			var edgeCount = edges.Count();

			if (edgeCount > 0)
			{
				var index = Rand.Next(0, edgeCount);
				successor = edges.ElementAt(index);
				return true;
			}

			successor = default;
			return false;
		}
	}
}