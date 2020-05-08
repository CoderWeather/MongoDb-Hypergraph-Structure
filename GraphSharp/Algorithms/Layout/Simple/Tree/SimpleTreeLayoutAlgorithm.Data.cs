using System.Collections.Generic;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Simple.Tree
{
	public partial class SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph> : DefaultParameterizedLayoutAlgorithmBase<
		TVertex, TEdge, TGraph, SimpleTreeLayoutParameters>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		private class Layer
		{
			public readonly IList<TVertex> Vertices = new List<TVertex>();
			public double LastTranslate;
			public double NextPosition;
			public double Size;

			public Layer()
			{
				LastTranslate = 0;
			}

			/* Width and Height Optimization */
		}

		private class VertexData
		{
			public TVertex parent;
			public double position;
			public double translate;

			/* Width and Height Optimization */
		}
	}
}