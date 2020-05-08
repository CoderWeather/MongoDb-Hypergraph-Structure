using System;
using System.Runtime.Serialization;

namespace QuickGraph
{
	[Serializable]
	public abstract class QuickGraphException
		: Exception
	{
		protected QuickGraphException()
		{
		}

		protected QuickGraphException(string message) : base(message)
		{
		}

		protected QuickGraphException(string message, Exception inner) : base(message, inner)
		{
		}

		protected QuickGraphException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}