using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    /// <summary>
    ///     A distance recorder for directed tree builder algorithms
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    [Serializable]
	public sealed class VertexDistanceRecorderObserver<TVertex, TEdge>
		: IObserver<ITreeBuilderAlgorithm<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		public VertexDistanceRecorderObserver(Func<TEdge, double> edgeWeights)
			: this(edgeWeights, DistanceRelaxers.EdgeShortestDistance, new Dictionary<TVertex, double>())
		{
		}

		public VertexDistanceRecorderObserver(
			Func<TEdge, double> edgeWeights,
			IDistanceRelaxer distanceRelaxer,
			IDictionary<TVertex, double> distances)
		{
			Contract.Requires(edgeWeights != null);
			Contract.Requires(distanceRelaxer != null);
			Contract.Requires(distances != null);

			EdgeWeights = edgeWeights;
			DistanceRelaxer = distanceRelaxer;
			Distances = distances;
		}

		public IDistanceRelaxer DistanceRelaxer { get; }

		public Func<TEdge, double> EdgeWeights { get; }

		public IDictionary<TVertex, double> Distances { get; }

		public IDisposable Attach(ITreeBuilderAlgorithm<TVertex, TEdge> algorithm)
		{
			algorithm.TreeEdge += TreeEdge;
			return new DisposableAction(
				() => algorithm.TreeEdge -= TreeEdge
			);
		}

		private void TreeEdge(TEdge edge)
		{
			var source = edge.Source;
			var target = edge.Target;

			double sourceDistance;
			if (!Distances.TryGetValue(source, out sourceDistance))
				Distances[source] = sourceDistance = DistanceRelaxer.InitialDistance;
			Distances[target] = DistanceRelaxer.Combine(sourceDistance, EdgeWeights(edge));
		}
	}
}