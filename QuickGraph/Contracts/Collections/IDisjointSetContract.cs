using System.Diagnostics.Contracts;

namespace QuickGraph.Collections.Contracts
{
	[ContractClassFor(typeof(IDisjointSet<>))]
	internal abstract class IDisjointSetContract<T>
		: IDisjointSet<T>
	{
		int IDisjointSet<T>.SetCount => default;

		int IDisjointSet<T>.ElementCount => default;


		void IDisjointSet<T>.MakeSet(T value)
		{
			IDisjointSet<T> ithis = this;
			Contract.Requires(value != null);
			Contract.Requires(!ithis.Contains(value));
			Contract.Ensures(ithis.Contains(value));
			Contract.Ensures(ithis.SetCount == Contract.OldValue(ithis.SetCount) + 1);
			Contract.Ensures(ithis.ElementCount == Contract.OldValue(ithis.ElementCount) + 1);
		}

		T IDisjointSet<T>.FindSet(T value)
		{
			IDisjointSet<T> ithis = this;
			Contract.Requires(value != null);
			Contract.Requires(ithis.Contains(value));

			return default;
		}

		bool IDisjointSet<T>.Union(T left, T right)
		{
			IDisjointSet<T> ithis = this;
			Contract.Requires(left != null);
			Contract.Requires(ithis.Contains(left));
			Contract.Requires(right != null);
			Contract.Requires(ithis.Contains(right));

			return default;
		}

		[Pure]
		bool IDisjointSet<T>.Contains(T value)
		{
			return default;
		}

		bool IDisjointSet<T>.AreInSameSet(T left, T right)
		{
			IDisjointSet<T> ithis = this;
			Contract.Requires(left != null);
			Contract.Requires(right != null);
			Contract.Requires(ithis.Contains(left));
			Contract.Requires(ithis.Contains(right));

			return default;
		}

		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			IDisjointSet<T> ithis = this;
			Contract.Invariant(0 <= ithis.SetCount);
			Contract.Invariant(ithis.SetCount <= ithis.ElementCount);
		}
	}
}