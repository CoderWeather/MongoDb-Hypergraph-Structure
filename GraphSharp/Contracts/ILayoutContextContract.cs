using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;
using GraphSharp.Algorithms.Layout;
using QuickGraph;

namespace GraphSharp.Contracts
{
	[ContractClassFor(typeof(ILayoutContext<,,>))]
	public class ILayoutContextContract<TVertex, TEdge, TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		[ContractInvariantMethod]
		protected void Invariants()
		{
			var lc = (ILayoutContext<TVertex, TEdge, TGraph>) this;
			Contract.Invariant(lc.Positions != null);
			Contract.Invariant(lc.Graph != null);
			Contract.Invariant(lc.Sizes != null);
		}

		#region ILayoutContext<TVertex,TEdge,TGraph> Members

		IDictionary<TVertex, Point> ILayoutContext<TVertex, TEdge, TGraph>.Positions => default;

		IDictionary<TVertex, Size> ILayoutContext<TVertex, TEdge, TGraph>.Sizes => default;

		TGraph ILayoutContext<TVertex, TEdge, TGraph>.Graph => default;

		LayoutMode ILayoutContext<TVertex, TEdge, TGraph>.Mode => default;

		#endregion
	}
}