using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IUndirectedEdge<>))]
	internal abstract class IUndirectedEdgeContract<TVertex>
		: IUndirectedEdge<TVertex>
	{
		[ContractInvariantMethod]
		private void IUndirectedEdgeInvariant()
		{
			IUndirectedEdge<TVertex> ithis = this;
			Contract.Invariant(Comparer<TVertex>.Default.Compare(ithis.Source, ithis.Target) <= 0);
		}

		#region IEdge<TVertex> Members

		public TVertex Source => throw new NotImplementedException();

		public TVertex Target => throw new NotImplementedException();

		#endregion
	}
}