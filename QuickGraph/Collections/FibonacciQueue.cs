using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickGraph.Algorithms;

namespace QuickGraph.Collections
{
	[DebuggerDisplay("Count = {Count}")]
	public sealed class FibonacciQueue<TVertex, TDistance> :
		IPriorityQueue<TVertex>
	{
		private readonly Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>> cells;
		private readonly Func<TVertex, TDistance> distances;

		private readonly FibonacciHeap<TDistance, TVertex> heap;

		public FibonacciQueue(Func<TVertex, TDistance> distances)
			: this(0, null, distances, Comparer<TDistance>.Default.Compare)
		{
		}

		public FibonacciQueue(
			int valueCount,
			IEnumerable<TVertex> values,
			Func<TVertex, TDistance> distances
		)
			: this(valueCount, values, distances, Comparer<TDistance>.Default.Compare)
		{
		}

		public FibonacciQueue(
			int valueCount,
			IEnumerable<TVertex> values,
			Func<TVertex, TDistance> distances,
			Func<TDistance, TDistance, int> distanceComparison
		)
		{
			Contract.Requires(valueCount >= 0);
			Contract.Requires(valueCount == 0 || values != null && valueCount == values.Count());
			Contract.Requires(distances != null);
			Contract.Requires(distanceComparison != null);

			this.distances = distances;
			cells = new Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>>(valueCount);
			if (valueCount > 0)
				foreach (var x in values)
					cells.Add(
						x,
						new FibonacciHeapCell<TDistance, TVertex>
						{
							Priority = this.distances(x),
							Value = x,
							Removed = true
						}
					);
			heap = new FibonacciHeap<TDistance, TVertex>(HeapDirection.Increasing, distanceComparison);
		}

		public FibonacciQueue(
			Dictionary<TVertex, TDistance> values,
			Func<TDistance, TDistance, int> distanceComparison
		)
		{
			Contract.Requires(values != null);
			Contract.Requires(distanceComparison != null);

			distances = AlgorithmExtensions.GetIndexer(values);
			cells = new Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>>(values.Count);
			foreach (var kv in values)
				cells.Add(
					kv.Key,
					new FibonacciHeapCell<TDistance, TVertex>
					{
						Priority = kv.Value,
						Value = kv.Key,
						Removed = true
					}
				);
			heap = new FibonacciHeap<TDistance, TVertex>(HeapDirection.Increasing, distanceComparison);
		}

		public FibonacciQueue(
			Dictionary<TVertex, TDistance> values)
			: this(values, Comparer<TDistance>.Default.Compare)
		{
		}

		#region IQueue<TVertex> Members

		public int Count
		{
			[Pure] get => heap.Count;
		}

		[Pure]
		public bool Contains(TVertex value)
		{
			FibonacciHeapCell<TDistance, TVertex> result;
			return
				cells.TryGetValue(value, out result) &&
				!result.Removed;
		}

		public void Update(TVertex v)
		{
			heap.ChangeKey(cells[v], distances(v));
		}

		public void Enqueue(TVertex value)
		{
			cells[value] = heap.Enqueue(distances(value), value);
		}

		public TVertex Dequeue()
		{
			var result = heap.Top;
			Contract.Assert(result != null);
			heap.Dequeue();
			return result.Value;
		}

		public TVertex Peek()
		{
			Contract.Assert(heap.Top != null);

			return heap.Top.Value;
		}

		[Pure]
		public TVertex[] ToArray()
		{
			TVertex[] result = new TVertex[heap.Count];
			var i = 0;
			foreach (var entry in heap)
				result[i++] = entry.Value;
			return result;
		}

		#endregion
	}
}