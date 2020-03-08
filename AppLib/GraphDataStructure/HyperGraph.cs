using System.Collections.Generic;

namespace AppLib.GraphDataStructure
{
    public class HyperGraph
    {
        #region Public Properties

        public LinkedList<HyperEdge> Edges { get; }
        public LinkedList<Vertex> Vertices { get; }

        #endregion

        #region Public Constructor

        public HyperGraph()
        {
            Edges = new LinkedList<HyperEdge>();
            Vertices = new LinkedList<Vertex>();
        }

        #endregion

        public void AddVertex(Vertex vertex)
        {
            
        }

        public void AddEdge(HyperEdge hyperEdge)
        {
        }
    }
}