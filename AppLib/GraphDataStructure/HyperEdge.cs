using System;
using System.Collections.Generic;

namespace AppLib.GraphDataStructure
{
    public class HyperEdge
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int Weight { get; set; }
        public HashSet<Vertex> Vertices { get; } = new HashSet<Vertex>(Vertex.Comparer);

        private sealed class EqualityComparer : IEqualityComparer<HyperEdge>
        {
            public bool Equals(HyperEdge x, HyperEdge y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.GetType() == y.GetType()
                       && (x.Id.Equals(y.Id) || x.Vertices.SetEquals(y.Vertices));
            }

            public int GetHashCode(HyperEdge obj) => HashCode.Combine(obj.Id, obj.Vertices);
        }

        public static IEqualityComparer<HyperEdge> Comparer { get; } = new EqualityComparer();
    }
}