using System;
using System.Threading;
using QuickGraph.Algorithms;

namespace GraphSharp.Algorithms
{
	public abstract class AlgorithmBase : IAlgorithm
	{
		private int cancelling;
		private volatile ComputationState state = ComputationState.NotRunning;
		private volatile object syncRoot = new object();

		protected bool IsAborting => cancelling > 0;

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
			InternalCompute();
			EndComputation();
		}

		public virtual void Abort()
		{
			var raise = false;
			lock (syncRoot)
			{
				if (state == ComputationState.Running)
				{
					state = ComputationState.PendingAbortion;
					Interlocked.Increment(ref cancelling);
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

		protected abstract void InternalCompute();

		protected virtual void OnStateChanged(EventArgs e)
		{
			var eh = StateChanged;
			if (eh != null)
				eh(this, e);
		}

		protected virtual void OnStarted(EventArgs e)
		{
			var eh = Started;
			if (eh != null)
				eh(this, e);
		}

		protected virtual void OnFinished(EventArgs e)
		{
			var eh = Finished;
			if (eh != null)
				eh(this, e);
		}

		protected virtual void OnAborted(EventArgs e)
		{
			var eh = Aborted;
			if (eh != null)
				eh(this, e);
		}

		protected void BeginComputation()
		{
			lock (syncRoot)
			{
				if (state != ComputationState.NotRunning)
					throw new InvalidOperationException();

				state = ComputationState.Running;
				cancelling = 0;
				OnStarted(EventArgs.Empty);
				OnStateChanged(EventArgs.Empty);
			}
		}

		protected void EndComputation()
		{
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

				cancelling = 0;
				OnStateChanged(EventArgs.Empty);
			}
		}
	}
}