using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IGraph<,>))]
	internal abstract class IGraphContract<TVertex, TEdge>
		: IGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		bool IGraph<TVertex, TEdge>.IsDirected => default;

		bool IGraph<TVertex, TEdge>.AllowParallelEdges => default;
	}
}