using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.ConnectedComponents
{
	[Serializable]
	public sealed class WeaklyConnectedComponentsAlgorithm<TVertex, TEdge> :
		AlgorithmBase<IVertexListGraph<TVertex, TEdge>>,
		IConnectedComponentAlgorithm<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private readonly Dictionary<int, int> componentEquivalences = new Dictionary<int, int>();
		private int currentComponent;

		public WeaklyConnectedComponentsAlgorithm(IVertexListGraph<TVertex, TEdge> visitedGraph)
			: this(visitedGraph, new Dictionary<TVertex, int>())
		{
		}

		public WeaklyConnectedComponentsAlgorithm(
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, int> components)
			: this(null, visitedGraph, components)
		{
		}

		public WeaklyConnectedComponentsAlgorithm(
			IAlgorithmComponent host,
			IVertexListGraph<TVertex, TEdge> visitedGraph,
			IDictionary<TVertex, int> components)
			: base(host, visitedGraph)
		{
			Contract.Requires(components != null);

			Components = components;
		}

		public IDictionary<TVertex, int> Components { get; }

		public int ComponentCount { get; private set; }

		protected override void Initialize()
		{
			ComponentCount = 0;
			currentComponent = 0;
			componentEquivalences.Clear();
			Components.Clear();
		}

		protected override void InternalCompute()
		{
			Contract.Ensures(0 <= ComponentCount && ComponentCount <= VisitedGraph.VertexCount);
			Contract.Ensures(VisitedGraph.Vertices.All(v => 0 <= Components[v] && Components[v] < ComponentCount));

			// shortcut for empty graph
			if (VisitedGraph.IsVerticesEmpty)
				return;

			var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(VisitedGraph);
			try
			{
				dfs.StartVertex += dfs_StartVertex;
				dfs.TreeEdge += dfs_TreeEdge;
				dfs.ForwardOrCrossEdge += dfs_ForwardOrCrossEdge;

				dfs.Compute();
			}
			finally
			{
				dfs.StartVertex -= dfs_StartVertex;
				dfs.TreeEdge -= dfs_TreeEdge;
				dfs.ForwardOrCrossEdge -= dfs_ForwardOrCrossEdge;
			}

			// updating component numbers
			foreach (var v in VisitedGraph.Vertices)
			{
				var component = Components[v];
				var equivalent = GetComponentEquivalence(component);
				if (component != equivalent)
					Components[v] = equivalent;
			}

			componentEquivalences.Clear();
		}

		private void dfs_StartVertex(TVertex v)
		{
			// we are looking on a new tree
			currentComponent = componentEquivalences.Count;
			componentEquivalences.Add(currentComponent, currentComponent);
			ComponentCount++;
			Components.Add(v, currentComponent);
		}

		private void dfs_TreeEdge(TEdge e)
		{
			// new edge, we store with the current component number
			Components.Add(e.Target, currentComponent);
		}

		private int GetComponentEquivalence(int component)
		{
			var equivalent = component;
			var temp = componentEquivalences[equivalent];
			var compress = false;
			while (temp != equivalent)
			{
				equivalent = temp;
				temp = componentEquivalences[equivalent];
				compress = true;
			}

			// path compression
			if (compress)
			{
				var c = component;
				temp = componentEquivalences[c];
				while (temp != equivalent)
				{
					temp = componentEquivalences[c];
					componentEquivalences[c] = equivalent;
				}
			}

			return equivalent;
		}

		private void dfs_ForwardOrCrossEdge(TEdge e)
		{
			// we have touched another tree, updating count and current component
			var otherComponent = GetComponentEquivalence(Components[e.Target]);
			if (otherComponent != currentComponent)
			{
				ComponentCount--;
				Contract.Assert(ComponentCount > 0);
				if (currentComponent > otherComponent)
				{
					componentEquivalences[currentComponent] = otherComponent;
					currentComponent = otherComponent;
				}
				else
				{
					componentEquivalences[otherComponent] = currentComponent;
				}
			}
		}
	}
}