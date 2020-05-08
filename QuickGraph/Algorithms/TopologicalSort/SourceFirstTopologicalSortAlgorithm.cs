using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.TopologicalSort
{
	[Serializable]
	public sealed class SourceFirstTopologicalSortAlgorithm<TVertex, TEdge> :
		AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private IList<TVertex> sortedVertices = new List<TVertex>();

		public SourceFirstTopologicalSortAlgorithm(
			IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph
		)
			: base(visitedGraph)
		{
			Heap = new BinaryQueue<TVertex, int>(e => InDegrees[e]);
		}

		public ICollection<TVertex> SortedVertices => sortedVertices;

		public BinaryQueue<TVertex, int> Heap { get; }

		public IDictionary<TVertex, int> InDegrees { get; } = new Dictionary<TVertex, int>();

		public event VertexAction<TVertex> AddVertex;

		private void OnAddVertex(TVertex v)
		{
			var eh = AddVertex;
			if (eh != null)
				eh(v);
		}

		public void Compute(IList<TVertex> vertices)
		{
			Contract.Requires(vertices != null);

			sortedVertices = vertices;
			Compute();
		}


		protected override void InternalCompute()
		{
			var cancelManager = Services.CancelManager;
			InitializeInDegrees();

			while (Heap.Count != 0)
			{
				if (cancelManager.IsCancelling) break;

				var v = Heap.Dequeue();
				if (InDegrees[v] != 0)
					throw new NonAcyclicGraphException();

				sortedVertices.Add(v);
				OnAddVertex(v);

				// update the count of it's adjacent vertices
				foreach (var e in VisitedGraph.OutEdges(v))
				{
					if (e.Source.Equals(e.Target))
						continue;

					InDegrees[e.Target]--;
					Contract.Assert(InDegrees[e.Target] >= 0);
					Heap.Update(e.Target);
				}
			}
		}

		private void InitializeInDegrees()
		{
			foreach (var v in VisitedGraph.Vertices)
			{
				InDegrees.Add(v, 0);
				Heap.Enqueue(v);
			}

			foreach (var e in VisitedGraph.Edges)
			{
				if (e.Source.Equals(e.Target))
					continue;
				InDegrees[e.Target]++;
			}
		}
	}
}