using System;
using System.Collections.Generic;

namespace HyperGraphSharp.Models
{
	public class HyperEdge : IEquatable<HyperEdge>
	{
		public HyperEdge()
		{
			Id = Guid.NewGuid();
			Weight = 1;
			Vertices = new HashSet<Vertex>(Vertex.Comparer);
		}

		public Guid Id { get; }
		public int Weight { get; set; }
		public ISet<Vertex> Vertices { get; }

		public static IEqualityComparer<HyperEdge> Comparer { get; } = new EqualityComparer();

		public bool Equals(HyperEdge? other) =>
			other != null && Id.Equals(other.Id);

		public override string ToString() => $"[{Weight}]";

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((HyperEdge) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

#nullable disable
		private sealed class EqualityComparer : IEqualityComparer<HyperEdge>
		{
			public bool Equals(HyperEdge x, HyperEdge y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				return x.GetType() == y.GetType() && x.Id.Equals(y.Id);
			}

			public int GetHashCode(HyperEdge obj)
			{
				return obj.Id.GetHashCode();
			}
		}
#nullable enable
	}
}