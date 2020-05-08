using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.MaximumFlow
{
	[Serializable]
	public sealed class GraphBalancerAlgorithm<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		private List<TEdge> deficientEdges = new List<TEdge>();
		private List<TVertex> deficientVertices = new List<TVertex>();
		private Dictionary<TEdge, int> preFlow = new Dictionary<TEdge, int>();

		private List<TEdge> surplusEdges = new List<TEdge>();
		private List<TVertex> surplusVertices = new List<TVertex>();

		public GraphBalancerAlgorithm(
			IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
			TVertex source,
			TVertex sink,
			VertexFactory<TVertex> vertexFactory,
			EdgeFactory<TVertex, TEdge> edgeFactory
		)
		{
			Contract.Requires(visitedGraph != null);
			Contract.Requires(vertexFactory != null);
			Contract.Requires(edgeFactory != null);
			Contract.Requires(source != null);
			Contract.Requires(visitedGraph.ContainsVertex(source));
			Contract.Requires(sink != null);
			Contract.Requires(visitedGraph.ContainsVertex(sink));

			VisitedGraph = visitedGraph;
			VertexFactory = vertexFactory;
			EdgeFactory = edgeFactory;
			Source = source;
			Sink = sink;

			// setting capacities = u(e) = +infty
			foreach (var edge in VisitedGraph.Edges)
				Capacities.Add(edge, double.MaxValue);

			// setting preflow = l(e) = 1
			foreach (var edge in VisitedGraph.Edges)
				preFlow.Add(edge, 1);
		}

		public GraphBalancerAlgorithm(
			IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
			VertexFactory<TVertex> vertexFactory,
			EdgeFactory<TVertex, TEdge> edgeFactory,
			TVertex source,
			TVertex sink,
			IDictionary<TEdge, double> capacities)
		{
			Contract.Requires(visitedGraph != null);
			Contract.Requires(vertexFactory != null);
			Contract.Requires(edgeFactory != null);
			Contract.Requires(source != null);
			Contract.Requires(visitedGraph.ContainsVertex(source));
			Contract.Requires(sink != null);
			Contract.Requires(visitedGraph.ContainsVertex(sink));
			Contract.Requires(capacities != null);

			VisitedGraph = visitedGraph;
			Source = source;
			Sink = sink;
			Capacities = capacities;

			// setting preflow = l(e) = 1
			foreach (var edge in VisitedGraph.Edges)
				preFlow.Add(edge, 1);
		}

		public IMutableBidirectionalGraph<TVertex, TEdge> VisitedGraph { get; }

		public VertexFactory<TVertex> VertexFactory { get; }

		public EdgeFactory<TVertex, TEdge> EdgeFactory { get; }

		public bool Balanced { get; private set; }

		public TVertex Source { get; }

		public TVertex Sink { get; }

		public TVertex BalancingSource { get; private set; }

		public TEdge BalancingSourceEdge { get; private set; }

		public TVertex BalancingSink { get; private set; }

		public TEdge BalancingSinkEdge { get; private set; }

		public ICollection<TVertex> SurplusVertices => surplusVertices;

		public ICollection<TEdge> SurplusEdges => surplusEdges;

		public ICollection<TVertex> DeficientVertices => deficientVertices;

		public ICollection<TEdge> DeficientEdges => deficientEdges;

		public IDictionary<TEdge, double> Capacities { get; } = new Dictionary<TEdge, double>();

		public event VertexAction<TVertex> BalancingSourceAdded;

		private void OnBalancingSourceAdded()
		{
			var eh = BalancingSourceAdded;
			if (eh != null)
				eh(Source);
		}

		public event VertexAction<TVertex> BalancingSinkAdded;

		private void OnBalancingSinkAdded()
		{
			var eh = BalancingSinkAdded;
			if (eh != null)
				eh(Sink);
		}

		public event EdgeAction<TVertex, TEdge> EdgeAdded;

		private void OnEdgeAdded(TEdge edge)
		{
			Contract.Requires(edge != null);

			var eh = EdgeAdded;
			if (eh != null)
				eh(edge);
		}

		public event VertexAction<TVertex> SurplusVertexAdded;

		private void OnSurplusVertexAdded(TVertex vertex)
		{
			Contract.Requires(vertex != null);
			var eh = SurplusVertexAdded;
			if (eh != null)
				eh(vertex);
		}

		public event VertexAction<TVertex> DeficientVertexAdded;

		private void OnDeficientVertexAdded(TVertex vertex)
		{
			Contract.Requires(vertex != null);

			var eh = DeficientVertexAdded;
			if (eh != null)
				eh(vertex);
		}

		public int GetBalancingIndex(TVertex v)
		{
			Contract.Requires(v != null);

			var bi = 0;
			foreach (var edge in VisitedGraph.OutEdges(v))
			{
				var pf = preFlow[edge];
				bi += pf;
			}

			foreach (var edge in VisitedGraph.InEdges(v))
			{
				var pf = preFlow[edge];
				bi -= pf;
			}

			return bi;
		}

		public void Balance()
		{
			if (Balanced)
				throw new InvalidOperationException("Graph already balanced");

			// step 0
			// create new source, new sink
			BalancingSource = VertexFactory();
			VisitedGraph.AddVertex(BalancingSource);
			OnBalancingSourceAdded();

			BalancingSink = VertexFactory();
			VisitedGraph.AddVertex(BalancingSink);
			OnBalancingSinkAdded();

			// step 1
			BalancingSourceEdge = EdgeFactory(BalancingSource, Source);
			VisitedGraph.AddEdge(BalancingSourceEdge);
			Capacities.Add(BalancingSourceEdge, double.MaxValue);
			preFlow.Add(BalancingSourceEdge, 0);
			OnEdgeAdded(BalancingSourceEdge);

			BalancingSinkEdge = EdgeFactory(Sink, BalancingSink);
			VisitedGraph.AddEdge(BalancingSinkEdge);
			Capacities.Add(BalancingSinkEdge, double.MaxValue);
			preFlow.Add(BalancingSinkEdge, 0);
			OnEdgeAdded(BalancingSinkEdge);

			// step 2
			// for each surplus vertex v, add (source -> v)
			foreach (var v in VisitedGraph.Vertices)
			{
				if (v.Equals(BalancingSource))
					continue;
				if (v.Equals(BalancingSink))
					continue;
				if (v.Equals(Source))
					continue;
				if (v.Equals(Sink))
					continue;

				var balacingIndex = GetBalancingIndex(v);
				if (balacingIndex == 0)
					continue;

				if (balacingIndex < 0)
				{
					// surplus vertex
					var edge = EdgeFactory(BalancingSource, v);
					VisitedGraph.AddEdge(edge);
					surplusEdges.Add(edge);
					surplusVertices.Add(v);
					preFlow.Add(edge, 0);
					Capacities.Add(edge, -balacingIndex);
					OnSurplusVertexAdded(v);
					OnEdgeAdded(edge);
				}
				else
				{
					// deficient vertex
					var edge = EdgeFactory(v, BalancingSink);
					deficientEdges.Add(edge);
					deficientVertices.Add(v);
					preFlow.Add(edge, 0);
					Capacities.Add(edge, balacingIndex);
					OnDeficientVertexAdded(v);
					OnEdgeAdded(edge);
				}
			}

			Balanced = true;
		}

		public void UnBalance()
		{
			if (!Balanced)
				throw new InvalidOperationException("Graph is not balanced");
			foreach (var edge in surplusEdges)
			{
				VisitedGraph.RemoveEdge(edge);
				Capacities.Remove(edge);
				preFlow.Remove(edge);
			}

			foreach (var edge in deficientEdges)
			{
				VisitedGraph.RemoveEdge(edge);
				Capacities.Remove(edge);
				preFlow.Remove(edge);
			}

			Capacities.Remove(BalancingSinkEdge);
			Capacities.Remove(BalancingSourceEdge);
			preFlow.Remove(BalancingSinkEdge);
			preFlow.Remove(BalancingSourceEdge);
			VisitedGraph.RemoveEdge(BalancingSourceEdge);
			VisitedGraph.RemoveEdge(BalancingSinkEdge);
			VisitedGraph.RemoveVertex(BalancingSource);
			VisitedGraph.RemoveVertex(BalancingSink);

			BalancingSource = default;
			BalancingSink = default;
			BalancingSourceEdge = default;
			BalancingSinkEdge = default;

			surplusEdges.Clear();
			deficientEdges.Clear();
			surplusVertices.Clear();
			deficientVertices.Clear();

			Balanced = false;
		}
	}
}