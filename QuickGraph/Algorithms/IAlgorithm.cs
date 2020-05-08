using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Contracts;

namespace QuickGraph.Algorithms
{
	[ContractClass(typeof(IAlgorithmContract<>))]
	public interface IAlgorithm<TGraph> :
		IComputation
	{
		TGraph VisitedGraph { get; }
	}
}