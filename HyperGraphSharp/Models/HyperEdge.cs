using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;

namespace HyperGraphSharp.Models
{
	public class HyperEdge : IEquatable<HyperEdge>
	{
		public HyperEdge()
		{
			Id = Guid.NewGuid();
			Weight = 0;
			Vertices = new HashSet<Vertex>();
		}

		[SerializationConstructor]
		public HyperEdge(int weight, HashSet<Vertex> vertices)
		{
			Id = Guid.NewGuid();
			Weight = weight;
			Vertices = vertices;
		}

		[IgnoreDataMember] public Guid Id { get; }
		public int Weight { get; set; }
		public HashSet<Vertex> Vertices { get; }

		[IgnoreDataMember] public static IEqualityComparer<HyperEdge> Comparer { get; } = new EqualityComparer();

		public bool Equals(HyperEdge? other) =>
			other != null && Vertices.Equals(other.Vertices);

		public override string ToString() => Weight.ToString();

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((HyperEdge) obj);
		}

		public override int GetHashCode() => Id.GetHashCode();

#nullable disable
		private sealed class EqualityComparer : IEqualityComparer<HyperEdge>
		{
			public bool Equals(HyperEdge x, HyperEdge y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				return x.GetType() == y.GetType() && x.Equals(y);
			}

			public int GetHashCode(HyperEdge obj) => obj.Id.GetHashCode();
		}
#nullable enable
	}
}