using System;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
	[Serializable]
	public class TaggedEdge<TVertex, TTag>
		: Edge<TVertex>
			, ITagged<TTag>
	{
		private TTag tag;

		public TaggedEdge(TVertex source, TVertex target, TTag tag)
			: base(source, target)
		{
			Contract.Ensures(Equals(Tag, tag));

			this.tag = tag;
		}

		public event EventHandler TagChanged;

		public TTag Tag
		{
			get => tag;
			set
			{
				if (!Equals(tag, value))
				{
					tag = value;
					OnTagChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnTagChanged(EventArgs e)
		{
			var eh = TagChanged;
			if (eh != null)
				eh(this, e);
		}
	}
}