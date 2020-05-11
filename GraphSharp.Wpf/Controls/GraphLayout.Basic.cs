using System.ComponentModel;
using QuickGraph;

namespace GraphSharp.Controls
{
	/// <summary>
	///     For general purposes, with general types.
	/// </summary>
	public class GraphLayout : GraphLayout<object, IEdge<object>, IBidirectionalGraph<object, IEdge<object>>>
	{
		public GraphLayout()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
				return;

			var g = new BidirectionalGraph<object, IEdge<object>>();
			var vertices = new object[] {"S", "A", "M", "P", "L", "E"};
			var edges = new IEdge<object>[]
			{
				new Edge<object>(vertices[0], vertices[1]),
				new Edge<object>(vertices[1], vertices[2]),
				new Edge<object>(vertices[1], vertices[3]),
				new Edge<object>(vertices[3], vertices[4]),
				new Edge<object>(vertices[0], vertices[4]),
				new Edge<object>(vertices[4], vertices[5])
			};
			g.AddVerticesAndEdgeRange(edges);
			OverlapRemovalAlgorithmType = "FSA";
			LayoutAlgorithmType = "FR";
			Graph = g;
		}
	}
}