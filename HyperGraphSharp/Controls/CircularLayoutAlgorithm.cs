using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using HyperGraphSharp.Extensions;
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
            radius *= 2;
            var angleStep = 360d / Graph.Vertices.Count;
            var curAngle = 0d;

            var center = new Point(0d, 0d);
            var vertexPoint = new Point(radius, 0d);

            foreach (var v in Graph.Vertices)
            {
                var m = new Matrix();
                m.RotateAt(curAngle, center.X, center.Y);
                var tempVertex = m.Transform(vertexPoint);
                VertexPositions[v] = tempVertex;
                curAngle += angleStep;
            }
        }

        private static Point NearestToCenterPoint(Point p, Size s, Point c)
        {
            var angle = p.AngleWith(c);

            var x = p.X + (s.Width / 2d) * Math.Cos(angle);
            var y = p.Y + (s.Height / 2d) * -Math.Sign(angle);

            return new Point(x, y);
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