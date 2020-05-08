using QuickGraph;

namespace GraphSharp
{
	public enum EdgeTypes
	{
		General,
		Hierarchical
	}

	public interface ITypedEdge<TVertex>
	{
		EdgeTypes Type { get; }
	}

	public class TypedEdge<TVertex> : Edge<TVertex>, ITypedEdge<TVertex>
	{
		public TypedEdge(TVertex source, TVertex target, EdgeTypes type)
			: base(source, target)
		{
			Type = type;
		}

		public EdgeTypes Type { get; }

		public override string ToString()
		{
			return string.Format("{0}: {1}-->{2}", Type, Source, Target);
		}
	}
}