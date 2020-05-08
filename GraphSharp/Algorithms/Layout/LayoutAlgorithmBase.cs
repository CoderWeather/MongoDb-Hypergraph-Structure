using System.Collections.Generic;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout
{
	public abstract class LayoutAlgorithmBase<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo> :
		LayoutAlgorithmBase<TVertex, TEdge, TGraph>, ILayoutAlgorithm<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		protected readonly IDictionary<TEdge, TEdgeInfo> edgeInfos = new Dictionary<TEdge, TEdgeInfo>();
		protected readonly IDictionary<TVertex, TVertexInfo> vertexInfos = new Dictionary<TVertex, TVertexInfo>();

		protected LayoutAlgorithmBase(TGraph visitedGraph)
			: base(visitedGraph)
		{
		}

		protected LayoutAlgorithmBase(TGraph visitedGraph, IDictionary<TVertex, Point> vertexPositions)
			: base(visitedGraph, vertexPositions)
		{
		}

		public IDictionary<TVertex, TVertexInfo> VertexInfos => vertexInfos;

		public IDictionary<TEdge, TEdgeInfo> EdgeInfos => edgeInfos;

		public new event LayoutIterationEndedEventHandler<TVertex, TEdge, TVertexInfo, TEdgeInfo> IterationEnded;

		public override object GetVertexInfo(TVertex vertex)
		{
			TVertexInfo info;
			if (VertexInfos.TryGetValue(vertex, out info))
				return info;

			return null;
		}

		public override object GetEdgeInfo(TEdge edge)
		{
			TEdgeInfo info;
			if (EdgeInfos.TryGetValue(edge, out info))
				return info;

			return null;
		}

		protected void OnIterationEnded(ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo> e)
		{
			IterationEnded?.Invoke(this, e);
		}
	}

	public abstract class LayoutAlgorithmBase<TVertex, TEdge, TGraph> : AlgorithmBase,
		ILayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		private readonly Dictionary<TVertex, Point> vertexPositions;

		protected LayoutAlgorithmBase(TGraph visitedGraph) :
			this(visitedGraph, null)
		{
		}

		protected LayoutAlgorithmBase(TGraph visitedGraph, IDictionary<TVertex, Point> vertexPositions)
		{
			VisitedGraph = visitedGraph;
			if (vertexPositions != null)
				this.vertexPositions = new Dictionary<TVertex, Point>(vertexPositions);
			else
				this.vertexPositions = new Dictionary<TVertex, Point>(visitedGraph.VertexCount);
		}

		public bool ReportOnIterationEndNeeded => IterationEnded != null;

		public bool ReportOnProgressChangedNeeded => ProgressChanged != null;

		public IDictionary<TVertex, Point> VertexPositions => vertexPositions;

		public TGraph VisitedGraph { get; }

		public event LayoutIterationEndedEventHandler<TVertex, TEdge> IterationEnded;

		public event ProgressChangedEventHandler ProgressChanged;

		public virtual object GetVertexInfo(TVertex vertex)
		{
			return null;
		}

		public virtual object GetEdgeInfo(TEdge edge)
		{
			return null;
		}

		protected virtual void OnIterationEnded(ILayoutIterationEventArgs<TVertex> args)
		{
			if (IterationEnded != null)
			{
				IterationEnded(this, args);

				//if the layout should be aborted
				if (args.Abort)
					Abort();
			}
		}

		protected virtual void OnProgressChanged(double percent)
		{
			if (ProgressChanged != null)
				ProgressChanged(this, percent);
		}
	}
}