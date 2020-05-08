using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms
{
	[Serializable]
	public abstract class RootedAlgorithmBase<TVertex, TGraph>
		: AlgorithmBase<TGraph>
	{
		private bool hasRootVertex;
		private TVertex rootVertex;

		protected RootedAlgorithmBase(
			IAlgorithmComponent host,
			TGraph visitedGraph)
			: base(host, visitedGraph)
		{
		}

		public bool TryGetRootVertex(out TVertex rootVertex)
		{
			if (hasRootVertex)
			{
				rootVertex = this.rootVertex;
				return true;
			}

			rootVertex = default;
			return false;
		}

		public void SetRootVertex(TVertex rootVertex)
		{
			Contract.Requires(rootVertex != null);

			var changed = Comparer<TVertex>.Default.Compare(this.rootVertex, rootVertex) != 0;
			this.rootVertex = rootVertex;
			if (changed)
				OnRootVertexChanged(EventArgs.Empty);
			hasRootVertex = true;
		}

		public void ClearRootVertex()
		{
			rootVertex = default;
			hasRootVertex = false;
		}

		public event EventHandler RootVertexChanged;

		protected virtual void OnRootVertexChanged(EventArgs e)
		{
			Contract.Requires(e != null);

			var eh = RootVertexChanged;
			if (eh != null)
				eh(this, e);
		}

		public void Compute(TVertex rootVertex)
		{
			Contract.Requires(rootVertex != null);

			SetRootVertex(rootVertex);
			Compute();
		}
	}
}