using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Contracts
{
	[ContractClassFor(typeof(IAlgorithm<>))]
	internal abstract class IAlgorithmContract<TGraph>
		: IAlgorithm<TGraph>
	{
		#region IAlgorithm<TGraph> Members

		TGraph IAlgorithm<TGraph>.VisitedGraph
		{
			get
			{
				Contract.Ensures(Contract.Result<TGraph>() != null);

				return default;
			}
		}

		#endregion

		#region IComputation Members

		object IComputation.SyncRoot => throw new NotImplementedException();

		ComputationState IComputation.State => throw new NotImplementedException();

		void IComputation.Compute()
		{
			throw new NotImplementedException();
		}

		void IComputation.Abort()
		{
			throw new NotImplementedException();
		}

		event EventHandler IComputation.StateChanged
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		event EventHandler IComputation.Started
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		event EventHandler IComputation.Finished
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		event EventHandler IComputation.Aborted
		{
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		#endregion
	}
}