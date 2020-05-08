using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IMutableGraph<,>))]
	internal abstract class IMutableGraphContract<TVertex, TEdge>
		: IMutableGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		#region IMutableGraph<TVertex,TEdge> Members

		void IMutableGraph<TVertex, TEdge>.Clear()
		{
			IMutableGraph<TVertex, TEdge> ithis = this;
		}

		#endregion


		event EventHandler IMutableGraph<TVertex, TEdge>.Cleared
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		#region IGraph<TVertex,TEdge> Members

		bool IGraph<TVertex, TEdge>.IsDirected => throw new NotImplementedException();

		bool IGraph<TVertex, TEdge>.AllowParallelEdges => throw new NotImplementedException();

		#endregion
	}
}