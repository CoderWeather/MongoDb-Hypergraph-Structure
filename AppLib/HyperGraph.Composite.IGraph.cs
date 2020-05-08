using QuickGraph;

namespace AppLib
{
	public partial class CompositeHyperGraph :
			IGraph<VertexBase<CompositeHyperEdge>, CompositeHyperEdge>
		// IEdgeListGraph<VertexBase<CompositeHyperEdge>, CompositeHyperEdge>,
		// IImplicitUndirectedGraph<VertexBase<CompositeHyperEdge>, CompositeHyperEdge>
	{
		public bool IsDirected => false;
		public bool AllowParallelEdges => true;

		// public bool ContainsEdge(CompositeHyperEdge edge) => EdgesList.Contains(edge);
		// public bool IsEdgesEmpty => EdgesList.Count == 0;
		// public int EdgeCount => EdgesList.Count;
		// public IEnumerable<CompositeHyperEdge> Edges => EdgesList;
		// public bool ContainsVertex(VertexBase<CompositeHyperEdge> vertex) => VerticesList.Contains(vertex);
		//
		// public bool IsVerticesEmpty => VerticesList.Count == 0;
		// public int VertexCount => VerticesList.Count;
		// public IEnumerable<VertexBase<CompositeHyperEdge>> Vertices => VerticesList;
		// public IEnumerable<CompositeHyperEdge> AdjacentEdges(VertexBase<CompositeHyperEdge> v)
		// {
		// 	return Edges.Where(edge => edge.)
		// }
		//
		// public int AdjacentDegree(VertexBase<CompositeHyperEdge> v)
		// {
		// 	throw new System.NotImplementedException();
		// }
		//
		// public bool IsAdjacentEdgesEmpty(VertexBase<CompositeHyperEdge> v)
		// {
		// 	throw new System.NotImplementedException();
		// }
		//
		// public CompositeHyperEdge AdjacentEdge(VertexBase<CompositeHyperEdge> v, int index)
		// {
		// 	throw new System.NotImplementedException();
		// }
		//
		// public bool TryGetEdge(VertexBase<CompositeHyperEdge> source, VertexBase<CompositeHyperEdge> target, out CompositeHyperEdge edge)
		// {
		// 	throw new System.NotImplementedException();
		// }
		//
		// public bool ContainsEdge(VertexBase<CompositeHyperEdge> source, VertexBase<CompositeHyperEdge> target)
		// {
		// 	throw new System.NotImplementedException();
		// }
		//
		// public EdgeEqualityComparer<VertexBase<CompositeHyperEdge>, CompositeHyperEdge> EdgeEqualityComparer { get; }
	}
}