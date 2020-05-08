using System.Diagnostics;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Simple.Hierarchical
{
	public partial class EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		protected class SugiEdge : TaggedEdge<SugiVertex, TEdge>
		{
            /// <summary>
            ///     Gets or sets that the edge is included in a
            ///     type 1 conflict as a non-inner segment (true) or not (false).
            /// </summary>
            public bool Marked;

			public bool TempMark;

			public SugiEdge(TEdge originalEdge, SugiVertex source, SugiVertex target)
				: base(source, target, originalEdge)
			{
			}

            /// <summary>
            ///     Gets the original edge of this SugiEdge.
            /// </summary>
            public TEdge OriginalEdge => Tag;

			public void SaveMarkedToTemp()
			{
				TempMark = Marked;
			}

			public void LoadMarkedFromTemp()
			{
				Marked = TempMark;
			}
		}


		protected enum VertexTypes
		{
			Original,
			PVertex,
			QVertex,
			RVertex
		}

		protected enum EdgeTypes
		{
			NonInnerSegment,
			InnerSegment
		}

		protected interface IData
		{
			int Position { get; set; }
		}

		protected abstract class Data : IData
		{
			public readonly double[] Shifts = new double[4]
				{double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity};

			/* Used by horizontal assignment */
			public readonly Data[] Sinks = new Data[4];
			public int Position { get; set; }
		}

		protected abstract class SugiVertexBase : Data
		{
			public TVertex OriginalVertex;
			public Segment Segment;
			public VertexTypes Type;

			public SugiVertexBase()
			{
			}

			public SugiVertexBase(TVertex originalVertex)
			{
				OriginalVertex = originalVertex;
				Type = VertexTypes.Original;
				Segment = null;
			}

			public int LayerIndex { get; set; }
			public double MeasuredPosition { get; set; }
		}

		[DebuggerDisplay("{Type}: {OriginalVertex} - {Position} ; {MeasuredPosition} on layer {LayerIndex}")]
		protected class SugiVertex : SugiVertexBase
		{
			public readonly SugiVertex[] Aligns = new SugiVertex[4];
			public readonly double[] BlockWidths = new double[4] {double.NaN, double.NaN, double.NaN, double.NaN};

			public readonly double[] HorizontalPositions = new double[4]
				{double.NaN, double.NaN, double.NaN, double.NaN};

			public readonly SugiVertex[] Roots = new SugiVertex[4];
			public readonly Size Size;
			public bool DoNotOpt;
			public double HorizontalPosition = double.NaN;
			public int IndexInsideLayer;
			public int PermutationIndex;
			public int TempPosition;
			public double VerticalPosition = double.NaN;

			public SugiVertex()
			{
				Size = new Size();
			}

			public SugiVertex(TVertex originalVertex, Size size)
				: base(originalVertex)
			{
				Size = size;
			}

			public void SavePositionToTemp()
			{
				TempPosition = Position;
			}

			public void LoadPositionFromTemp()
			{
				Position = TempPosition;
			}
		}

		protected class Segment : Data
		{
            /// <summary>
            ///     Gets or sets the p-vertex of the segment.
            /// </summary>
            public SugiVertex PVertex;

            /// <summary>
            ///     Gets or sets the q-vertex of the segment.
            /// </summary>
            public SugiVertex QVertex;
		}
	}
}