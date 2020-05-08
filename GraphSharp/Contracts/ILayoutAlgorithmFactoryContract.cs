using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using GraphSharp.Algorithms.Layout;
using QuickGraph;

namespace GraphSharp.Contracts
{
	[ContractClassFor(typeof(ILayoutAlgorithmFactory<,,>))]
	public class
		ILayoutAlgorithmFactoryContract<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		#region ILayoutAlgorithmFactory<TVertex,TEdge,TGraph> Members

		IEnumerable<string> ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.AlgorithmTypes
		{
			get
			{
				Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

				return default;
			}
		}

		ILayoutAlgorithm<TVertex, TEdge, TGraph> ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.CreateAlgorithm(
			string newAlgorithmType, ILayoutContext<TVertex, TEdge, TGraph> context, ILayoutParameters parameters)
		{
			Contract.Requires(newAlgorithmType != null);
			var laf = (ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>) this;
			Contract.Requires(laf.AlgorithmTypes.Contains(newAlgorithmType));
			Contract.Requires(context.Sizes != null);

			return default;
		}

		ILayoutParameters ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.CreateParameters(string algorithmType,
			ILayoutParameters oldParameters)
		{
			Contract.Requires(algorithmType != null);
			var laf = (ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>) this;
			Contract.Requires(laf.AlgorithmTypes.Contains(algorithmType));

			return default;
		}

		bool ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.IsValidAlgorithm(string algorithmType)
		{
			Contract.Requires(algorithmType != null);
			var laf = (ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>) this;
			Contract.Ensures(Contract.Result<bool>() == laf.AlgorithmTypes.Contains(algorithmType));

			return false;
		}

		string ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.GetAlgorithmType(
			ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
		{
			Contract.Requires(algorithm != null);
			var laf = (ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>) this;
			Contract.Ensures(
				Contract.Result<string>() == null || laf.AlgorithmTypes.Contains(Contract.Result<string>()));

			return default;
		}

		bool ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.NeedEdgeRouting(string algorithmType)
		{
			return default;
		}

		bool ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>.NeedOverlapRemoval(string algorithmType)
		{
			return default;
		}

		#endregion
	}
}