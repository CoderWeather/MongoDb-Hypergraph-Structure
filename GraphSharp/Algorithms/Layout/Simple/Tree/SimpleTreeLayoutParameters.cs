namespace GraphSharp.Algorithms.Layout.Simple.Tree
{
	public class SimpleTreeLayoutParameters : LayoutParametersBase
	{
		private LayoutDirection direction = LayoutDirection.TopToBottom;

		private double layerGap = 10;

		private bool optimizeWidthAndHeight;

		private SpanningTreeGeneration spanningTreeGeneration = SpanningTreeGeneration.DFS;
		private double vertexGap = 10;

		private double widthPerHeight = 1.0;

		/// <summary>
		///     Gets or sets the gap between the vertices.
		/// </summary>
		public double VertexGap
		{
			get => vertexGap;
			set
			{
				if (vertexGap != value)
				{
					vertexGap = value;
					NotifyPropertyChanged("VertexGap");
				}
			}
		}

		/// <summary>
		///     Gets or sets the gap between the layers.
		/// </summary>
		public double LayerGap
		{
			get => layerGap;
			set
			{
				if (layerGap != value)
				{
					layerGap = value;
					NotifyPropertyChanged("LayerGap");
				}
			}
		}

		/// <summary>
		///     Gets or sets the direction of the layout.
		/// </summary>
		public LayoutDirection Direction
		{
			get => direction;
			set
			{
				if (direction != value)
				{
					direction = value;
					NotifyPropertyChanged("Direction");
				}
			}
		}

		/// <summary>
		///     Gets or sets the direction of the layout.
		/// </summary>
		public SpanningTreeGeneration SpanningTreeGeneration
		{
			get => spanningTreeGeneration;
			set
			{
				if (spanningTreeGeneration != value)
				{
					spanningTreeGeneration = value;
					NotifyPropertyChanged("SpanningTreeGeneration");
				}
			}
		}

		public bool OptimizeWidthAndHeight
		{
			get => optimizeWidthAndHeight;
			set
			{
				if (value == optimizeWidthAndHeight)
					return;

				optimizeWidthAndHeight = value;
				NotifyPropertyChanged("OptimizeWidthAndHeight");
			}
		}

		public double WidthPerHeight
		{
			get => widthPerHeight;
			set
			{
				if (value == widthPerHeight)
					return;

				widthPerHeight = value;
				NotifyPropertyChanged("WidthPerHeight");
			}
		}
	}
}