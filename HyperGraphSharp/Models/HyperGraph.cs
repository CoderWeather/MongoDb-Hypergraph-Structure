using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;

namespace HyperGraphSharp.Models
{
	public class HyperGraph
	{
		#region Public Properties

		[IgnoreDataMember] public Guid Id { get; }
		public string Caption { get; }
		public List<Vertex> Vertices { get; }
		public List<HyperEdge> HyperEdges { get; }

		#endregion

		#region Public Constructor

		public HyperGraph(string caption)
		{
			Id = Guid.NewGuid();
			Caption = caption;
			Vertices = new List<Vertex>();
			HyperEdges = new List<HyperEdge>();
		}

		[SerializationConstructor]
		public HyperGraph(string caption, List<Vertex> vertices, List<HyperEdge> hyperEdges)
		{
			Id = Guid.NewGuid();
			Caption = caption;
			Vertices = vertices;
			HyperEdges = hyperEdges;

			foreach (var edge in HyperEdges)
			{
				var findVertexList = Vertices
				   .Where(v => edge.Vertices
					   .Any(vIn => vIn.Data == v.Data))
				   .ToList();
				edge.Vertices.Clear();
				findVertexList.ForEach(vertex =>
				{
					vertex.HyperEdges.Add(edge);
					edge.Vertices.Add(vertex);
				});
			}
		}

		#endregion
	}
}