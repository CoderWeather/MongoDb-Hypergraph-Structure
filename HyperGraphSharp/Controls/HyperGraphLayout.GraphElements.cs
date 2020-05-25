using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HyperGraphSharp.Extensions;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Controls
{
    public partial class HyperGraphLayout
    {
        protected virtual void RemoveVertexControl(Vertex vertex)
        {
            VertexControls.Remove(vertex);
        }

        protected virtual void RemoveEdgeControl(HyperEdge hyperEdge)
        {
            HyperEdgeControls.Remove(hyperEdge);
        }

        protected void RemoveAllGraphElements()
        {
            foreach (var vertex in VertexControls.Keys.ToArray())
                RemoveVertexControl(vertex);

            foreach (var edge in HyperEdgeControls.Keys.ToArray())
                RemoveEdgeControl(edge);

            VertexControls.Clear();
            HyperEdgeControls.Clear();
        }

        protected virtual void CreateVertexControl(Vertex vertex)
        {
            var vertexControl = new VertexControl
            {
                Vertex = vertex,
                DataContext = vertex,
                RootHyperGraphCanvas = this
            };

            if (VertexControls.TryAdd(vertex, vertexControl) is false)
            {
                VertexControls[vertex] = vertexControl;
            }

            // Children.Add(vertexControl);
            InternalChildren.Add(vertexControl);

            vertexControl.InvalidateMeasure();
        }

        protected virtual void CreateHyperEdgeControl(HyperEdge hyperEdge)
        {
            var hyperEdgeControl = new HyperEdgeControl
            {
                HyperEdge = hyperEdge,
                DataContext = hyperEdge
            };

            if (HyperEdgeControls.TryAdd(hyperEdge, hyperEdgeControl) is false)
            {
                HyperEdgeControls[hyperEdge] = hyperEdgeControl;
            }

            var findVertexList = new List<VertexControl>();
            foreach (var v in hyperEdge.Vertices)
            {
                if (VertexControls.TryGetValue(v, out var vertexControl) is true)
                {
                    if (vertexControl != null)
                        findVertexList.Add(vertexControl);
                }
            }

            hyperEdgeControl.Vertices = findVertexList.ToArray();
            // hyperEdgeControl.Vertices =
            //     hyperEdge.Vertices.Select(v => VertexControls[v]).ToArray();
            foreach (var vertexControl in hyperEdgeControl.Vertices)
                vertexControl.HyperEdges.Add(hyperEdgeControl);

            InternalChildren.Add(hyperEdgeControl);
        }

        protected void RecreateGraphElements()
        {
            RemoveAllGraphElements();

            foreach (var vertex in Graph.Vertices.Where(vertex => !VertexControls.ContainsKey(vertex)))
                CreateVertexControl(vertex);

            foreach (var edge in Graph.HyperEdges.Where(edge => !HyperEdgeControls.ContainsKey(edge)))
                CreateHyperEdgeControl(edge);
        }

        public VertexControl? GetVertexControl(Vertex vertex)
        {
            VertexControls.TryGetValue(vertex, out var vc);
            return vc;
        }

        protected VertexControl GetOrCreateVertexControl(Vertex vertex)
        {
            if (!VertexControls.ContainsKey(vertex))
                CreateVertexControl(vertex);

            return VertexControls[vertex];
        }

        public HyperEdgeControl? GetEdgeControl(HyperEdge edge)
        {
            HyperEdgeControls.TryGetValue(edge, out var ec);
            return ec;
        }

        protected HyperEdgeControl GetOrCreateEdgeControl(HyperEdge edge)
        {
            if (!HyperEdgeControls.ContainsKey(edge))
                CreateHyperEdgeControl(edge);

            return HyperEdgeControls[edge];
        }

        protected virtual void InitializePosition(Vertex vertex)
        {
            var vertexControl = VertexControls[vertex];

            if (Graph.Vertices.Contains(vertex) is false || vertex.HyperEdges.Count == 0)
                return;

            var pos = new Point();
            var count = 0;
            foreach (var neighbour in vertex.GetNeighbours())
            {
                if (neighbour is null)
                    continue;

                if (VertexControls.TryGetValue(neighbour, out var neighbourControl))
                {
                    var x = GetX(neighbourControl);
                    var y = GetY(neighbourControl);
                    pos.X += double.IsNaN(x) ? 0.0 : x;
                    pos.Y += double.IsNaN(y) ? 0.0 : y;
                    count++;
                }
            }

            if (count == 0)
                return;

            pos.X /= count;
            pos.Y /= count;
            SetX(vertexControl, pos.X);
            SetY(vertexControl, pos.Y);
        }

        private void InitVertexPositions()
        {
            foreach (var vertex in Graph.Vertices)
                InitializePosition(vertex);
        }
    }
}