using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.RankedShortestPath
{
	public abstract class RankedShortestPathAlgorithmBase<TVertex, TEdge, TGraph>
		: RootedAlgorithmBase<TVertex, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IGraph<TVertex, TEdge>
	{
		private List<IEnumerable<TEdge>> computedShortestPaths;
		private int shortestPathCount = 3;

		protected RankedShortestPathAlgorithmBase(
			IAlgorithmComponent host,
			TGraph visitedGraph,
			IDistanceRelaxer distanceRelaxer)
			: base(host, visitedGraph)
		{
			Contract.Requires(distanceRelaxer != null);

			DistanceRelaxer = distanceRelaxer;
		}

		public int ShortestPathCount
		{
			get => shortestPathCount;
			set
			{
				Contract.Requires(value > 1);
				Contract.Ensures(ShortestPathCount == value);

				shortestPathCount = value;
			}
		}

		public int ComputedShortestPathCount
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() == ComputedShortestPaths.Count());

				return computedShortestPaths == null ? 0 : computedShortestPaths.Count;
			}
		}

		public IEnumerable<IEnumerable<TEdge>> ComputedShortestPaths
		{
			get
			{
				if (computedShortestPaths == null)
					yield break;
				foreach (var path in computedShortestPaths)
					yield return path;
			}
		}

		public IDistanceRelaxer DistanceRelaxer { get; }

		protected void AddComputedShortestPath(List<TEdge> path)
		{
			Contract.Requires(path != null);
			Contract.Requires(path.All(e => e != null));

			var pathArray = path.ToArray();
			computedShortestPaths.Add(pathArray);
		}

		protected override void Initialize()
		{
			base.Initialize();
			computedShortestPaths = new List<IEnumerable<TEdge>>(ShortestPathCount);
		}
	}
}