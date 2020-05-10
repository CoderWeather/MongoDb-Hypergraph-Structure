using System.Collections.Generic;
using System.Linq;
using AppLib.GraphModels;

namespace DesktopApp.Extensions
{
	public static class GraphExtensions
	{
		public static IEnumerable<Vertex> GetNeighbours(this Vertex vertex)
		{
			return vertex.HyperEdges.SelectMany(edge => edge.Vertices).Distinct();
		}
	}
}