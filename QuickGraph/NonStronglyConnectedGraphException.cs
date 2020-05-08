using System;
using System.Runtime.Serialization;

namespace QuickGraph
{
    /// <summary>
    ///     Exception raised when an algorithm detects a non-strongly connected graph.
    /// </summary>
    [Serializable]
	public class NonStronglyConnectedGraphException
		: QuickGraphException
	{
		public NonStronglyConnectedGraphException()
		{
		}

		public NonStronglyConnectedGraphException(string message) : base(message)
		{
		}

		public NonStronglyConnectedGraphException(string message, Exception inner) : base(message, inner)
		{
		}

		protected NonStronglyConnectedGraphException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}