namespace GraphSharp.Algorithms.Layout.Simple.FDP
{
	public class LinLogLayoutParameters : LayoutParametersBase
	{
		internal double attractionExponent = 1.0;

		internal double gravitationMultiplier = 0.1;

		internal int iterationCount = 100;

		internal double repulsiveExponent;

		public double AttractionExponent
		{
			get => attractionExponent;
			set
			{
				attractionExponent = value;
				NotifyPropertyChanged("AttractionExponent");
			}
		}

		public double RepulsiveExponent
		{
			get => repulsiveExponent;
			set
			{
				repulsiveExponent = value;
				NotifyPropertyChanged("RepulsiveExponent");
			}
		}

		public double GravitationMultiplier
		{
			get => gravitationMultiplier;
			set
			{
				gravitationMultiplier = value;
				NotifyPropertyChanged("GravitationMultiplier");
			}
		}

		public int IterationCount
		{
			get => iterationCount;
			set
			{
				iterationCount = value;
				NotifyPropertyChanged("IterationCount");
			}
		}
	}
}