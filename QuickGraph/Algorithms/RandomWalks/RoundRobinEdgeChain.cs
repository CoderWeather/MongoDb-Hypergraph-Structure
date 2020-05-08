using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickGraph.Algorithms.RandomWalks
{
	[Serializable]
	public sealed class RoundRobinEdgeChain<TVertex, TEdge>
		: IEdgeChain<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private Dictionary<TVertex, int> outEdgeIndices = new Dictionary<TVertex, int>();

		public bool TryGetSuccessor(IImplicitGraph<TVertex, TEdge> g, TVertex u, out TEdge successor)
		{
			var outDegree = g.OutDegree(u);
			if (outDegree > 0)
			{
				int index;
				if (!outEdgeIndices.TryGetValue(u, out index))
				{
					index = 0;
					outEdgeIndices.Add(u, index);
				}

				var e = g.OutEdge(u, index);
				outEdgeIndices[u] = ++index % outDegree;

				successor = e;
				return true;
			}

			successor = default;
			return false;
		}

		public bool TryGetSuccessor(IEnumerable<TEdge> edges, TVertex u, out TEdge successor)
		{
			var edgeCount = edges.Count();

			if (edgeCount > 0)
			{
				int index;
				if (!outEdgeIndices.TryGetValue(u, out index))
				{
					index = 0;
					outEdgeIndices.Add(u, index);
				}

				var e = edges.ElementAt(index);
				outEdgeIndices[u] = ++index % edgeCount;
				successor = e;
			}

			successor = default;
			return false;
		}
	}
}