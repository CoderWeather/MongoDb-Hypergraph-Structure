using System;
using System.Collections.Generic;

namespace HyperGraphSharp.Models
{
	public class Vertex : IEquatable<Vertex>
	{
		#region Public Constructor

		public Vertex(string? data)
		{
			Id = Guid.NewGuid();
			Data = data;
			HyperEdges = new HashSet<HyperEdge>(HyperEdge.Comparer);
		}

		#endregion

		public override string ToString()
		{
			return Data ?? "NULL";
		}

		#region Public Properties

		public Guid Id { get; }
		public string? Data { get; }
		public HashSet<HyperEdge> HyperEdges { get; }

		#endregion

		#region Equality

		public static IEqualityComparer<Vertex> Comparer { get; } = new EqualityComparer();

		public bool Equals(Vertex? other)
		{
			return Id.Equals(other?.Id) && Data == other.Data;
		}


		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Vertex) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		private sealed class EqualityComparer : IEqualityComparer<Vertex?>
		{
			public bool Equals(Vertex? x, Vertex? y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				return x.GetType() == y.GetType() && x.Id.Equals(y.Id) && x.Data == y.Data;
			}

			public int GetHashCode(Vertex? obj)
			{
				return HashCode.Combine(obj?.Id, obj?.Data);
			}
		}

		#endregion
	}
}