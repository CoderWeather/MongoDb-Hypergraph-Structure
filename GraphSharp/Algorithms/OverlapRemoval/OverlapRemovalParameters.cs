using System.ComponentModel;

namespace GraphSharp.Algorithms.OverlapRemoval
{
	public class OverlapRemovalParameters : IOverlapRemovalParameters
	{
		private float horizontalGap = 10;
		private float verticalGap = 10;

		public float VerticalGap
		{
			get => verticalGap;
			set
			{
				if (verticalGap != value)
				{
					verticalGap = value;
					NotifyChanged("VerticalGap");
				}
			}
		}

		public float HorizontalGap
		{
			get => horizontalGap;
			set
			{
				if (horizontalGap != value)
				{
					horizontalGap = value;
					NotifyChanged("HorizontalGap");
				}
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}