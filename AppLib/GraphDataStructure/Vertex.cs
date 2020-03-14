using System;
using System.Collections.Generic;

namespace AppLib.GraphDataStructure
{
    public class Vertex : IComparable<Vertex>
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Data { get; set; } = string.Empty;
        public HashSet<HyperEdge> Edges { get; } = new HashSet<HyperEdge>();

        public int CompareTo(Vertex other) =>
            string.Compare(Data, other.Data, StringComparison.Ordinal);

        private sealed class EqualityComparer : IEqualityComparer<Vertex>
        {
            public bool Equals(Vertex x, Vertex y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.GetType() == y.GetType() && (x.Id.Equals(y.Id) || x.Data == y.Data);
            }

            public int GetHashCode(Vertex obj) => HashCode.Combine(obj.Id, obj.Data);
        }

        public static IEqualityComparer<Vertex> Comparer { get; } = new EqualityComparer();
    }
}