using System;
using System.Collections.Generic;

namespace AppLib
{
	public abstract class HyperGraphBase<TVertex, TEdge>
	{
		#region Public Constructor

		public HyperGraphBase()
		{
			Id = Guid.NewGuid();
			VerticesList = new List<TVertex>();
			EdgesList = new List<TEdge>();
		}

		#endregion

		#region Public Properties

		public Guid Id { get; }
		public List<TVertex> VerticesList { get; }
		public List<TEdge> EdgesList { get; }

		#endregion
	}
}