using System;
using System.Runtime.Serialization;

namespace QuickGraph
{
	[Serializable]
	public class NonAcyclicGraphException
		: QuickGraphException
	{
		public NonAcyclicGraphException()
		{
		}

		public NonAcyclicGraphException(string message) : base(message)
		{
		}

		public NonAcyclicGraphException(string message, Exception inner) : base(message, inner)
		{
		}

		protected NonAcyclicGraphException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}