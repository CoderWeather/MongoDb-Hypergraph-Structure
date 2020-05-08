namespace GraphSharp.Algorithms.Layout.Simple.FDP
{
	public class KKLayoutParameters : LayoutParametersBase
	{
		private double _k = 1;


		private bool adjustForGravity;

		private double disconnectedMultiplier = 0.5;

		private bool exchangeVertices;

		private double height = 300;

		private double lengthFactor = 1;

		private int maxIterations = 200;
		private double width = 300;

		/// <summary>
		///     Width of the bounding box.
		/// </summary>
		public double Width
		{
			get => width;
			set
			{
				width = value;
				NotifyPropertyChanged("Width");
			}
		}

		/// <summary>
		///     Height of the bounding box.
		/// </summary>
		public double Height
		{
			get => height;
			set
			{
				height = value;
				NotifyPropertyChanged("Height");
			}
		}

		/// <summary>
		///     Maximum number of the iterations.
		/// </summary>
		public int MaxIterations
		{
			get => maxIterations;
			set
			{
				maxIterations = value;
				NotifyPropertyChanged("MaxIterations");
			}
		}

		public double K
		{
			get => _k;
			set
			{
				_k = value;
				NotifyPropertyChanged("K");
			}
		}

		/// <summary>
		///     If true, then after the layout process, the vertices will be moved, so the barycenter will be
		///     in the center point of the bounding box.
		/// </summary>
		public bool AdjustForGravity
		{
			get => adjustForGravity;
			set
			{
				adjustForGravity = value;
				NotifyPropertyChanged("AdjustForGravity");
			}
		}

		public bool ExchangeVertices
		{
			get => exchangeVertices;
			set
			{
				exchangeVertices = value;
				NotifyPropertyChanged("ExchangeVertices");
			}
		}

		/// <summary>
		///     Multiplier of the ideal edge length. (With this parameter the user can modify the ideal edge length).
		/// </summary>
		public double LengthFactor
		{
			get => lengthFactor;
			set
			{
				lengthFactor = value;
				NotifyPropertyChanged("LengthFactor");
			}
		}

		/// <summary>
		///     Ideal distance between the disconnected points (1 is equal the ideal edge length).
		/// </summary>
		public double DisconnectedMultiplier
		{
			get => disconnectedMultiplier;
			set
			{
				disconnectedMultiplier = value;
				NotifyPropertyChanged("DisconnectedMultiplier");
			}
		}
	}
}