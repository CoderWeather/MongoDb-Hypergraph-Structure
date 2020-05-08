using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <reference-ref
    ///     idref="boost" />
    [Serializable]
	public sealed class EdgePredecessorRecorderObserver<TVertex, TEdge> :
		IObserver<IEdgePredecessorRecorderAlgorithm<TVertex, TEdge>>
		where TEdge : IEdge<TVertex>
	{
		public EdgePredecessorRecorderObserver()
			: this(new Dictionary<TEdge, TEdge>(), new List<TEdge>())
		{
		}

		public EdgePredecessorRecorderObserver(
			IDictionary<TEdge, TEdge> edgePredecessors,
			IList<TEdge> endPathEdges
		)
		{
			Contract.Requires(edgePredecessors != null);
			Contract.Requires(endPathEdges != null);

			EdgePredecessors = edgePredecessors;
			EndPathEdges = endPathEdges;
		}

		public IDictionary<TEdge, TEdge> EdgePredecessors { get; }

		public IList<TEdge> EndPathEdges { get; }

		public IDisposable Attach(IEdgePredecessorRecorderAlgorithm<TVertex, TEdge> algorithm)
		{
			algorithm.DiscoverTreeEdge += DiscoverTreeEdge;
			algorithm.FinishEdge += FinishEdge;

			return new DisposableAction(
				() =>
				{
					algorithm.DiscoverTreeEdge -= DiscoverTreeEdge;
					algorithm.FinishEdge -= FinishEdge;
				});
		}

		public ICollection<TEdge> Path(TEdge se)
		{
			List<TEdge> path = new List<TEdge>();

			var ec = se;
			path.Insert(0, ec);
			while (EdgePredecessors.ContainsKey(ec))
			{
				var e = EdgePredecessors[ec];
				path.Insert(0, e);
				ec = e;
			}

			return path;
		}

		public ICollection<ICollection<TEdge>> AllPaths()
		{
			IList<ICollection<TEdge>> es = new List<ICollection<TEdge>>();

			foreach (var e in EndPathEdges)
				es.Add(Path(e));

			return es;
		}

		public ICollection<TEdge> MergedPath(TEdge se, IDictionary<TEdge, GraphColor> colors)
		{
			List<TEdge> path = new List<TEdge>();

			var ec = se;
			var c = colors[ec];
			if (c != GraphColor.White)
				return path;
			colors[ec] = GraphColor.Black;

			path.Insert(0, ec);
			while (EdgePredecessors.ContainsKey(ec))
			{
				var e = EdgePredecessors[ec];
				c = colors[e];
				if (c != GraphColor.White)
					return path;
				colors[e] = GraphColor.Black;

				path.Insert(0, e);
				ec = e;
			}

			return path;
		}

		public ICollection<ICollection<TEdge>> AllMergedPaths()
		{
			List<ICollection<TEdge>> es = new List<ICollection<TEdge>>(EndPathEdges.Count);
			IDictionary<TEdge, GraphColor> colors = new Dictionary<TEdge, GraphColor>();

			foreach (var de in EdgePredecessors)
			{
				colors[de.Key] = GraphColor.White;
				colors[de.Value] = GraphColor.White;
			}

			for (var i = 0; i < EndPathEdges.Count; ++i)
				es.Add(MergedPath(EndPathEdges[i], colors));

			return es;
		}

		private void DiscoverTreeEdge(TEdge edge, TEdge targetEdge)
		{
			if (!edge.Equals(targetEdge))
				EdgePredecessors[targetEdge] = edge;
		}

		private void FinishEdge(TEdge args)
		{
			foreach (var edge in EdgePredecessors.Values)
				if (edge.Equals(args))
					return;

			EndPathEdges.Add(args);
		}
	}
}