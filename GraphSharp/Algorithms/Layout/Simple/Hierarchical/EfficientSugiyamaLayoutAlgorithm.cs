using System.Collections.Generic;
using System.Windows;
using GraphSharp.Algorithms.EdgeRouting;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Simple.Hierarchical
{
	public partial class EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
		: DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, EfficientSugiyamaLayoutParameters>,
			IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
        /// <summary>
        ///     Edges that has been involved in cycles in the original graph. (These has
        ///     been reverted during this layout algorithm).
        /// </summary>
        private readonly IList<TEdge> _cycleEdges = new List<TEdge>();

		private readonly IDictionary<TEdge, IList<SugiVertex>> _dummyVerticesOfEdges =
			new Dictionary<TEdge, IList<SugiVertex>>();

        /// <summary>
        ///     It stores the vertices or segments which inside the layers.
        /// </summary>
        private readonly IList<IList<SugiVertex>> _layers =
			new List<IList<SugiVertex>>();

		private readonly IDictionary<TVertex, SugiVertex> _vertexMap =
			new Dictionary<TVertex, SugiVertex>();

		private readonly IDictionary<TVertex, Size> _vertexSizes;

        /// <summary>
        ///     The copy of the VisitedGraph which should be laid out.
        /// </summary>
        private IMutableBidirectionalGraph<SugiVertex, SugiEdge> _graph;

        /// <summary>
        ///     Isolated vertices in the visited graph, which will be handled only in
        ///     the last step of the layout.
        /// </summary>
        private List<SugiVertex> _isolatedVertices;

		public EfficientSugiyamaLayoutAlgorithm(
			TGraph visitedGraph,
			EfficientSugiyamaLayoutParameters parameters,
			IDictionary<TVertex, Size> vertexSizes)
			: this(visitedGraph, parameters, null, vertexSizes)
		{
		}

		public EfficientSugiyamaLayoutAlgorithm(
			TGraph visitedGraph,
			EfficientSugiyamaLayoutParameters parameters,
			IDictionary<TVertex, Point> vertexPositions,
			IDictionary<TVertex, Size> vertexSizes)
			: base(visitedGraph, vertexPositions, parameters)
		{
			_vertexSizes = vertexSizes;
		}


		#region IEdgeRoutingAlgorithm<TVertex,TEdge,TGraph> Members

        /// <summary>
        ///     Routing points for the edges of the original graph.
        /// </summary>
        public IDictionary<TEdge, Point[]> EdgeRoutes { get; } = new Dictionary<TEdge, Point[]>();

		#endregion

        /// <summary>
        ///     Initializes the private _graph field which stores the graph that
        ///     we operate on.
        /// </summary>
        private void InitTheGraph()
		{
			//make a copy of the original graph
			_graph = new BidirectionalGraph<SugiVertex, SugiEdge>();

			//copy the vertices
			foreach (var vertex in VisitedGraph.Vertices)
			{
				var size = new Size();
				if (_vertexSizes != null)
					_vertexSizes.TryGetValue(vertex, out size);

				var vertexWrapper = new SugiVertex(vertex, size);
				_graph.AddVertex(vertexWrapper);
				_vertexMap[vertex] = vertexWrapper;
			}

			//copy the edges
			foreach (var edge in VisitedGraph.Edges)
			{
				var edgeWrapper = new SugiEdge(edge, _vertexMap[edge.Source], _vertexMap[edge.Target]);
				_graph.AddEdge(edgeWrapper);
			}
		}

		protected override void InternalCompute()
		{
			InitTheGraph();

			//first step
			DoPreparing();

			BuildSparseNormalizedGraph();
			DoCrossingMinimizations();
			CalculatePositions();
		}
	}
}