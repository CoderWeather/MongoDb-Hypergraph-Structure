using System;
using System.Runtime.Serialization;

namespace QuickGraph
{
	[Serializable]
	public class NegativeWeightException
		: QuickGraphException
	{
		public NegativeWeightException()
		{
		}

		public NegativeWeightException(string message) : base(message)
		{
		}

		public NegativeWeightException(string message, Exception inner) : base(message, inner)
		{
		}

		protected NegativeWeightException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}