using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph.Contracts
{
	[ContractClassFor(typeof(IMutableVertexSet<>))]
	internal abstract class IMutableVertexSetContract<TVertex>
		: IMutableVertexSet<TVertex>
	{
		#region IImplicitVertexSet<TVertex> Members

		public bool ContainsVertex(TVertex vertex)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IMutableVertexSet<TVertex> Members

		event VertexAction<TVertex> IMutableVertexSet<TVertex>.VertexAdded
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		bool IMutableVertexSet<TVertex>.AddVertex(TVertex v)
		{
			IMutableVertexSet<TVertex> ithis = this;
			Contract.Requires(v != null);
			Contract.Ensures(Contract.Result<bool>() == Contract.OldValue(!ithis.ContainsVertex(v)));
			Contract.Ensures(ithis.ContainsVertex(v));
			Contract.Ensures(ithis.VertexCount ==
				Contract.OldValue(ithis.VertexCount) + (Contract.Result<bool>() ? 1 : 0));

			return default;
		}

		int IMutableVertexSet<TVertex>.AddVertexRange(IEnumerable<TVertex> vertices)
		{
			IMutableVertexSet<TVertex> ithis = this;
			Contract.Requires(vertices != null);
			Contract.Requires(vertices.All(v => v != null));
			Contract.Ensures(vertices.All(v => ithis.ContainsVertex(v)));
			Contract.Ensures(ithis.VertexCount == Contract.OldValue(ithis.VertexCount) + Contract.Result<int>());

			return default;
		}

		event VertexAction<TVertex> IMutableVertexSet<TVertex>.VertexRemoved
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		bool IMutableVertexSet<TVertex>.RemoveVertex(TVertex v)
		{
			IMutableVertexSet<TVertex> ithis = this;
			Contract.Requires(v != null);
			Contract.Ensures(Contract.Result<bool>() == Contract.OldValue(ithis.ContainsVertex(v)));
			Contract.Ensures(!ithis.ContainsVertex(v));
			Contract.Ensures(ithis.VertexCount ==
				Contract.OldValue(ithis.VertexCount) - (Contract.Result<bool>() ? 1 : 0));

			return default;
		}

		int IMutableVertexSet<TVertex>.RemoveVertexIf(VertexPredicate<TVertex> pred)
		{
			IMutableVertexSet<TVertex> ithis = this;
			Contract.Requires(pred != null);
			Contract.Ensures(Contract.Result<int>() == Contract.OldValue(ithis.Vertices.Count(v => pred(v))));
			Contract.Ensures(ithis.Vertices.All(v => !pred(v)));
			Contract.Ensures(ithis.VertexCount == Contract.OldValue(ithis.VertexCount) - Contract.Result<int>());

			return default;
		}

		#endregion

		#region IVertexSet<TVertex> Members

		public bool IsVerticesEmpty => throw new NotImplementedException();

		public int VertexCount => throw new NotImplementedException();

		public IEnumerable<TVertex> Vertices => throw new NotImplementedException();

		#endregion
	}
}