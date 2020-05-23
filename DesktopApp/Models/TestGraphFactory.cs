using HyperGraphSharp.Models;

namespace DesktopApp.Models
{
	public static class TestGraphFactory
	{
		public static HyperGraph GenerateTest1()
		{
			var graph = new HyperGraph();

			var v1 = new Vertex("1");
			var v2 = new Vertex("22");
			var v3 = new Vertex("333");
			var v4 = new Vertex("4444");
			var v5 = new Vertex("55555");
			var v6 = new Vertex("666666");
			var v7 = new Vertex("7777777");
			var v8 = new Vertex("88888888");
			var v9 = new Vertex("999999999");
			var v10 = new Vertex("1010101010");

			var e1 = new HyperEdge {Vertices = {v1, v2, v3}, Weight = 400};
			var e2 = new HyperEdge {Vertices = {v2, v3, v4}, Weight = 500};
			var e3 = new HyperEdge {Vertices = {v3, v4, v5}, Weight = 600};
			var e4 = new HyperEdge {Vertices = {v4, v5, v6}, Weight = 700};
			var e5 = new HyperEdge {Vertices = {v5, v6, v7}, Weight = 800};
			var e6 = new HyperEdge {Vertices = {v6, v7, v8}, Weight = 900};
			var e7 = new HyperEdge {Vertices = {v7, v8, v9}, Weight = 1000};
			var e8 = new HyperEdge {Vertices = {v8, v9, v10}, Weight = 50000};
			var e9 = new HyperEdge {Vertices = {v9, v10, v1}, Weight = 25000};
			var e10 = new HyperEdge {Vertices = {v10, v1, v2}, Weight = 1000000};

			v1.HyperEdges.Add(e1);
			v1.HyperEdges.Add(e2);
			v1.HyperEdges.Add(e3);

			v2.HyperEdges.Add(e2);
			v2.HyperEdges.Add(e3);
			v2.HyperEdges.Add(e4);

			v3.HyperEdges.Add(e3);
			v3.HyperEdges.Add(e4);
			v3.HyperEdges.Add(e5);

			v4.HyperEdges.Add(e4);
			v4.HyperEdges.Add(e5);
			v4.HyperEdges.Add(e6);

			v5.HyperEdges.Add(e5);
			v5.HyperEdges.Add(e6);
			v5.HyperEdges.Add(e7);

			v6.HyperEdges.Add(e6);
			v6.HyperEdges.Add(e7);
			v6.HyperEdges.Add(e8);

			v7.HyperEdges.Add(e7);
			v7.HyperEdges.Add(e8);
			v7.HyperEdges.Add(e9);

			v8.HyperEdges.Add(e8);
			v8.HyperEdges.Add(e9);
			v8.HyperEdges.Add(e10);

			v9.HyperEdges.Add(e9);
			v9.HyperEdges.Add(e10);
			v9.HyperEdges.Add(e1);

			v10.HyperEdges.Add(e10);
			v10.HyperEdges.Add(e1);
			v10.HyperEdges.Add(e2);

			graph.Vertices.AddRange(new[]
			{
				v1, v2, v3, v4, v5, v6, v7, v8, v9, v10
			});
			graph.HyperEdges.AddRange(new[]
			{
				// new HyperEdge {Vertices = {v1, v2, v3, v4, v5, v6, v7, v8, v9, v10}, Weight = 500},
				e1,
				e2,
				e3,
				e4,
				e5,
				e6,
				e7,
				e8,
				e9,
				e10,
			});

			return graph;
		}
	}
}