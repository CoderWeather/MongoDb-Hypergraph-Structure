using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using QuickGraph.Algorithms.Services;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.ShortestPath
{
    /// <summary>
    ///     Floyd-Warshall all shortest path algorith,
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    public class FloydWarshallAllShortestPathAlgorithm<TVertex, TEdge>
		: AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		private readonly Dictionary<SEquatableEdge<TVertex>, VertexData> data;
		private readonly IDistanceRelaxer distanceRelaxer;
		private readonly Func<TEdge, double> weights;

		public FloydWarshallAllShortestPathAlgorithm(
			IAlgorithmComponent host,
			IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights,
			IDistanceRelaxer distanceRelaxer
		)
			: base(host, visitedGraph)
		{
			Contract.Requires(weights != null);
			Contract.Requires(distanceRelaxer != null);

			this.weights = weights;
			this.distanceRelaxer = distanceRelaxer;
			data = new Dictionary<SEquatableEdge<TVertex>, VertexData>();
		}

		public FloydWarshallAllShortestPathAlgorithm(
			IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights,
			IDistanceRelaxer distanceRelaxer)
			: base(visitedGraph)
		{
			Contract.Requires(weights != null);
			Contract.Requires(distanceRelaxer != null);

			this.weights = weights;
			this.distanceRelaxer = distanceRelaxer;
			data = new Dictionary<SEquatableEdge<TVertex>, VertexData>();
		}

		public FloydWarshallAllShortestPathAlgorithm(
			IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
			Func<TEdge, double> weights)
			: this(visitedGraph, weights, DistanceRelaxers.ShortestDistance)
		{
		}

		public bool TryGetDistance(TVertex source, TVertex target, out double cost)
		{
			Contract.Requires(source != null);
			Contract.Requires(target != null);

			VertexData value;
			if (data.TryGetValue(new SEquatableEdge<TVertex>(source, target), out value))
			{
				cost = value.Distance;
				return true;
			}

			cost = -1;
			return false;
		}

		public bool TryGetPath(
			TVertex source,
			TVertex target,
			out IEnumerable<TEdge> path)
		{
			Contract.Requires(source != null);
			Contract.Requires(target != null);

			if (source.Equals(target))
			{
				path = null;
				return false;
			}

#if DEBUG && !SILVERLIGHT
			var set = new HashSet<TVertex>();
			set.Add(source);
			set.Add(target);
#endif

			var edges = new EdgeList<TVertex, TEdge>();
			var todo = new Stack<SEquatableEdge<TVertex>>();
			todo.Push(new SEquatableEdge<TVertex>(source, target));
			while (todo.Count > 0)
			{
				var current = todo.Pop();
				Contract.Assert(!current.Source.Equals(current.Target));
				VertexData data;
				if (this.data.TryGetValue(current, out data))
				{
					TEdge edge;
					if (data.TryGetEdge(out edge))
					{
						edges.Add(edge);
					}
					else
					{
						TVertex intermediate;
						if (data.TryGetPredecessor(out intermediate))
						{
#if DEBUG && !SILVERLIGHT
							Contract.Assert(set.Add(intermediate));
#endif
							todo.Push(new SEquatableEdge<TVertex>(intermediate, current.Target));
							todo.Push(new SEquatableEdge<TVertex>(current.Source, intermediate));
						}
						else
						{
							Contract.Assert(false);
							path = null;
							return false;
						}
					}
				}
				else
				{
					// no path found
					path = null;
					return false;
				}
			}

			Contract.Assert(todo.Count == 0);
			Contract.Assert(edges.Count > 0);
			path = edges.ToArray();
			return true;
		}

		protected override void InternalCompute()
		{
			var cancelManager = Services.CancelManager;
			// matrix i,j -> path
			data.Clear();

			var vertices = VisitedGraph.Vertices;
			var edges = VisitedGraph.Edges;

			// prepare the matrix with initial costs
			// walk each edge and add entry in cost dictionary
			foreach (var edge in edges)
			{
				var ij = edge.ToVertexPair<TVertex, TEdge>();
				var cost = weights(edge);
				VertexData value;
				if (!data.TryGetValue(ij, out value))
					data[ij] = new VertexData(cost, edge);
				else if (cost < value.Distance)
					data[ij] = new VertexData(cost, edge);
			}

			if (cancelManager.IsCancelling) return;

			// walk each vertices and make sure cost self-cost 0
			foreach (var v in vertices)
			{
				var e = new SEquatableEdge<TVertex>(v, v);
				data[e] = new VertexData();
			}

			if (cancelManager.IsCancelling) return;

			// iterate k, i, j
			foreach (var vk in vertices)
			{
				if (cancelManager.IsCancelling) return;
				foreach (var vi in vertices)
				{
					var ik = new SEquatableEdge<TVertex>(vi, vk);
					VertexData pathik;
					if (data.TryGetValue(ik, out pathik))
						foreach (var vj in vertices)
						{
							var kj = new SEquatableEdge<TVertex>(vk, vj);

							VertexData pathkj;
							if (data.TryGetValue(kj, out pathkj))
							{
								var combined = distanceRelaxer.Combine(pathik.Distance, pathkj.Distance);
								var ij = new SEquatableEdge<TVertex>(vi, vj);
								VertexData pathij;
								if (data.TryGetValue(ij, out pathij))
								{
									if (distanceRelaxer.Compare(combined, pathij.Distance) < 0)
										data[ij] = new VertexData(combined, vk);
								}
								else
								{
									data[ij] = new VertexData(combined, vk);
								}
							}
						}
				}
			}

			// check negative cycles
			foreach (var vi in vertices)
			{
				var ii = new SEquatableEdge<TVertex>(vi, vi);
				VertexData value;
				if (data.TryGetValue(ii, out value) &&
					value.Distance < 0)
					throw new NegativeCycleGraphException();
			}
		}

		[Conditional("DEBUG")]
		public void Dump(TextWriter writer)
		{
			writer.WriteLine("data:");
			foreach (var kv in data)
				writer.WriteLine("{0}->{1}: {2}",
					kv.Key.Source,
					kv.Key.Target,
					kv.Value.ToString());
		}

		private readonly struct VertexData
		{
			public readonly double Distance;
			private readonly TVertex _predecessor;
			private readonly bool predecessorStored;
			private readonly TEdge _edge;
			private readonly bool edgeStored;

			public bool TryGetPredecessor(out TVertex predecessor)
			{
				predecessor = _predecessor;
				return predecessorStored;
			}

			public bool TryGetEdge(out TEdge edge)
			{
				edge = _edge;
				return edgeStored;
			}

			public VertexData(double distance, TEdge edge)
			{
				Contract.Requires(edge != null);
				Distance = distance;
				_predecessor = default;
				predecessorStored = false;
				_edge = edge;
				edgeStored = true;
			}

			public VertexData(double distance, TVertex predecessor)
			{
				Contract.Requires(predecessor != null);

				Distance = distance;
				_predecessor = predecessor;
				predecessorStored = true;
				_edge = default;
				edgeStored = false;
			}

			[ContractInvariantMethod]
			private void ObjectInvariant()
			{
				Contract.Invariant(!edgeStored || _edge != null);
				Contract.Invariant(!predecessorStored || _predecessor != null);
			}

			public override string ToString()
			{
				if (edgeStored)
					return string.Format("e:{0}-{1}", Distance, _edge);
				return string.Format("p:{0}-{1}", Distance, _predecessor);
			}
		}
	}
}