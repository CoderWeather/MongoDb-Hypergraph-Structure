using System;
using System.Collections.Generic;

namespace HyperGraphSharp.Models
{
	public class HyperGraph
	{
		#region Public Constructor

		public HyperGraph()
		{
			Id = Guid.NewGuid();
			Vertices = new List<Vertex>();
			HyperEdges = new List<HyperEdge>();
		}

		#endregion

		#region Public Properties

		public Guid Id { get; }
		public List<Vertex> Vertices { get; }
		public List<HyperEdge> HyperEdges { get; }

		#endregion
	}
}