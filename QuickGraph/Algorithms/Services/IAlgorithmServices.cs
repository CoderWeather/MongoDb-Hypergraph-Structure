using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Services
{
    /// <summary>
    ///     Common services available to algorithm instances
    /// </summary>
    public interface IAlgorithmServices
	{
		ICancelManager CancelManager { get; }
	}

	internal class AlgorithmServices :
		IAlgorithmServices
	{
		private readonly IAlgorithmComponent host;

		private ICancelManager _cancelManager;

		public AlgorithmServices(IAlgorithmComponent host)
		{
			Contract.Requires(host != null);

			this.host = host;
		}

		public ICancelManager CancelManager
		{
			get
			{
				if (_cancelManager == null)
					_cancelManager = host.GetService<ICancelManager>();
				return _cancelManager;
			}
		}
	}
}