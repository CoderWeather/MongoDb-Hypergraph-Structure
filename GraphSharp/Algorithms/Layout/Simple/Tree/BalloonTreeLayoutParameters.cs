namespace GraphSharp.Algorithms.Layout.Simple.Tree
{
	public class BalloonTreeLayoutParameters : LayoutParametersBase
	{
		internal float border = 20.0f;
		internal int minRadius = 2;

		public int MinRadius
		{
			get => minRadius;
			set
			{
				if (value != minRadius)
				{
					minRadius = value;
					NotifyPropertyChanged("MinRadius");
				}
			}
		}


		public float Border
		{
			get => border;
			set
			{
				if (value != border)
				{
					border = value;
					NotifyPropertyChanged("Border");
				}
			}
		}
	}
}