using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms
{
	public abstract class AlgorithmBase<TGraph> :
		IAlgorithm<TGraph>,
		IAlgorithmComponent
	{
		private readonly AlgorithmServices services;

		private Dictionary<Type, object> _services;
		private volatile ComputationState state = ComputationState.NotRunning;
		private volatile object syncRoot = new object();

        /// <summary>
        ///     Creates a new algorithm with an (optional) host.
        /// </summary>
        /// <param name="host">if null, host is set to the this reference</param>
        /// <param name="visitedGraph"></param>
        protected AlgorithmBase(IAlgorithmComponent host, TGraph visitedGraph)
		{
			Contract.Requires(visitedGraph != null);
			if (host == null)
				host = this;
			VisitedGraph = visitedGraph;
			services = new AlgorithmServices(host);
		}

		protected AlgorithmBase(TGraph visitedGraph)
		{
			Contract.Requires(visitedGraph != null);
			VisitedGraph = visitedGraph;
			services = new AlgorithmServices(this);
		}

		public TGraph VisitedGraph { get; }

		public object SyncRoot => syncRoot;

		public ComputationState State
		{
			get
			{
				lock (syncRoot)
				{
					return state;
				}
			}
		}

		public void Compute()
		{
			BeginComputation();
			Initialize();
			try
			{
				InternalCompute();
			}
			finally
			{
				Clean();
			}

			EndComputation();
		}

		public void Abort()
		{
			var raise = false;
			lock (syncRoot)
			{
				if (state == ComputationState.Running)
				{
					state = ComputationState.PendingAbortion;
					Services.CancelManager.Cancel();
					raise = true;
				}
			}

			if (raise)
				OnStateChanged(EventArgs.Empty);
		}

		public event EventHandler StateChanged;

		public event EventHandler Started;

		public event EventHandler Finished;

		public event EventHandler Aborted;

		public IAlgorithmServices Services => services;

		public T GetService<T>()
			where T : IService
		{
			T service;
			if (!TryGetService(out service))
				throw new InvalidOperationException("service not found");
			return service;
		}

		public bool TryGetService<T>(out T service)
			where T : IService
		{
			object serviceObject;
			if (TryGetService(typeof(T), out serviceObject))
			{
				service = (T) serviceObject;
				return true;
			}

			service = default;
			return false;
		}

		protected virtual void Initialize()
		{
		}

		protected virtual void Clean()
		{
		}

		protected abstract void InternalCompute();

		protected virtual void OnStateChanged(EventArgs e)
		{
			EventHandler eh = StateChanged;
			if (eh != null)
				eh(this, e);
		}

		protected virtual void OnStarted(EventArgs e)
		{
			EventHandler eh = Started;
			if (eh != null)
				eh(this, e);
		}

		protected virtual void OnFinished(EventArgs e)
		{
			EventHandler eh = Finished;
			if (eh != null)
				eh(this, e);
		}

		protected virtual void OnAborted(EventArgs e)
		{
			EventHandler eh = Aborted;
			if (eh != null)
				eh(this, e);
		}

		protected void BeginComputation()
		{
			Contract.Requires(State == ComputationState.NotRunning);
			lock (syncRoot)
			{
				state = ComputationState.Running;
				Services.CancelManager.ResetCancel();
				OnStarted(EventArgs.Empty);
				OnStateChanged(EventArgs.Empty);
			}
		}

		protected void EndComputation()
		{
			Contract.Requires(
				State == ComputationState.Running ||
				State == ComputationState.Aborted);
			lock (syncRoot)
			{
				switch (state)
				{
					case ComputationState.Running:
						state = ComputationState.Finished;
						OnFinished(EventArgs.Empty);
						break;
					case ComputationState.PendingAbortion:
						state = ComputationState.Aborted;
						OnAborted(EventArgs.Empty);
						break;
					default:
						throw new InvalidOperationException();
				}

				Services.CancelManager.ResetCancel();
				OnStateChanged(EventArgs.Empty);
			}
		}

		protected virtual bool TryGetService(Type serviceType, out object service)
		{
			Contract.Requires(serviceType != null);
			lock (SyncRoot)
			{
				if (_services == null)
					_services = new Dictionary<Type, object>();
				if (!_services.TryGetValue(serviceType, out service))
				{
					if (serviceType == typeof(ICancelManager))
						_services[serviceType] = service = new CancelManager();
					else
						_services[serviceType] = service = null;
				}

				return service != null;
			}
		}
	}
}