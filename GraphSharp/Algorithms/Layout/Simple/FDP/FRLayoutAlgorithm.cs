using System;
using System.Collections.Generic;
using System.Windows;
using QuickGraph;
using QuickGraph.Algorithms;

namespace GraphSharp.Algorithms.Layout.Simple.FDP
{
	public class
		FRLayoutAlgorithm<Vertex, Edge, Graph> : ParameterizedLayoutAlgorithmBase<Vertex, Edge, Graph,
			FRLayoutParametersBase>
		where Vertex : class
		where Edge : IEdge<Vertex>
		where Graph : IVertexAndEdgeListGraph<Vertex, Edge>
	{
		private double _maxHeight = double.PositiveInfinity;

		private double _maxWidth = double.PositiveInfinity;

        /// <summary>
        ///     Actual temperature of the 'mass'.
        /// </summary>
        private double _temperature;

		protected override FRLayoutParametersBase DefaultParameters => new FreeFRLayoutParameters();

        /// <summary>
        ///     It computes the layout of the vertices.
        /// </summary>
        protected override void InternalCompute()
		{
			//initializing the positions
			if (Parameters is BoundedFRLayoutParameters)
			{
				var param = Parameters as BoundedFRLayoutParameters;
				InitializeWithRandomPositions(param.Width, param.Height);
				_maxWidth = param.Width;
				_maxHeight = param.Height;
			}
			else
			{
				InitializeWithRandomPositions(10.0, 10.0);
			}

			Parameters.VertexCount = VisitedGraph.VertexCount;

			// Actual temperature of the 'mass'. Used for cooling.
			var minimalTemperature = Parameters.InitialTemperature * 0.01;
			_temperature = Parameters.InitialTemperature;
			for (var i = 0;
				i < Parameters._iterationLimit
			 && _temperature > minimalTemperature
			 && State != ComputationState.PendingAbortion;
				i++)
			{
				IterateOne();

				//make some cooling
				switch (Parameters._coolingFunction)
				{
					case FRCoolingFunction.Linear:
						_temperature *= 1.0 - i / (double) Parameters._iterationLimit;
						break;
					case FRCoolingFunction.Exponential:
						_temperature *= Parameters._lambda;
						break;
				}

				//iteration ended, do some report
				if (ReportOnIterationEndNeeded)
				{
					var statusInPercent = i / (double) Parameters._iterationLimit;
					OnIterationEnded(i, statusInPercent, string.Empty, true);
				}
			}
		}


		protected void IterateOne()
		{
			//create the forces (zero forces)
			var forces = new Dictionary<Vertex, Vector>();

			#region Repulsive forces

			var force = new Vector(0, 0);
			foreach (var v in VisitedGraph.Vertices)
			{
				force.X = 0;
				force.Y = 0;
				var posV = VertexPositions[v];
				foreach (var u in VisitedGraph.Vertices)
				{
					//doesn't repulse itself
					if (u.Equals(v))
						continue;

					//calculating repulsive force
					var delta = posV - VertexPositions[u];
					var length = Math.Max(delta.Length, double.Epsilon);
					delta = delta / length * Parameters.ConstantOfRepulsion / length;

					force += delta;
				}

				forces[v] = force;
			}

			#endregion

			#region Attractive forces

			foreach (var e in VisitedGraph.Edges)
			{
				var source = e.Source;
				var target = e.Target;

				//vonzуerх szбmнtбsa a kйt pont kцzt
				var delta = VertexPositions[source] - VertexPositions[target];
				var length = Math.Max(delta.Length, double.Epsilon);
				delta = delta / length * Math.Pow(length, 2) / Parameters.ConstantOfAttraction;

				forces[source] -= delta;
				forces[target] += delta;
			}

			#endregion

			#region Limit displacement

			foreach (var v in VisitedGraph.Vertices)
			{
				var pos = VertexPositions[v];

				//erх limitбlбsa a temperature-el
				var delta = forces[v];
				var length = Math.Max(delta.Length, double.Epsilon);
				delta = delta / length * Math.Min(delta.Length, _temperature);

				//erхhatбs a pontra
				pos += delta;

				//falon ne menjьnk ki
				pos.X = Math.Min(_maxWidth, Math.Max(0, pos.X));
				pos.Y = Math.Min(_maxHeight, Math.Max(0, pos.Y));
				VertexPositions[v] = pos;
			}

			#endregion
		}

		#region Constructors

		public FRLayoutAlgorithm(Graph visitedGraph)
			: base(visitedGraph)
		{
		}

		public FRLayoutAlgorithm(Graph visitedGraph, IDictionary<Vertex, Point> vertexPositions,
			FRLayoutParametersBase parameters)
			: base(visitedGraph, vertexPositions, parameters)
		{
		}

		#endregion
	}
}