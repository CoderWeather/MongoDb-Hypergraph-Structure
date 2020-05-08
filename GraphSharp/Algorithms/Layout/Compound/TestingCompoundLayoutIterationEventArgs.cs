using System.Collections.Generic;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Compound
{
	public class TestingCompoundLayoutIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
		: CompoundLayoutIterationEventArgs<TVertex, TEdge>,
			ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
		where TVertex : class
		where TEdge : IEdge<TVertex>
	{
		public TestingCompoundLayoutIterationEventArgs(
			int iteration,
			double statusInPercent,
			string message,
			IDictionary<TVertex, Point> vertexPositions,
			IDictionary<TVertex, Size> innerCanvasSizes,
			IDictionary<TVertex, TVertexInfo> vertexInfos,
			Point gravitationCenter)
			: base(iteration, statusInPercent, message, vertexPositions, innerCanvasSizes)
		{
			VertexInfos = vertexInfos;
			GravitationCenter = gravitationCenter;
		}

		public Point GravitationCenter { get; }

		public override object GetVertexInfo(TVertex vertex)
		{
			TVertexInfo info = default;
			if (VertexInfos.TryGetValue(vertex, out info))
				return info;

			return null;
		}

		public IDictionary<TVertex, TVertexInfo> VertexInfos { get; }

		public IDictionary<TEdge, TEdgeInfo> EdgeInfos => null;
	}
}