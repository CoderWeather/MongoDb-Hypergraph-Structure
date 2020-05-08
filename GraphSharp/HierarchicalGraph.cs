using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace GraphSharp
{
	public class HierarchicalGraph<TVertex, TEdge> :
		BidirectionalGraph<TVertex, TEdge>, IHierarchicalBidirectionalGraph<TVertex, TEdge>
		where TEdge : TypedEdge<TVertex>
	{
		#region Properties, fields

		private class TypedEdgeCollectionWrapper
		{
			public readonly List<TEdge> InGeneralEdges = new List<TEdge>();
			public readonly List<TEdge> InHierarchicalEdges = new List<TEdge>();
			public readonly List<TEdge> OutGeneralEdges = new List<TEdge>();
			public readonly List<TEdge> OutHierarchicalEdges = new List<TEdge>();
		}

		private readonly Dictionary<TVertex, TypedEdgeCollectionWrapper> typedEdgeCollections =
			new Dictionary<TVertex, TypedEdgeCollectionWrapper>();

		#endregion

		#region Constructors

		public HierarchicalGraph()
		{
		}

		public HierarchicalGraph(bool allowParallelEdges)
			: base(allowParallelEdges)
		{
		}

		public HierarchicalGraph(bool allowParallelEdges, int vertexCapacity)
			: base(allowParallelEdges, vertexCapacity)
		{
		}

		#endregion

		#region Add/Remove Vertex

		public override bool AddVertex(TVertex v)
		{
			base.AddVertex(v);
			if (!typedEdgeCollections.ContainsKey(v)) typedEdgeCollections[v] = new TypedEdgeCollectionWrapper();
			return true;
		}

		public override bool RemoveVertex(TVertex v)
		{
			var ret = base.RemoveVertex(v);
			if (ret)
			{
				//remove the edges from the typedEdgeCollections
				var edgeCollection = typedEdgeCollections[v];
				foreach (var e in edgeCollection.InGeneralEdges)
					typedEdgeCollections[e.Source].OutGeneralEdges.Remove(e);
				foreach (var e in edgeCollection.OutGeneralEdges)
					typedEdgeCollections[e.Target].InGeneralEdges.Remove(e);

				foreach (var e in edgeCollection.InHierarchicalEdges)
					typedEdgeCollections[e.Source].OutHierarchicalEdges.Remove(e);
				foreach (var e in edgeCollection.OutHierarchicalEdges)
					typedEdgeCollections[e.Target].InHierarchicalEdges.Remove(e);

				typedEdgeCollections.Remove(v);
				return true;
			}

			return false;
		}

		#endregion

		#region Add/Remove Edge

		public override bool AddEdge(TEdge e)
		{
			if (base.AddEdge(e))
			{
				//add edge to the source collections
				var sourceEdgeCollection = typedEdgeCollections[e.Source];
				switch (e.Type)
				{
					case EdgeTypes.General:
						sourceEdgeCollection.OutGeneralEdges.Add(e);
						break;
					case EdgeTypes.Hierarchical:
						sourceEdgeCollection.OutHierarchicalEdges.Add(e);
						break;
				}

				//add edge to the target collections
				var targetEdgeCollection = typedEdgeCollections[e.Target];
				switch (e.Type)
				{
					case EdgeTypes.General:
						targetEdgeCollection.InGeneralEdges.Add(e);
						break;
					case EdgeTypes.Hierarchical:
						targetEdgeCollection.InHierarchicalEdges.Add(e);
						break;
				}

				return true;
			}

			return false;
		}

		public override bool RemoveEdge(TEdge e)
		{
			if (base.RemoveEdge(e))
			{
				//remove edge from the source collections
				var sourceEdgeCollection = typedEdgeCollections[e.Source];
				switch (e.Type)
				{
					case EdgeTypes.General:
						sourceEdgeCollection.OutGeneralEdges.Remove(e);
						break;
					case EdgeTypes.Hierarchical:
						sourceEdgeCollection.OutHierarchicalEdges.Remove(e);
						break;
				}

				//remove edge from the target collections
				var targetEdgeCollection = typedEdgeCollections[e.Target];
				switch (e.Type)
				{
					case EdgeTypes.General:
						targetEdgeCollection.InGeneralEdges.Remove(e);
						break;
					case EdgeTypes.Hierarchical:
						targetEdgeCollection.InHierarchicalEdges.Remove(e);
						break;
				}

				return true;
			}

			return false;
		}

		#endregion

		#region Hierarchical Edges

		public IEnumerable<TEdge> HierarchicalEdgesFor(TVertex v)
		{
			var collections = typedEdgeCollections[v];
			return collections.InHierarchicalEdges.Concat(collections.OutHierarchicalEdges);
		}

		public int HierarchicalEdgeCountFor(TVertex v)
		{
			return typedEdgeCollections[v].InHierarchicalEdges.Count +
				typedEdgeCollections[v].OutHierarchicalEdges.Count;
		}

		public IEnumerable<TEdge> InHierarchicalEdges(TVertex v)
		{
			return typedEdgeCollections[v].InHierarchicalEdges;
		}

		public int InHierarchicalEdgeCount(TVertex v)
		{
			return typedEdgeCollections[v].InHierarchicalEdges.Count;
		}

		public IEnumerable<TEdge> OutHierarchicalEdges(TVertex v)
		{
			return typedEdgeCollections[v].OutHierarchicalEdges;
		}

		public int OutHierarchicalEdgeCount(TVertex v)
		{
			return typedEdgeCollections[v].OutHierarchicalEdges.Count;
		}

		#endregion

		#region General Edges

		public IEnumerable<TEdge> GeneralEdgesFor(TVertex v)
		{
			var collections = typedEdgeCollections[v];
			foreach (var e in collections.InGeneralEdges) yield return e;
			foreach (var e in collections.OutGeneralEdges) yield return e;
		}

		public int GeneralEdgeCountFor(TVertex v)
		{
			return typedEdgeCollections[v].InGeneralEdges.Count + typedEdgeCollections[v].OutGeneralEdges.Count;
		}

		public IEnumerable<TEdge> InGeneralEdges(TVertex v)
		{
			return typedEdgeCollections[v].InGeneralEdges;
		}

		public int InGeneralEdgeCount(TVertex v)
		{
			return typedEdgeCollections[v].InGeneralEdges.Count;
		}

		public IEnumerable<TEdge> OutGeneralEdges(TVertex v)
		{
			return typedEdgeCollections[v].OutGeneralEdges;
		}

		public int OutGeneralEdgeCount(TVertex v)
		{
			return typedEdgeCollections[v].OutGeneralEdges.Count;
		}

		#endregion

		#region IHierarchicalBidirectionalGraph<TVertex,TEdge> Members

		public IEnumerable<TEdge> HierarchicalEdges
		{
			get
			{
				foreach (var v in Vertices)
				foreach (var e in OutHierarchicalEdges(v))
					yield return e;
			}
		}

		public int HierarchicalEdgeCount
		{
			get { return Vertices.Sum(v => InHierarchicalEdgeCount(v)); }
		}

		public IEnumerable<TEdge> GeneralEdges
		{
			get
			{
				foreach (var v in Vertices)
				foreach (var e in OutGeneralEdges(v))
					yield return e;
			}
		}

		public int GeneralEdgeCount
		{
			get { return Vertices.Sum(v => InGeneralEdgeCount(v)); }
		}

		#endregion
	}
}