namespace GraphSharp
{
	public class WrappedVertex<TVertex>
	{
		public WrappedVertex(TVertex original)
		{
			Original = original;
		}

		public TVertex Original { get; }
	}
}