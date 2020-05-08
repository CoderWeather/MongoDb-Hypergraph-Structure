using System;
using System.Diagnostics;

namespace QuickGraph
{
    /// <summary>
    ///     An equatable edge implementation
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    [Serializable]
	[DebuggerDisplay("{Source}->{Target}")]
	public class EquatableEdge<TVertex>
		: Edge<TVertex>
			, IEquatable<EquatableEdge<TVertex>>
	{
		public EquatableEdge(TVertex source, TVertex target)
			: base(source, target)
		{
		}

		public bool Equals(EquatableEdge<TVertex> other)
		{
			return
				other != null &&
				Source.Equals(other.Source) &&
				Target.Equals(other.Target);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EquatableEdge<TVertex>);
		}

		public override int GetHashCode()
		{
			return
				HashCodeHelper.Combine(Source.GetHashCode(), Target.GetHashCode());
		}
	}
}