using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
	internal struct DisposableAction
		: IDisposable
	{
		public delegate void Action();

		private Action action;

		public DisposableAction(Action action)
		{
			Contract.Requires(action != null);
			this.action = action;
		}

		public void Dispose()
		{
			var a = action;
			action = null;
			if (a != null)
				a();
		}
	}
}