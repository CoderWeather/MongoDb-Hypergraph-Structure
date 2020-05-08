using System.ComponentModel;

namespace GraphSharp.Algorithms.EdgeRouting
{
	public class EdgeRoutingParameters : IEdgeRoutingParameters
	{
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