using System.Linq;
using AppLib.GraphModels;
using DesktopApp.GraphControls;
using NUnit.Framework;

namespace UnitTests
{
	public class CircularAlgorithm
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void VertexPoints()
		{
			var vertices = new[]
			{
				new Vertex("v1"),
				new Vertex("v2"),
				new Vertex("v3"),
				new Vertex("v4"),
				new Vertex("v5")
			};
			var vertexControls = vertices.Select(v => new VertexControl
			{
				Vertex = v
			});
		}
	}
}