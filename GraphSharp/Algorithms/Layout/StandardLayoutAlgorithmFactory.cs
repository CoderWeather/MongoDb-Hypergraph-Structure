using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphSharp.Algorithms.Layout.Compound;
using GraphSharp.Algorithms.Layout.Compound.FDP;
using GraphSharp.Algorithms.Layout.Simple.Circular;
using GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout
{
	public class
		StandardLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		public IEnumerable<string> AlgorithmTypes =>
			new[]
			{
				"Circular", "Tree", "FR", "BoundedFR", "KK", "ISOM", "LinLog", "EfficientSugiyama", /*"Sugiyama",*/
				"CompoundFDP"
			};

		public ILayoutAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(string newAlgorithmType,
			ILayoutContext<TVertex, TEdge, TGraph> context, ILayoutParameters parameters)
		{
			if (context == null || context.Graph == null)
				return null;

			switch (context.Mode)
			{
				case LayoutMode.Simple:
					return newAlgorithmType switch
					{
						"Tree" => new SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph,
							context.Positions, context.Sizes, parameters as SimpleTreeLayoutParameters),
						"Circular" => new CircularLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph,
							context.Positions, context.Sizes, parameters as CircularLayoutParameters),
						"FR" => new FRLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
							parameters as FRLayoutParametersBase),
						"BoundedFR" => new FRLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
							parameters as BoundedFRLayoutParameters),
						"KK" => new KKLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
							parameters as KKLayoutParameters),
						"ISOM" => new ISOMLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
							parameters as ISOMLayoutParameters),
						"LinLog" => new LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
							parameters as LinLogLayoutParameters),
						"EfficientSugiyama" => new EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>(
							context.Graph, parameters as EfficientSugiyamaLayoutParameters, context.Positions,
							context.Sizes),
						"Sugiyama" => new SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Sizes,
							context.Positions, parameters as SugiyamaLayoutParameters,
							e => e is TypedEdge<TVertex> ? (e as TypedEdge<TVertex>).Type : EdgeTypes.Hierarchical),
						"CompoundFDP" => new CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph,
							context.Sizes, new Dictionary<TVertex, Thickness>(),
							new Dictionary<TVertex, CompoundVertexInnerLayoutType>(), context.Positions,
							parameters as CompoundFDPLayoutParameters),
						_ => null
					};

					break;
				case LayoutMode.Compound:
				{
					var compoundContext = context as ICompoundLayoutContext<TVertex, TEdge, TGraph>;
					return newAlgorithmType switch
					{
						"CompoundFDP" => new CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>(compoundContext.Graph,
							compoundContext.Sizes, compoundContext.VertexBorders, compoundContext.LayoutTypes,
							compoundContext.Positions, parameters as CompoundFDPLayoutParameters),
						_ => null
					};

					break;
				}
				default:
					return null;
			}
		}

		public ILayoutParameters CreateParameters(string algorithmType, ILayoutParameters oldParameters)
		{
			switch (algorithmType)
			{
				case "Tree":
					return oldParameters.CreateNewParameter<SimpleTreeLayoutParameters>();
				case "Circular":
					return oldParameters.CreateNewParameter<CircularLayoutParameters>();
				case "FR":
					return oldParameters.CreateNewParameter<FreeFRLayoutParameters>();
				case "BoundedFR":
					return oldParameters.CreateNewParameter<BoundedFRLayoutParameters>();
				case "KK":
					return oldParameters.CreateNewParameter<KKLayoutParameters>();
				case "ISOM":
					return oldParameters.CreateNewParameter<ISOMLayoutParameters>();
				case "LinLog":
					return oldParameters.CreateNewParameter<LinLogLayoutParameters>();
				case "EfficientSugiyama":
					return oldParameters.CreateNewParameter<EfficientSugiyamaLayoutParameters>();
				case "Sugiyama":
					return oldParameters.CreateNewParameter<SugiyamaLayoutParameters>();
				case "CompoundFDP":
					return oldParameters.CreateNewParameter<CompoundFDPLayoutParameters>();
				default:
					return null;
			}
		}


		public bool IsValidAlgorithm(string algorithmType)
		{
			return AlgorithmTypes.Contains(algorithmType);
		}

		public string GetAlgorithmType(ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
		{
			if (algorithm == null)
				return string.Empty;

			var index = algorithm.GetType().Name.IndexOf("LayoutAlgorithm");
			if (index == -1)
				return string.Empty;

			var algoType = algorithm.GetType().Name;
			return algoType.Substring(0, algoType.Length - index);
		}

		public bool NeedEdgeRouting(string algorithmType)
		{
			return algorithmType != "Sugiyama" && algorithmType != "EfficientSugiyama";
		}

		public bool NeedOverlapRemoval(string algorithmType)
		{
			return algorithmType != "Sugiyama"
			 && algorithmType != "EfficientSugiyama"
			 && algorithmType != "Circular"
			 && algorithmType != "Tree"
			 && algorithmType != "CompoundFDP";
		}
	}
}