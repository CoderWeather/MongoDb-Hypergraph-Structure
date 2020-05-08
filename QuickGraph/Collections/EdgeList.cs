using System;
using System.Collections.Generic;

namespace QuickGraph.Collections
{
	[Serializable]
	public sealed class EdgeList<TVertex, TEdge>
		: List<TEdge>
			, IEdgeList<TVertex, TEdge>
			, ICloneable
		where TEdge : IEdge<TVertex>
	{
		public EdgeList()
		{
		}

		public EdgeList(int capacity)
			: base(capacity)
		{
		}

		public EdgeList(EdgeList<TVertex, TEdge> list)
			: base(list)
		{
		}

		IEdgeList<TVertex, TEdge> IEdgeList<TVertex, TEdge>.Clone()
		{
			return Clone();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public EdgeList<TVertex, TEdge> Clone()
		{
			return new EdgeList<TVertex, TEdge>(this);
		}
	}
}