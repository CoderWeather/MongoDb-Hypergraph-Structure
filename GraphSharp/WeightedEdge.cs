using QuickGraph;

namespace GraphSharp
{
	public class WeightedEdge<Vertex> : Edge<Vertex>
	{
		public WeightedEdge(Vertex source, Vertex target)
			: this(source, target, 1)
		{
		}

		public WeightedEdge(Vertex source, Vertex target, double weight)
			: base(source, target)
		{
			Weight = weight;
		}

		public double Weight { get; }
	}
}