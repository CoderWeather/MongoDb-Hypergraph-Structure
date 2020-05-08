#nullable disable
using System;
using QuickGraph;

namespace AppLib
{
	public class CompositeHyperEdge : IEdge<VertexBase<CompositeHyperEdge>>
	{
		public VertexBase<CompositeHyperEdge> Source { get; set; }
		public VertexBase<CompositeHyperEdge> Target { get; set; }

		protected bool Equals(CompositeHyperEdge other)
		{
			return Equals(Source, other.Source) && Equals(Target, other.Target);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((CompositeHyperEdge) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Source, Target);
		}
	}
}