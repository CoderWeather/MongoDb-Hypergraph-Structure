using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
	[Serializable]
	public class FilteredGraph<TVertex, TEdge, TGraph> : IGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
		where TGraph : IGraph<TVertex, TEdge>
	{
		public FilteredGraph(
			TGraph baseGraph,
			VertexPredicate<TVertex> vertexPredicate,
			EdgePredicate<TVertex, TEdge> edgePredicate
		)
		{
			Contract.Requires(baseGraph != null);
			Contract.Requires(vertexPredicate != null);
			Contract.Requires(edgePredicate != null);

			BaseGraph = baseGraph;
			VertexPredicate = vertexPredicate;
			EdgePredicate = edgePredicate;
		}

        /// <summary>
        ///     Underlying filtered graph
        /// </summary>
        public TGraph BaseGraph { get; }

        /// <summary>
        ///     Edge predicate used to filter the edges
        /// </summary>
        public EdgePredicate<TVertex, TEdge> EdgePredicate { get; }

		public VertexPredicate<TVertex> VertexPredicate { get; }

		public bool IsDirected => BaseGraph.IsDirected;

		public bool AllowParallelEdges => BaseGraph.AllowParallelEdges;

		protected bool TestEdge(TEdge edge)
		{
			return VertexPredicate(edge.Source)
			 && VertexPredicate(edge.Target)
			 && EdgePredicate(edge);
		}
	}
}