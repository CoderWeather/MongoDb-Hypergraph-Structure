using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QuickGraph.Collections
{
	[Serializable]
	public sealed class VertexEdgeDictionary<TVertex, TEdge>
		: Dictionary<TVertex, IEdgeList<TVertex, TEdge>>
			, IVertexEdgeDictionary<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		public VertexEdgeDictionary()
		{
		}

		public VertexEdgeDictionary(int capacity)
			: base(capacity)
		{
		}

		public VertexEdgeDictionary(IEqualityComparer<TVertex> vertexComparer)
			: base(vertexComparer)
		{
		}

		public VertexEdgeDictionary(int capacity, IEqualityComparer<TVertex> vertexComparer)
			: base(capacity, vertexComparer)
		{
		}

		public VertexEdgeDictionary(
			SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		IVertexEdgeDictionary<TVertex, TEdge> IVertexEdgeDictionary<TVertex, TEdge>.Clone()
		{
			return Clone();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public VertexEdgeDictionary<TVertex, TEdge> Clone()
		{
			var clone = new VertexEdgeDictionary<TVertex, TEdge>(Count);
			foreach (var kv in this)
				clone.Add(kv.Key, kv.Value.Clone());
			return clone;
		}
	}
}