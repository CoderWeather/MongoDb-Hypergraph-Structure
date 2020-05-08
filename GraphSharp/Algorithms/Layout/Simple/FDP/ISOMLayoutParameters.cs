namespace GraphSharp.Algorithms.Layout.Simple.FDP
{
	public class ISOMLayoutParameters : LayoutParametersBase
	{
		private double _coolingFactor = 2;

		private double _height = 300;

		private double _initialAdaption = 0.9;

		private int _initialRadius = 5;

		private double _minAdaption;

		private int _minRadius = 1;

		private int _radiusConstantTime = 100;
		private double _width = 300;

		private int maxEpoch = 2000;

		/// <summary>
		///     Width of the bounding box. Default value is 300.
		/// </summary>
		public double Width
		{
			get => _width;
			set
			{
				_width = value;
				NotifyPropertyChanged("Width");
			}
		}

		/// <summary>
		///     Height of the bounding box. Default value is 300.
		/// </summary>
		public double Height
		{
			get => _height;
			set
			{
				_height = value;
				NotifyPropertyChanged("Height");
			}
		}

		/// <summary>
		///     Maximum iteration number. Default value is 2000.
		/// </summary>
		public int MaxEpoch
		{
			get => maxEpoch;
			set
			{
				maxEpoch = value;
				NotifyPropertyChanged("MaxEpoch");
			}
		}

		/// <summary>
		///     Radius constant time. Default value is 100.
		/// </summary>
		public int RadiusConstantTime
		{
			get => _radiusConstantTime;
			set
			{
				_radiusConstantTime = value;
				NotifyPropertyChanged("RadiusConstantTime");
			}
		}

		/// <summary>
		///     Default value is 5.
		/// </summary>
		public int InitialRadius
		{
			get => _initialRadius;
			set
			{
				_initialRadius = value;
				NotifyPropertyChanged("InitialRadius");
			}
		}

		/// <summary>
		///     Minimal radius. Default value is 1.
		/// </summary>
		public int MinRadius
		{
			get => _minRadius;
			set
			{
				_minRadius = value;
				NotifyPropertyChanged("MinRadius");
			}
		}

		/// <summary>
		///     Default value is 0.9.
		/// </summary>
		public double InitialAdaption
		{
			get => _initialAdaption;
			set
			{
				_initialAdaption = value;
				NotifyPropertyChanged("InitialAdaption");
			}
		}

		/// <summary>
		///     Default value is 0.
		/// </summary>
		public double MinAdaption
		{
			get => _minAdaption;
			set
			{
				_minAdaption = value;
				NotifyPropertyChanged("MinAdaption");
			}
		}

		/// <summary>
		///     Default value is 2.
		/// </summary>
		public double CoolingFactor
		{
			get => _coolingFactor;
			set
			{
				_coolingFactor = value;
				NotifyPropertyChanged("CoolingFactor");
			}
		}
	}
}