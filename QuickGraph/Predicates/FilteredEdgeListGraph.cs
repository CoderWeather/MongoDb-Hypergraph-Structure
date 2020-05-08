using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
	[Serializable]
	public sealed class FilteredEdgeListGraph<TVertex, TEdge, TGraph>
		: FilteredImplicitVertexSet<TVertex, TEdge, TGraph>
			, IEdgeListGraph<TVertex, TEdge>
		where TGraph : IEdgeListGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		public FilteredEdgeListGraph(
			TGraph baseGraph,
			VertexPredicate<TVertex> vertexPredicate,
			EdgePredicate<TVertex, TEdge> edgePredicate
		)
			: base(baseGraph, vertexPredicate, edgePredicate)
		{
		}

		public bool IsVerticesEmpty
		{
			get
			{
				foreach (var v in BaseGraph.Vertices)
					if (VertexPredicate(v))
						return false;
				return true;
			}
		}

		public int VertexCount
		{
			get
			{
				var count = 0;
				foreach (var v in BaseGraph.Vertices)
					if (VertexPredicate(v))
						count++;
				return count;
			}
		}

		public IEnumerable<TVertex> Vertices
		{
			get
			{
				foreach (var v in BaseGraph.Vertices)
					if (VertexPredicate(v))
						yield return v;
			}
		}

		public bool IsEdgesEmpty
		{
			get
			{
				foreach (var edge in BaseGraph.Edges)
					if (FilterEdge(edge))
						return false;
				return true;
			}
		}

		public int EdgeCount
		{
			get
			{
				var count = 0;
				foreach (var edge in BaseGraph.Edges)
					if (FilterEdge(edge))
						count++;
				return count;
			}
		}

		public IEnumerable<TEdge> Edges
		{
			get
			{
				foreach (var edge in BaseGraph.Edges)
					if (FilterEdge(edge))
						yield return edge;
			}
		}

		[Pure]
		public bool ContainsEdge(TEdge edge)
		{
			return
				FilterEdge(edge) &&
				BaseGraph.ContainsEdge(edge);
		}

		[Pure]
		private bool FilterEdge(TEdge edge)
		{
			return VertexPredicate(edge.Source)
			 && VertexPredicate(edge.Target)
			 && EdgePredicate(edge);
		}
	}
}