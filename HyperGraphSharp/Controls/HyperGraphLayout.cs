﻿using System.Collections.Generic;
using System.Windows;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Controls
{
	public partial class HyperGraphLayout : HyperGraphCanvas
	{
		#region Protected Constructor

		protected HyperGraphLayout()
		{
			// _edgesAdded = new Queue<THyperEdge>();
			// _edgesRemoved = new Queue<THyperEdge>();
			// _verticesAdded = new Queue<TVertex>();
			// _verticesRemoved = new Queue<TVertex>();
			HyperEdgeControls = new Dictionary<HyperEdge, HyperEdgeControl>();
			VertexControls = new Dictionary<Vertex, VertexControl>();
		}

		#endregion

		#region Public Methods

        public virtual void Layout()
		{
			if (Graph is null || Graph.Vertices.Count == 0) 
                return;

			UpdateLayout();

            if (!IsLoaded)
            {
				void Handler(object s, RoutedEventArgs e)
                {
                    Layout();
					if(e.Source is HyperGraphLayout graphLayout)
                        graphLayout.Loaded -= Handler;
                }

                Loaded += Handler;
                return;
			}

			var layoutAlgorithm = new CircularLayoutAlgorithm(
				Graph, LatestVertexPositions, ActualVertexSizes);
			layoutAlgorithm.Compute();

            foreach (var (vertex, newPos) in layoutAlgorithm.VertexPositions)
            {
				SetX(VertexControls[vertex], newPos.X);
				SetY(VertexControls[vertex], newPos.Y);
            }
			// InitVertexPositions();
		}

        #endregion

		#region Private Fields

		// private readonly Queue<THyperEdge> _edgesAdded;
		// private readonly Queue<THyperEdge> _edgesRemoved;
		//
		// private readonly Queue<TVertex> _verticesAdded;
		// private readonly Queue<TVertex> _verticesRemoved;

		#endregion

		#region Protected Fields

		protected readonly Dictionary<HyperEdge, HyperEdgeControl> HyperEdgeControls;
		protected readonly Dictionary<Vertex, VertexControl> VertexControls;

		#endregion

		#region Protected Properties

		#endregion
	}
}