using System;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
	[Serializable]
	public abstract class VertexEventArgs<TVertex> : EventArgs
	{
		protected VertexEventArgs(TVertex vertex)
		{
			Contract.Requires(vertex != null);
			Vertex = vertex;
		}

		public TVertex Vertex { get; }
	}

	public delegate void VertexAction<TVertex>(TVertex vertex);
}