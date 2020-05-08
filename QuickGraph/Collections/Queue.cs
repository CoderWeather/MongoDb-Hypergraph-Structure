using System;

namespace QuickGraph.Collections
{
	[Serializable]
	public sealed class Queue<T> : System.Collections.Generic.Queue<T>,
		IQueue<T>
	{
	}
}