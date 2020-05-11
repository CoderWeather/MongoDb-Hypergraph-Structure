using System.Collections.Generic;
using System.Linq;
using GraphSharp.Algorithms.Highlight;
using QuickGraph;

namespace GraphSharp.Controls
{
	public partial class GraphLayout<TVertex, TEdge, TGraph> :
		IHighlightController<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		private void SetHighlightProperties(TVertex vertex, VertexControl presenter)
		{
			if (IsHighlightedVertex(vertex, out var highlightInfo))
			{
				GraphElementBehaviour.SetIsHighlighted(presenter, true);
				GraphElementBehaviour.SetHighlightInfo(presenter, highlightInfo);
			}

			if (IsSemiHighlightedVertex(vertex, out var semiHighlightInfo))
			{
				GraphElementBehaviour.SetIsSemiHighlighted(presenter, true);
				GraphElementBehaviour.SetSemiHighlightInfo(presenter, semiHighlightInfo);
			}
		}

		private void SetHighlightProperties(TEdge edge, EdgeControl edgeControl)
		{
			if (IsHighlightedEdge(edge, out var highlightInfo))
			{
				GraphElementBehaviour.SetIsHighlighted(edgeControl, true);
				GraphElementBehaviour.SetHighlightInfo(edgeControl, highlightInfo);
			}

			if (IsSemiHighlightedEdge(edge, out var semiHighlightInfo))
			{
				GraphElementBehaviour.SetIsSemiHighlighted(edgeControl, true);
				GraphElementBehaviour.SetSemiHighlightInfo(edgeControl, semiHighlightInfo);
			}
		}

		#region IHighlightController<TVertex,TEdge,TGraph> Members

		private readonly IDictionary<TVertex, object> _highlightedVertices = new Dictionary<TVertex, object>();
		private readonly IDictionary<TVertex, object> _semiHighlightedVertices = new Dictionary<TVertex, object>();
		private readonly IDictionary<TEdge, object> _highlightedEdges = new Dictionary<TEdge, object>();
		private readonly IDictionary<TEdge, object> _semiHighlightedEdges = new Dictionary<TEdge, object>();

		public IEnumerable<TVertex> HighlightedVertices => _highlightedVertices.Keys.ToArray();

		public IEnumerable<TVertex> SemiHighlightedVertices => _semiHighlightedVertices.Keys.ToArray();

		public IEnumerable<TEdge> HighlightedEdges => _highlightedEdges.Keys.ToArray();

		public IEnumerable<TEdge> SemiHighlightedEdges => _semiHighlightedEdges.Keys.ToArray();

		public bool IsHighlightedVertex(TVertex vertex)
		{
			return _highlightedVertices.ContainsKey(vertex);
		}

		public bool IsHighlightedVertex(TVertex vertex, out object highlightInfo)
		{
			return _highlightedVertices.TryGetValue(vertex, out highlightInfo);
		}

		public bool IsSemiHighlightedVertex(TVertex vertex)
		{
			return _semiHighlightedVertices.ContainsKey(vertex);
		}

		public bool IsSemiHighlightedVertex(TVertex vertex, out object semiHighlightInfo)
		{
			return _semiHighlightedVertices.TryGetValue(vertex, out semiHighlightInfo);
		}

		public bool IsHighlightedEdge(TEdge edge)
		{
			return _highlightedEdges.ContainsKey(edge);
		}

		public bool IsHighlightedEdge(TEdge edge, out object highlightInfo)
		{
			return _highlightedEdges.TryGetValue(edge, out highlightInfo);
		}

		public bool IsSemiHighlightedEdge(TEdge edge)
		{
			return _semiHighlightedEdges.ContainsKey(edge);
		}

		public bool IsSemiHighlightedEdge(TEdge edge, out object semiHighlightInfo)
		{
			return _semiHighlightedEdges.TryGetValue(edge, out semiHighlightInfo);
		}

		public void HighlightVertex(TVertex vertex, object highlightInfo)
		{
			_highlightedVertices[vertex] = highlightInfo;
			if (VertexControls.TryGetValue(vertex, out var vc))
			{
				GraphElementBehaviour.SetIsHighlighted(vc, true);
				GraphElementBehaviour.SetHighlightInfo(vc, highlightInfo);
			}
		}

		public void SemiHighlightVertex(TVertex vertex, object semiHighlightInfo)
		{
			_semiHighlightedVertices[vertex] = semiHighlightInfo;
			if (VertexControls.TryGetValue(vertex, out var vc))
			{
				GraphElementBehaviour.SetIsSemiHighlighted(vc, true);
				GraphElementBehaviour.SetSemiHighlightInfo(vc, semiHighlightInfo);
			}
		}

		public void HighlightEdge(TEdge edge, object highlightInfo)
		{
			_highlightedEdges[edge] = highlightInfo;
			if (EdgeControls.TryGetValue(edge, out var ec))
			{
				GraphElementBehaviour.SetIsHighlighted(ec, true);
				GraphElementBehaviour.SetHighlightInfo(ec, highlightInfo);
			}
		}

		public void SemiHighlightEdge(TEdge edge, object semiHighlightInfo)
		{
			_semiHighlightedEdges[edge] = semiHighlightInfo;
			if (EdgeControls.TryGetValue(edge, out var ec))
			{
				GraphElementBehaviour.SetIsSemiHighlighted(ec, true);
				GraphElementBehaviour.SetSemiHighlightInfo(ec, semiHighlightInfo);
			}
		}

		public void RemoveHighlightFromVertex(TVertex vertex)
		{
			_highlightedVertices.Remove(vertex);
			if (VertexControls.TryGetValue(vertex, out var vc))
			{
				GraphElementBehaviour.SetIsHighlighted(vc, false);
				GraphElementBehaviour.SetHighlightInfo(vc, null);
			}
		}

		public void RemoveSemiHighlightFromVertex(TVertex vertex)
		{
			_semiHighlightedVertices.Remove(vertex);
			if (VertexControls.TryGetValue(vertex, out var vc))
			{
				GraphElementBehaviour.SetIsSemiHighlighted(vc, false);
				GraphElementBehaviour.SetSemiHighlightInfo(vc, null);
			}
		}

		public void RemoveHighlightFromEdge(TEdge edge)
		{
			_highlightedEdges.Remove(edge);
			if (EdgeControls.TryGetValue(edge, out var ec))
			{
				GraphElementBehaviour.SetIsHighlighted(ec, false);
				GraphElementBehaviour.SetHighlightInfo(ec, null);
			}
		}

		public void RemoveSemiHighlightFromEdge(TEdge edge)
		{
			_semiHighlightedEdges.Remove(edge);
			if (EdgeControls.TryGetValue(edge, out var ec))
			{
				GraphElementBehaviour.SetIsSemiHighlighted(ec, false);
				GraphElementBehaviour.SetSemiHighlightInfo(ec, null);
			}
		}

		#endregion
	}
}