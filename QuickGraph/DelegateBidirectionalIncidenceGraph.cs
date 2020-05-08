using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    /// <summary>
    ///     A delegate based bidirectional implicit graph
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    [Serializable]
	public class DelegateBidirectionalIncidenceGraph<TVertex, TEdge>
		: DelegateIncidenceGraph<TVertex, TEdge>
			, IBidirectionalIncidenceGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>, IEquatable<TEdge>
	{
		public DelegateBidirectionalIncidenceGraph(
			TryFunc<TVertex, IEnumerable<TEdge>> tryGetOutEdges,
			TryFunc<TVertex, IEnumerable<TEdge>> tryGetInEdges)
			: base(tryGetOutEdges)
		{
			Contract.Requires(tryGetInEdges != null);

			TryGetInEdgesFunc = tryGetInEdges;
		}

		public TryFunc<TVertex, IEnumerable<TEdge>> TryGetInEdgesFunc { get; }

		#region IBidirectionalImplicitGraph<TVertex,TEdge> Members

		public bool IsInEdgesEmpty(TVertex v)
		{
			foreach (var edge in InEdges(v))
				return false;
			return true;
		}

		public int InDegree(TVertex v)
		{
			return InEdges(v).Count();
		}

		public IEnumerable<TEdge> InEdges(TVertex v)
		{
			IEnumerable<TEdge> result;
			if (!TryGetInEdgesFunc(v, out result))
				return Enumerable.Empty<TEdge>();
			return result;
		}

		public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
		{
			return TryGetInEdgesFunc(v, out edges);
		}

		public TEdge InEdge(TVertex v, int index)
		{
			return InEdges(v).ElementAt(index);
		}

		public int Degree(TVertex v)
		{
			return InDegree(v) + OutDegree(v);
		}

		#endregion
	}
}