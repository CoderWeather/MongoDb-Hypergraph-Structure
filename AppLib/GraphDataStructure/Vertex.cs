using System;
using System.Collections.Generic;

namespace AppLib.GraphDataStructure
{
    public class Vertex
    {
        public string Data { get; set; } = string.Empty;
        public HashSet<HyperEdge> Edges { get; } = new HashSet<HyperEdge>();
    }
}