using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
	[Serializable]
	public sealed class BinaryQueue<TVertex, TDistance> :
		IPriorityQueue<TVertex>
	{
		private readonly Func<TVertex, TDistance> distances;
		private readonly BinaryHeap<TDistance, TVertex> heap;

		public BinaryQueue(
			Func<TVertex, TDistance> distances
		)
			: this(distances, Comparer<TDistance>.Default.Compare)
		{
		}

		public BinaryQueue(
			Func<TVertex, TDistance> distances,
			Func<TDistance, TDistance, int> distanceComparison
		)
		{
			Contract.Requires(distances != null);
			Contract.Requires(distanceComparison != null);

			this.distances = distances;
			heap = new BinaryHeap<TDistance, TVertex>(distanceComparison);
		}

		public void Update(TVertex v)
		{
			heap.Update(distances(v), v);
		}

		public int Count => heap.Count;

		public bool Contains(TVertex value)
		{
			return heap.IndexOf(value) > -1;
		}

		public void Enqueue(TVertex value)
		{
			heap.Add(distances(value), value);
		}

		public TVertex Dequeue()
		{
			return heap.RemoveMinimum().Value;
		}

		public TVertex Peek()
		{
			return heap.Minimum().Value;
		}

		public TVertex[] ToArray()
		{
			return heap.ToValueArray();
		}
	}
}