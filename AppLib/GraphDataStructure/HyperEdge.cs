using System.Collections.Generic;

namespace AppLib.GraphDataStructure
{
    public class HyperEdge
    {
        public int VerticesCounter { get; } = 0;
        public HashSet<Vertex> Vertices { get; } = new HashSet<Vertex>();
    }
}