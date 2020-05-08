using System;
using System.Runtime.Serialization;

namespace QuickGraph
{
	[Serializable]
	public class ParallelEdgeNotAllowedException
		: QuickGraphException
	{
		public ParallelEdgeNotAllowedException()
		{
		}

		public ParallelEdgeNotAllowedException(string message) : base(message)
		{
		}

		public ParallelEdgeNotAllowedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ParallelEdgeNotAllowedException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}