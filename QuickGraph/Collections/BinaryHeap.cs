using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
    /// <summary>
    ///     Binary heap
    /// </summary>
    /// <remarks>
    ///     Indexing rules:
    ///     parent index: index ¡ 1)/2
    ///     left child: 2 * index + 1
    ///     right child: 2 * index + 2
    ///     Reference:
    ///     http://dotnetslackers.com/Community/files/folders/data-structures-and-algorithms/entry28722.aspx
    /// </remarks>
    /// <typeparam name="TValue">type of the value</typeparam>
    /// <typeparam name="TPriority">type of the priority metric</typeparam>
    [DebuggerDisplay("Count = {Count}")]
	public class BinaryHeap<TPriority, TValue>
		: IEnumerable<KeyValuePair<TPriority, TValue>>
	{
		private KeyValuePair<TPriority, TValue>[] items;
		private int version;

		private const int DefaultCapacity = 16;

		public BinaryHeap()
			: this(DefaultCapacity, Comparer<TPriority>.Default.Compare)
		{
		}

		public BinaryHeap(Func<TPriority, TPriority, int> priorityComparison)
			: this(DefaultCapacity, priorityComparison)
		{
		}

		public BinaryHeap(int capacity, Func<TPriority, TPriority, int> priorityComparison)
		{
			Contract.Requires(capacity >= 0);
			Contract.Requires(priorityComparison != null);

			items = new KeyValuePair<TPriority, TValue>[capacity];
			PriorityComparison = priorityComparison;
		}

		public Func<TPriority, TPriority, int> PriorityComparison { get; }

		public int Capacity => items.Length;

		public int Count { get; private set; }

		public void Add(TPriority priority, TValue value)
		{
			version++;
			ResizeArray();
			items[Count++] = new KeyValuePair<TPriority, TValue>(priority, value);
			MinHeapifyDown(Count - 1);
		}

		private void MinHeapifyDown(int start)
		{
			var i = start;
			var j = (i - 1) / 2;
			while (i > 0 &&
				Less(i, j))
			{
				Swap(i, j);
				i = j;
				j = (i - 1) / 2;
			}
		}

		public TValue[] ToValueArray()
		{
			var values = new TValue[items.Length];
			for (var i = 0; i < values.Length; ++i)
				values[i] = items[i].Value;
			return values;
		}

		private void ResizeArray()
		{
			if (Count == items.Length)
			{
				var newItems = new KeyValuePair<TPriority, TValue>[Count * 2 + 1];
				Array.Copy(items, newItems, Count);
				items = newItems;
			}
		}

		public KeyValuePair<TPriority, TValue> Minimum()
		{
			if (Count == 0)
				throw new InvalidOperationException();
			return items[0];
		}

		public KeyValuePair<TPriority, TValue> RemoveMinimum()
		{
			// shortcut for heap with 1 element.
			if (Count == 1)
			{
				version++;
				return items[--Count];
			}

			return RemoveAt(0);
		}

		public KeyValuePair<TPriority, TValue> RemoveAt(int index)
		{
			if (Count == 0)
				throw new InvalidOperationException("heap is empty");
			if ((index < 0) | (index >= Count) | (index + Count < Count))
				throw new ArgumentOutOfRangeException("index");

			version++;
			// shortcut for heap with 1 element.
			if (Count == 1)
				return items[--Count];

			if (index < Count - 1)
			{
				Swap(index, Count - 1);
				MinHeapifyUp(index);
			}

			return items[--Count];
		}

		private void MinHeapifyUp(int index)
		{
			var left = 2 * index + 1;
			var right = 2 * index + 2;
			while (
				left < Count - 1 && !Less(index, left) ||
				right < Count - 1 && !Less(index, right)
			)
			{
				if (right >= Count - 1 ||
					Less(left, right))
				{
					Swap(left, index);
					index = left;
				}
				else
				{
					Swap(right, index);
					index = right;
				}

				left = 2 * index + 1;
				right = 2 * index + 2;
			}
		}

		public int IndexOf(TValue value)
		{
			for (var i = 0; i < Count; i++)
				if (Equals(value, items[i].Value))
					return i;
			return -1;
		}

		public bool MinimumUpdate(TPriority priority, TValue value)
		{
			// find index
			for (var i = 0; i < Count; i++)
				if (Equals(value, items[i].Value))
				{
					if (PriorityComparison(priority, items[i].Key) <= 0)
					{
						RemoveAt(i);
						Add(priority, value);
						return true;
					}

					return false;
				}

			// not in collection
			Add(priority, value);
			return true;
		}

		public void Update(TPriority priority, TValue value)
		{
			// find index
			var index = IndexOf(value);
			// remove if needed
			if (index > -1)
				RemoveAt(index);
			Add(priority, value);
		}

		[Pure]
		private bool Less(int i, int j)
		{
			Contract.Requires(
				(i >= 0) & (i < Count) &
				(j >= 0) & (j < Count) &
				(i != j));

			return PriorityComparison(items[i].Key, items[j].Key) <= 0;
		}

		private void Swap(int i, int j)
		{
			Contract.Requires(
				i >= 0 && i < Count &&
				j >= 0 && j < Count &&
				i != j);

			var kv = items[i];
			items[i] = items[j];
			items[j] = kv;
		}

#if DEEP_INVARIANT
        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(this.items != null);
            Contract.Invariant(
                this.count > -1 &
                this.count <= this.items.Length);
            Contract.Invariant(
                EnumerableContract.All(0, this.count, index =>
                {
                    var left = 2 * index + 1;
                    var right = 2 * index + 2;
                    return  (left >= count || this.Less(index, left)) &&
                            (right >= count || this.Less(index, right));
                })
            );
        }
#endif

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
		{
			return new Enumerator(this);
		}

		private struct Enumerator :
			IEnumerator<KeyValuePair<TPriority, TValue>>
		{
			private BinaryHeap<TPriority, TValue> owner;
			private KeyValuePair<TPriority, TValue>[] items;
			private readonly int count;
			private readonly int version;
			private int index;

			public Enumerator(BinaryHeap<TPriority, TValue> owner)
			{
				this.owner = owner;
				items = owner.items;
				count = owner.Count;
				version = owner.version;
				index = -1;
			}

			public KeyValuePair<TPriority, TValue> Current
			{
				get
				{
					if (version != owner.version)
						throw new InvalidOperationException();
					if ((index < 0) | (index == count))
						throw new InvalidOperationException();
					Contract.Assert(index <= count);
					return items[index];
				}
			}

			void IDisposable.Dispose()
			{
				owner = null;
				items = null;
			}

			object IEnumerator.Current => Current;

			public bool MoveNext()
			{
				if (version != owner.version)
					throw new InvalidOperationException();
				return ++index < count;
			}

			public void Reset()
			{
				if (version != owner.version)
					throw new InvalidOperationException();
				index = -1;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}