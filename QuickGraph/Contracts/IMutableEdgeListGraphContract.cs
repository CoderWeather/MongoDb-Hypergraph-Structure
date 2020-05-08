using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IMutableEdgeListGraph<,>))]
	internal abstract class IMutableEdgeListGraphContract<TVertex, TEdge>
		: IMutableEdgeListGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		#region IMutableGraph<TVertex,TEdge> Members

		void IMutableGraph<TVertex, TEdge>.Clear()
		{
			throw new NotImplementedException();
		}

		#endregion


		event EventHandler IMutableGraph<TVertex, TEdge>.Cleared
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		#region IMutableEdgeListGraph<TVertex,TEdge> Members

		bool IMutableEdgeListGraph<TVertex, TEdge>.AddEdge(TEdge e)
		{
			IMutableEdgeListGraph<TVertex, TEdge> ithis = this;
			Contract.Requires(e != null);
			Contract.Requires(ithis.ContainsVertex(e.Source));
			Contract.Requires(ithis.ContainsVertex(e.Target));
			Contract.Ensures(ithis.ContainsEdge(e));
			Contract.Ensures(ithis.AllowParallelEdges ||
				Contract.Result<bool>() == Contract.OldValue(!ithis.ContainsEdge(e)));
			Contract.Ensures(ithis.EdgeCount == Contract.OldValue(ithis.EdgeCount) + (Contract.Result<bool>() ? 1 : 0));

			return default;
		}

		event EdgeAction<TVertex, TEdge> IMutableEdgeListGraph<TVertex, TEdge>.EdgeAdded
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		int IMutableEdgeListGraph<TVertex, TEdge>.AddEdgeRange(IEnumerable<TEdge> edges)
		{
			IMutableEdgeListGraph<TVertex, TEdge> ithis = this;
			Contract.Requires(edges != null);
			Contract.Requires(typeof(TEdge).GetTypeInfo().IsValueType || edges.All(edge => edge != null));
			Contract.Requires(edges.All(edge =>
				ithis.ContainsVertex(edge.Source) &&
				ithis.ContainsVertex(edge.Target)
			));
			Contract.Ensures(edges.All(edge => ithis.ContainsEdge(edge)), "all edge from edges belong to the graph");
			Contract.Ensures(
				Contract.Result<int>() == Contract.OldValue(edges.Count(edge => !ithis.ContainsEdge(edge))));
			Contract.Ensures(ithis.EdgeCount == Contract.OldValue(ithis.EdgeCount) + Contract.Result<int>());

			return default;
		}

		bool IMutableEdgeListGraph<TVertex, TEdge>.RemoveEdge(TEdge e)
		{
			IMutableEdgeListGraph<TVertex, TEdge> ithis = this;
			Contract.Requires(e != null);
			Contract.Ensures(Contract.Result<bool>() == Contract.OldValue(ithis.ContainsEdge(e)));
			Contract.Ensures(!ithis.ContainsEdge(e));
			Contract.Ensures(ithis.EdgeCount == Contract.OldValue(ithis.EdgeCount) - (Contract.Result<bool>() ? 1 : 0));

			return default;
		}

		event EdgeAction<TVertex, TEdge> IMutableEdgeListGraph<TVertex, TEdge>.EdgeRemoved
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		int IMutableEdgeListGraph<TVertex, TEdge>.RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
		{
			IMutableEdgeListGraph<TVertex, TEdge> ithis = this;
			Contract.Requires(predicate != null);
			Contract.Ensures(Contract.Result<int>() == Contract.OldValue(ithis.Edges.Count(e => predicate(e))));
			Contract.Ensures(ithis.Edges.All(e => !predicate(e)));
			Contract.Ensures(ithis.EdgeCount == Contract.OldValue(ithis.EdgeCount) - Contract.Result<int>());

			return default;
		}

		#endregion

		#region IGraph<TVertex,TEdge> Members

		bool IGraph<TVertex, TEdge>.IsDirected => throw new NotImplementedException();

		bool IGraph<TVertex, TEdge>.AllowParallelEdges => throw new NotImplementedException();

		#endregion

		#region IEdgeSet<TVertex,TEdge> Members

		bool IEdgeSet<TVertex, TEdge>.IsEdgesEmpty => throw new NotImplementedException();

		int IEdgeSet<TVertex, TEdge>.EdgeCount => throw new NotImplementedException();

		IEnumerable<TEdge> IEdgeSet<TVertex, TEdge>.Edges => throw new NotImplementedException();

		bool IEdgeSet<TVertex, TEdge>.ContainsEdge(TEdge edge)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IVertexSet<TVertex> Members

		bool IVertexSet<TVertex>.IsVerticesEmpty => throw new NotImplementedException();

		int IVertexSet<TVertex>.VertexCount => throw new NotImplementedException();

		IEnumerable<TVertex> IVertexSet<TVertex>.Vertices => throw new NotImplementedException();

		bool IImplicitVertexSet<TVertex>.ContainsVertex(TVertex vertex)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}