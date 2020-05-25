using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;

namespace HyperGraphSharp.Models
{
	public class Vertex : IEquatable<Vertex>
	{
		#region Public Properties

		[IgnoreDataMember] public Guid Id { get; }
		public string? Data { get; }
		[IgnoreDataMember] public HashSet<HyperEdge> HyperEdges { get; }

		#endregion

		#region Public Constructor

		[SerializationConstructor]
		public Vertex(string? data)
		{
			Id = Guid.NewGuid();
			Data = data;
			HyperEdges = new HashSet<HyperEdge>();
		}

		#endregion

		public override string ToString() => Data ?? "NULL";

		#region Equality

		public static IEqualityComparer<Vertex> Comparer { get; } = new EqualityComparer();

		public bool Equals(Vertex? other) =>
			other != null && Data == other.Data && HyperEdges.Equals(other.HyperEdges);


		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Vertex) obj);
		}

		public override int GetHashCode() => Id.GetHashCode();

		private sealed class EqualityComparer : IEqualityComparer<Vertex?>
		{
			public bool Equals(Vertex? x, Vertex? y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				return x.GetType() == y.GetType() && x.Equals(y);
			}

			public int GetHashCode(Vertex? obj) =>
				HashCode.Combine(obj?.Id, obj?.Data);
		}

		#endregion
	}
}