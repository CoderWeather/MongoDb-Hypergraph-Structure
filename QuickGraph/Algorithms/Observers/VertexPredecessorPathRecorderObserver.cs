using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <reference-ref
    ///     idref="boost" />
    [Serializable]
	public sealed class VertexPredecessorPathRecorderObserver<TVertex, TEdge> :
		IObserver<IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private readonly List<TVertex> endPathVertices = new List<TVertex>();

		public VertexPredecessorPathRecorderObserver()
			: this(new Dictionary<TVertex, TEdge>())
		{
		}

		public VertexPredecessorPathRecorderObserver(
			IDictionary<TVertex, TEdge> vertexPredecessors)
		{
			Contract.Requires(vertexPredecessors != null);
			VertexPredecessors = vertexPredecessors;
		}

		public IDictionary<TVertex, TEdge> VertexPredecessors { get; }

		public ICollection<TVertex> EndPathVertices => endPathVertices;

		public IDisposable Attach(IVertexPredecessorRecorderAlgorithm<TVertex, TEdge> algorithm)
		{
			algorithm.TreeEdge += TreeEdge;
			algorithm.FinishVertex += FinishVertex;
			return new DisposableAction(
				() =>
				{
					algorithm.TreeEdge -= TreeEdge;
					algorithm.FinishVertex -= FinishVertex;
				});
		}

		private void TreeEdge(TEdge e)
		{
			VertexPredecessors[e.Target] = e;
		}

		private void FinishVertex(TVertex v)
		{
			foreach (var edge in VertexPredecessors.Values)
				if (edge.Source.Equals(v))
					return;
			endPathVertices.Add(v);
		}

		public IEnumerable<IEnumerable<TEdge>> AllPaths()
		{
			var es = new List<IEnumerable<TEdge>>();
			foreach (var v in EndPathVertices)
			{
				IEnumerable<TEdge> path;
				if (VertexPredecessors.TryGetPath(v, out path))
					es.Add(path);
			}

			return es;
		}
	}
}