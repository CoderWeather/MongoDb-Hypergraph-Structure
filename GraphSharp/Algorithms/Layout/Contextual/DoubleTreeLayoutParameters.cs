namespace GraphSharp.Algorithms.Layout.Contextual
{
	public enum DoubleTreeSides
	{
		Side1,
		Side2
	}

	public class DoubleTreeLayoutParameters : LayoutParametersBase
	{
		private LayoutDirection direction = LayoutDirection.LeftToRight;

		private double layerGap = 10;

		private DoubleTreeSides prioritizedTreeSide = DoubleTreeSides.Side1;

		private double vertexGap = 10;

		/// <summary>
		///     Gets or sets the layout direction.
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
		///     Gets or sets the gap between the neighbor vertices in a layer.
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
		///     Gets or sets the the prioritized tree side (the one with the bigger priority: where a vertex
		///     goes if it should go on both side).
		/// </summary>
		public DoubleTreeSides PrioritizedTreeSide
		{
			get => prioritizedTreeSide;
			set
			{
				if (prioritizedTreeSide != value)
				{
					prioritizedTreeSide = value;
					NotifyPropertyChanged("PrioritizedTreeSide");
				}
			}
		}
	}
}