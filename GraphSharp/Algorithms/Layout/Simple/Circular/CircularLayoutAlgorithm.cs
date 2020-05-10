using System;
using System.Collections.Generic;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Simple.Circular
{
    public class CircularLayoutAlgorithm<TVertex, TEdge, TGraph> :
        DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, CircularLayoutParameters>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private readonly IDictionary<TVertex, Size> _sizes;

        public CircularLayoutAlgorithm(TGraph visitedGraph, IDictionary<TVertex, Point> vertexPositions,
            IDictionary<TVertex, Size> vertexSizes, CircularLayoutParameters parameters)
            : base(visitedGraph, vertexPositions, parameters)
        {
            //Contract.Requires( vertexSizes != null );
            //Contract.Requires( visitedGraph.Vertices.All( v => vertexSizes.ContainsKey( v ) ) );

            _sizes = vertexSizes;
        }

        protected override void InternalCompute()
        {
            //calculate the size of the circle
            double perimeter = 0;
            var halfSize = new double[VisitedGraph.VertexCount];
            var i = 0;
            foreach (var v in VisitedGraph.Vertices)
            {
                var s = _sizes[v];
                halfSize[i] = Math.Sqrt(s.Width * s.Width + s.Height * s.Height) * 0.5;
                perimeter += halfSize[i] * 2;
                i++;
            }

            var radius = perimeter / (2 * Math.PI);

            //
            // pre-calculation
            //
            double angle = 0, a;
            i = 0;
            foreach (var v in VisitedGraph.Vertices)
            {
                a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
                angle += a;
                if (ReportOnIterationEndNeeded)
                    VertexPositions[v] = new Point
                    {
                        X = Math.Cos(angle) * radius + radius,
                        Y = Math.Sin(angle) * radius + radius
                    };
                angle += a;
            }

            if (ReportOnIterationEndNeeded)
                OnIterationEnded(0, 50, "Pre-calculation done.", false);

            // recalculate radius
            radius = angle / (2 * Math.PI) * radius;

            // calculation
            angle = 0;
            i = 0;
            foreach (var v in VisitedGraph.Vertices)
            {
                a = Math.Sin(halfSize[i] * 0.5 / radius) * 2;
                angle += a;
                VertexPositions[v] = new Point
                {
                    X = Math.Cos(angle) * radius + radius,
                    Y = Math.Sin(angle) * radius + radius
                };
                angle += a;
            }
        }
    }
}