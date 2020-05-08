using System;
using System.Collections.Generic;

namespace AppLib
{
	public abstract class VertexBase<TEdge>
	{
		protected VertexBase()
		{
			Id = Guid.NewGuid();
			Data = null;
			Edges = new HashSet<TEdge>();
		}

		public Guid Id { get; }
		public string? Data { get; set; }
		public HashSet<TEdge> Edges { get; }

		public static IEqualityComparer<VertexBase<TEdge>> Comparer { get; } = new EqualityComparer();

		protected bool Equals(Vertex other)
		{
			return Id.Equals(other.Id) && Data == other.Data;
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Vertex) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, Data);
		}

		private sealed class EqualityComparer : IEqualityComparer<VertexBase<TEdge>>
		{
			public bool Equals(VertexBase<TEdge> x, VertexBase<TEdge> y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return x.Id.Equals(y.Id) && x.Data == y.Data;
			}

			public int GetHashCode(VertexBase<TEdge> obj)
			{
				return HashCode.Combine(obj.Id, obj.Data);
			}
		}
	}
}