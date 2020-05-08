using System;
using System.Collections.Generic;

namespace AppLib
{
	public class HyperEdge
	{
		public Guid Id { get; } = Guid.NewGuid();
		public int Weight { get; set; } = 1;
		public HashSet<Vertex> Vertices { get; } = new HashSet<Vertex>(Vertex.Comparer);

		protected bool Equals(HyperEdge other)
		{
			return Id.Equals(other.Id);
		}

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
	}
}