using System;
using System.Runtime.Serialization;

namespace QuickGraph
{
	[Serializable]
	public class NegativeCycleGraphException
		: QuickGraphException
	{
		public NegativeCycleGraphException()
		{
		}

		public NegativeCycleGraphException(string message) : base(message)
		{
		}

		public NegativeCycleGraphException(string message, Exception inner) : base(message, inner)
		{
		}

		protected NegativeCycleGraphException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}