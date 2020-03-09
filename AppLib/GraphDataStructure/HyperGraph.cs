using System;
using System.Collections.Generic;

namespace AppLib.GraphDataStructure
{
    public class HyperGraph
    {
        #region Public Get-Only Properties
        
        public Guid Id { get; } = Guid.NewGuid();

        public List<HyperEdge> Edges { get; }
        public List<Vertex> Vertices { get; }

        #endregion

        #region Public Constructor

        public HyperGraph()
        {
            Edges = new List<HyperEdge>();
            Vertices = new List<Vertex>();
        }

        #endregion
    }
}