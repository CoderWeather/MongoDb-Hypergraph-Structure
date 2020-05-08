using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IEdgeSet<,>))]
	internal abstract class IEdgeSetContract<TVertex, TEdge>
		: IEdgeSet<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		bool IEdgeSet<TVertex, TEdge>.IsEdgesEmpty
		{
			get
			{
				IEdgeSet<TVertex, TEdge> ithis = this;
				Contract.Ensures(Contract.Result<bool>() == (ithis.EdgeCount == 0));

				return default;
			}
		}

		int IEdgeSet<TVertex, TEdge>.EdgeCount
		{
			get
			{
				IEdgeSet<TVertex, TEdge> ithis = this;
				Contract.Ensures(Contract.Result<int>() == ithis.Edges.Count());

				return default;
			}
		}

		IEnumerable<TEdge> IEdgeSet<TVertex, TEdge>.Edges
		{
			get
			{
				Contract.Ensures(Contract.Result<IEnumerable<TEdge>>() != null);
				Contract.Ensures(Contract.Result<IEnumerable<TEdge>>().All(e => e != null));

				return default;
			}
		}

		[Pure]
		bool IEdgeSet<TVertex, TEdge>.ContainsEdge(TEdge edge)
		{
			IEdgeSet<TVertex, TEdge> ithis = this;
			Contract.Requires(edge != null);
			Contract.Ensures(Contract.Result<bool>() == Contract.Exists(ithis.Edges, e => e.Equals(edge)));

			return default;
		}
	}
}