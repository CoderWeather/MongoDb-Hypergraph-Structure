using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Controls
{
    public partial class GraphLayout<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        protected void RemoveAllGraphElement()
        {
            foreach (var vertex in VertexControls.Keys.ToArray())
                RemoveVertexControl(vertex);
            foreach (var edge in EdgeControls.Keys.ToArray())
                RemoveEdgeControl(edge);
            VertexControls.Clear();
            EdgeControls.Clear();
        }

        /// <summary>
        ///     If the graph has been changed, the elements will be regenerated.
        /// </summary>
        protected void RecreateGraphElements(bool tryKeepControls)
        {
            if (Graph == null)
            {
                RemoveAllGraphElement();
            }
            else
            {
                if (tryKeepControls && !IsCompoundMode)
                {
                    //remove the old graph elements
                    foreach (var (edge, _) in EdgeControls.ToList())
                    {
                        var remove = false;
                        try
                        {
                            remove = !Graph.ContainsEdge(edge.Source, edge.Target) ||
                                     !Graph.ContainsEdge(edge);
                        }
                        catch
                        {
                            // ignored
                        }

                        if (remove) RemoveEdgeControl(edge);
                    }

                    foreach (var (vertex, _) in VertexControls
                        .ToList()
                        .Where(kvp => !Graph.ContainsVertex(kvp.Key)))
                    {
                        RemoveVertexControl(vertex);
                    }
                }
                else
                {
                    RemoveAllGraphElement();
                }

                //
                // Presenters for vertices
                //
                foreach (var vertex in Graph.Vertices)
                    if (!VertexControls.ContainsKey(vertex))
                        CreateVertexControl(vertex);

                //
                // Presenters for edges
                //
                foreach (var edge in Graph.Edges)
                    if (!EdgeControls.ContainsKey(edge))
                        CreateEdgeControl(edge);

                //
                // subscribe to events of the Graph mutations
                //
                if (!IsCompoundMode)
                {
                    if (Graph is IMutableBidirectionalGraph<TVertex, TEdge> mutableGraph)
                    {
                        mutableGraph.VertexAdded += OnMutableGraph_VertexAdded;
                        mutableGraph.VertexRemoved += OnMutableGraph_VertexRemoved;
                        mutableGraph.EdgeAdded += OnMutableGraph_EdgeAdded;
                        mutableGraph.EdgeRemoved += OnMutableGraph_EdgeRemoved;
                    }
                }
            }

            Sizes = null;
        }

        private void DoNotificationLayout()
        {
            lock (_notificationSyncRoot)
            {
                _lastNotificationTimestamp = DateTime.Now;
            }

            if (Worker != null)
                return;

            Worker = new BackgroundWorker();
            Worker.DoWork += (s, e) =>
            {
                var w = (BackgroundWorker) s;
                lock (_notificationSyncRoot)
                {
                    while (DateTime.Now - _lastNotificationTimestamp < _notificationLayoutDelay)
                    {
                        Thread.Sleep(_notificationLayoutDelay);
                        if (w.CancellationPending)
                            break;
                    }
                }
            };
            Worker.RunWorkerCompleted += (s, e) =>
            {
                Worker = null;
                OnMutation();
                ContinueLayout();
                HighlightAlgorithm?.ResetHighlight();
            };
            Worker.RunWorkerAsync();
        }

        private void OnMutation()
        {
            while (_edgesRemoved.Count > 0)
            {
                var edge = _edgesRemoved.Dequeue();
                RemoveEdgeControl(edge);
            }

            while (_verticesRemoved.Count > 0)
            {
                var vertex = _verticesRemoved.Dequeue();
                RemoveVertexControl(vertex);
            }

            var verticesToInitPos = _verticesAdded.ToList();
            while (_verticesAdded.Count > 0)
            {
                var vertex = _verticesAdded.Dequeue();
                CreateVertexControl(vertex);
            }

            while (_edgesAdded.Count > 0)
            {
                var edge = _edgesAdded.Dequeue();
                CreateEdgeControl(edge);
            }

            foreach (var vertex in verticesToInitPos)
                InitializePosition(vertex);
        }

        private void OnMutableGraph_EdgeRemoved(TEdge edge)
        {
            if (EdgeControls.ContainsKey(edge))
            {
                _edgesRemoved.Enqueue(edge);
                DoNotificationLayout();
            }
        }

        private void OnMutableGraph_EdgeAdded(TEdge edge)
        {
            _edgesAdded.Enqueue(edge);
            DoNotificationLayout();
        }

        private void OnMutableGraph_VertexRemoved(TVertex vertex)
        {
            if (VertexControls.ContainsKey(vertex))
            {
                _verticesRemoved.Enqueue(vertex);
                DoNotificationLayout();
            }
        }

        private void OnMutableGraph_VertexAdded(TVertex vertex)
        {
            _verticesAdded.Enqueue(vertex);
            DoNotificationLayout();
        }

        public VertexControl? GetVertexControl(TVertex vertex)
        {
            VertexControls.TryGetValue(vertex, out var vc);
            return vc;
        }

        protected VertexControl GetOrCreateVertexControl(TVertex vertex)
        {
            if (!VertexControls.ContainsKey(vertex))
                CreateVertexControl(vertex);

            return VertexControls[vertex];
        }

        protected virtual void CreateVertexControl(TVertex vertex)
        {
            VertexControl presenter;
            var compoundGraph = Graph as ICompoundGraph<TVertex, TEdge>;

            if (IsCompoundMode && compoundGraph != null && compoundGraph.IsCompoundVertex(vertex))
            {
                var compoundPresenter = new CompoundVertexControl
                {
                    Vertex = vertex,
                    DataContext = vertex
                };
                compoundPresenter.Expanded += CompoundVertexControl_ExpandedOrCollapsed;
                compoundPresenter.Collapsed += CompoundVertexControl_ExpandedOrCollapsed;
                presenter = compoundPresenter;
            }
            else
            {
                // Create the Control of the vertex
                presenter = new VertexControl
                {
                    Vertex = vertex,
                    DataContext = vertex
                };
            }

            //var presenter = _vertexPool.GetObject();
            //presenter.Vertex = vertex;
            VertexControls[vertex] = presenter;
            presenter.RootCanvas = this;

            if (IsCompoundMode && compoundGraph != null && compoundGraph.IsChildVertex(vertex))
            {
                var parent = compoundGraph.GetParent(vertex);
                var parentControl = GetOrCreateVertexControl(parent) as CompoundVertexControl;

                Debug.Assert(parentControl != null);

                parentControl.Vertices.Add(presenter);
            }
            else
            {
                //add the presenter to the GraphLayout
                Children.Add(presenter);
            }

            //Measuring & Arrange
            presenter.InvalidateMeasure();
            SetHighlightProperties(vertex, presenter);
            RunCreationTransition(presenter);
        }

        protected virtual void InitializePosition(TVertex vertex)
        {
            var presenter = VertexControls[vertex];
            //initialize position
            if (Graph.Degree(vertex) > 0)
            {
                var pos = new Point();
                var count = 0;
                foreach (var neighbour in Graph.GetNeighbours(vertex))
                {
                    if (VertexControls.TryGetValue(neighbour, out var neighbourControl))
                    {
                        var x = GetX(neighbourControl);
                        var y = GetY(neighbourControl);
                        pos.X += double.IsNaN(x) ? 0.0 : x;
                        pos.Y += double.IsNaN(y) ? 0.0 : y;
                        count++;
                    }
                }

                if (count > 0)
                {
                    pos.X /= count;
                    pos.Y /= count;
                    SetX(presenter, pos.X);
                    SetY(presenter, pos.Y);
                }
            }
        }

        private static void CompoundVertexControl_ExpandedOrCollapsed(object sender, RoutedEventArgs e) =>
            throw new NotImplementedException();

        public EdgeControl? GetEdgeControl(TEdge edge)
        {
            EdgeControls.TryGetValue(edge, out var ec);
            return ec;
        }

        protected EdgeControl GetOrCreateEdgeControl(TEdge edge)
        {
            if (!EdgeControls.ContainsKey(edge))
                CreateEdgeControl(edge);

            return EdgeControls[edge];
        }

        protected virtual void CreateEdgeControl(TEdge edge)
        {
            var edgeControl = new EdgeControl
            {
                Edge = edge,
                DataContext = edge
            };
            //var edgeControl = _edgePool.GetObject();
            //edgeControl.Edge = edge;
            EdgeControls[edge] = edgeControl;

            //set the Source and the Target
            edgeControl.Source = VertexControls[edge.Source];
            edgeControl.Target = VertexControls[edge.Target];

            if (ActualLayoutMode == Algorithms.Layout.LayoutMode.Simple)
                Children.Insert(0, edgeControl);
            else
                Children.Add(edgeControl);
            SetHighlightProperties(edge, edgeControl);
            RunCreationTransition(edgeControl);
        }

        protected virtual void RemoveVertexControl(TVertex vertex)
        {
            RunDestructionTransition(VertexControls[vertex], false);
            VertexControls.Remove(vertex);
        }

        protected virtual void RemoveEdgeControl(TEdge edge)
        {
            RunDestructionTransition(EdgeControls[edge], false);
            EdgeControls.Remove(edge);
        }
    }
}