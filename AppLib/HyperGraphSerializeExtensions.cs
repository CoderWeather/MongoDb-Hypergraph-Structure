using System.IO;
using System.Threading.Tasks;
using HyperGraphSharp.Models;
using Utf8Json;

namespace AppLib
{
	public static class HyperGraphSerializeExtensions
	{
		public static byte[] Serialize(this HyperGraph graph) =>
			JsonSerializer.Serialize(graph);

		public static Task SerializeAsync(this HyperGraph graph, Stream stream) =>
			JsonSerializer.SerializeAsync(stream, graph);

		public static byte[] Serialize(this HyperEdge edge) =>
			JsonSerializer.Serialize(edge);

		public static Task SerializeAsync(this HyperEdge edge, Stream stream) =>
			JsonSerializer.SerializeAsync(stream, edge);
		
		public static byte[] Serialize(this Vertex vertex) =>
			JsonSerializer.Serialize(vertex);

		public static Task SerializeAsync(this Vertex vertex, Stream stream) =>
			JsonSerializer.SerializeAsync(stream, vertex);
	}
}