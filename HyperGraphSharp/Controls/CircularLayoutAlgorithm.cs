using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Controls
{
	public class CircularLayoutAlgorithm
	{
		public CircularLayoutAlgorithm(HyperGraph graph,
			IDictionary<Vertex, Point> vertexPositions,
			IDictionary<Vertex, Size> vertexSizes)
		{
			Graph = graph;
			VertexPositions = vertexPositions;
			VertexSizes = vertexSizes;
		}

		public HyperGraph Graph { get; }
		public IDictionary<Vertex, Point> VertexPositions { get; }
		public IDictionary<Vertex, Size> VertexSizes { get; }

		public IDictionary<HyperEdge, Point[]>? EdgeRoutes { get; private set; }

		public void Compute()
		{
			double perimeter = 0;
			var halfSize = new double[Graph.Vertices.Count];
			var i = 0;
			foreach (var vSize in Graph.Vertices.Select(v => VertexSizes[v]))
			{
				halfSize[i] = Math.Sqrt(vSize.Width * vSize.Width + vSize.Height * vSize.Height) * 0.5;
				perimeter += halfSize[i] * 2;
				i++;
			}

			var radius = perimeter / (2 * Math.PI);

			//
			// pre-calculation
			//
			double angle = 0, a;
			i = 0;
			foreach (var v in Graph.Vertices)
			{
				a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
				angle += a;
				VertexPositions[v] = new Point
				{
					X = Math.Cos(angle) * radius + radius,
					Y = Math.Sin(angle) * radius + radius
				};
				angle += a;
				i++;
			}

			// recalculate radius
			radius = angle / (2 * Math.PI) * radius;

			// calculation
			angle = 0;
			i = 0;
			foreach (var v in Graph.Vertices)
			{
				a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
				angle += a;
				VertexPositions[v] = new Point
				{
					X = Math.Cos(angle) * radius + radius,
					Y = Math.Sin(angle) * radius + radius
				};
				angle += a;
				i++;
			}

			EdgeRoutes = RouteEdges();
		}

		public IDictionary<HyperEdge, Point[]> RouteEdges()
		{
			var edges = Graph.HyperEdges;
			var resDict = new Dictionary<HyperEdge, Point[]>();

			foreach (var edge in edges)
			{
				var vertexPositions = VertexPositions
				   .Where(pair => edge.Vertices.Contains(pair.Key))
				   .Select(pair => pair.Value).ToList();
				var medianPoint = new Point
				{
					X = vertexPositions.Average(p => p.X),
					Y = vertexPositions.Average(p => p.Y)
				};
				resDict.Add(edge, vertexPositions.Append(medianPoint).ToArray());
			}

			return resDict;
		}
	}
}