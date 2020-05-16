using HyperGraphSharp.Models;

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
            var v6 = new Vertex("v6");
            var v7 = new Vertex("v7");
            var v8 = new Vertex("v8");
            var v9 = new Vertex("v9");
            var v10 = new Vertex("v10");

            var e1 = new HyperEdge {Vertices = {v1, v2, v3}};
            var e2 = new HyperEdge {Vertices = {v2, v3, v4}};
            var e3 = new HyperEdge {Vertices = {v3, v4, v5}};
            var e4 = new HyperEdge {Vertices = {v4, v5, v6}};
            var e5 = new HyperEdge {Vertices = {v5, v6, v7}};
            var e6 = new HyperEdge {Vertices = {v6, v7, v8}};
            var e7 = new HyperEdge {Vertices = {v7, v8, v9}};
            var e8 = new HyperEdge {Vertices = {v8, v9, v10}};
            var e9 = new HyperEdge {Vertices = {v9, v10, v1}};
            var e10 = new HyperEdge {Vertices = {v10, v1, v2}};

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
                new HyperEdge {Vertices = {v1, v2, v3, v4, v5, v6, v7, v8, v9, v10}},
                // e1, e2, e3, e4, e5, e6, e7, e8, e9, e10
            });

            return graph;
        }
    }
}