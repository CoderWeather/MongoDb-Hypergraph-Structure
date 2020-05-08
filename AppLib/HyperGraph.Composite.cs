using System.Collections.Generic;
using System.Linq;

namespace AppLib
{
	public partial class CompositeHyperGraph :
		HyperGraphBase<VertexBase<CompositeHyperEdge>, CompositeHyperEdge>
	{
		#region Private Constructor

		private CompositeHyperGraph()
		{
			InvisibleVertices = new List<InvisibleVertex>();
		}

		#endregion

		#region Protected Auto-Properties

		public List<InvisibleVertex> InvisibleVertices { get; }

		#endregion

		#region Public Static Methods

		public static CompositeHyperGraph Create(HyperGraph hyperGraph)
		{
			var resGraph = new CompositeHyperGraph();
			resGraph.VerticesList.AddRange(hyperGraph.VerticesList
			   .Select(v => new CompositeVertex {Data = v.Data}));

			foreach (var hyperEdge in hyperGraph.EdgesList)
			{
				var invisibleVertex = new InvisibleVertex();
				resGraph.InvisibleVertices.Add(invisibleVertex);

				foreach (var compositeEdge in hyperEdge.Vertices.Select(hyperVertex => new CompositeHyperEdge
				{
					Source = resGraph.VerticesList.First(v =>
						v.Data?.Equals(hyperVertex.Data) ?? false),
					Target = invisibleVertex
				}))
				{
					resGraph.EdgesList.Add(compositeEdge);
					invisibleVertex.Edges.Add(compositeEdge);
				}
			}

			return resGraph;
		}

		#endregion
	}
}