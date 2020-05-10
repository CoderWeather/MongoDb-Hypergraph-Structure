using AppLib.GraphModels;

namespace DesktopApp.Models
{
	public static class TestGraphFactory
	{
		public static HyperGraph GenerateTest1()
		{
			var graph = new HyperGraph();

			var v1 = new Vertex("v1");
			var v2 = new Vertex("v2");
			var v3 = new Vertex("v3");
			var v4 = new Vertex("v4");
			var v5 = new Vertex("v5");

			var e1 = new HyperEdge {Vertices = {v1, v2, v3}, Weight = 3};
			var e2 = new HyperEdge {Vertices = {v2, v3, v4}, Weight = 3};
			var e3 = new HyperEdge {Vertices = {v3, v4, v5}, Weight = 3};
			var e4 = new HyperEdge {Vertices = {v4, v5, v1}, Weight = 3};
			var e5 = new HyperEdge {Vertices = {v5, v1, v2}, Weight = 3};

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
			v4.HyperEdges.Add(e1);

			v5.HyperEdges.Add(e5);
			v5.HyperEdges.Add(e1);
			v5.HyperEdges.Add(e2);

			graph.Vertices.AddRange(new[] {v1, v2, v3, v4, v5});
			graph.HyperEdges.AddRange(new[] {e1, e2, e3, e4, e5});

			return graph;
		}
	}
}