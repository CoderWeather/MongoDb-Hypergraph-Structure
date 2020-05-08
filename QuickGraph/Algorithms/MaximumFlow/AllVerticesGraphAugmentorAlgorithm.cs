using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.MaximumFlow
{
	public sealed class AllVerticesGraphAugmentorAlgorithm<TVertex, TEdge>
		: GraphAugmentorAlgorithmBase<TVertex, TEdge, IMutableVertexAndEdgeSet<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		public AllVerticesGraphAugmentorAlgorithm(
			IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
			VertexFactory<TVertex> vertexFactory,
			EdgeFactory<TVertex, TEdge> edgeFactory
		)
			: this(null, visitedGraph, vertexFactory, edgeFactory)
		{
		}

		public AllVerticesGraphAugmentorAlgorithm(
			IAlgorithmComponent host,
			IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
			VertexFactory<TVertex> vertexFactory,
			EdgeFactory<TVertex, TEdge> edgeFactory
		)
			: base(host, visitedGraph, vertexFactory, edgeFactory)
		{
		}

		protected override void AugmentGraph()
		{
			var cancelManager = Services.CancelManager;
			foreach (var v in VisitedGraph.Vertices)
			{
				if (cancelManager.IsCancelling) break;

				AddAugmentedEdge(SuperSource, v);
				AddAugmentedEdge(v, SuperSink);
			}
		}
	}
}