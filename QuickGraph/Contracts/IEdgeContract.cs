using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IEdge<>))]
	internal abstract class IEdgeContract<TVertex>
		: IEdge<TVertex>
	{
		TVertex IEdge<TVertex>.Source
		{
			get
			{
				Contract.Ensures(Contract.Result<TVertex>() != null);
				return default;
			}
		}

		TVertex IEdge<TVertex>.Target
		{
			get
			{
				Contract.Ensures(Contract.Result<TVertex>() != null);
				return default;
			}
		}

		[ContractInvariantMethod]
		private void IEdgeInvariant()
		{
			IEdge<TVertex> ithis = this;
			Contract.Invariant(ithis.Source != null);
			Contract.Invariant(ithis.Target != null);
		}
	}
}