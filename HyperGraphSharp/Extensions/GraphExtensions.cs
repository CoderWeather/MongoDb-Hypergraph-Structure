using System.Collections.Generic;
using System.Linq;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Extensions
{
	public static class GraphExtensions
	{
		public static IEnumerable<Vertex> GetNeighbours(this Vertex vertex)
		{
			return vertex.HyperEdges.SelectMany(edge => edge.Vertices).Distinct();
		}
	}
}