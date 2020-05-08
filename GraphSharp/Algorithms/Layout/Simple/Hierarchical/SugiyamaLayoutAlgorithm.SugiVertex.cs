using System.Diagnostics;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Simple.Hierarchical
{
	public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		[DebuggerDisplay("{Original} [{LayerIndex}] Pos={Position} Meas={Measure} RealPos={RealPosition}")]
		private class SugiVertex : WrappedVertex<TVertex>
		{
			//Constants
			public const int UndefinedLayerIndex = -1;
			public const int UndefinedPosition = -1;

			//Private fields
			private int _layerIndex = UndefinedLayerIndex;

			/// <summary>
			///     The measure of the vertex (up/down-barycenter/median depends on the implementation).
			/// </summary>
			public double Measure;

			public int PermutationIndex;

			//Public fields
			/// <summary>
			///     The position of the vertex inside the layer.
			/// </summary>
			public int Position;

			/// <summary>
			///     The priority of the vertex. Used in the horizontal position assignment phase.
			///     The dummy vertices has maximal priorities (because the dummy edge should be as vertical as possible).
			///     The other vertices priority based on it's edge count.
			/// </summary>
			public int Priority;

			/// <summary>
			///     The real position (x and y coordinates) of the vertex.
			/// </summary>
			public Point RealPosition;

			/// <summary>
			///     Size of the vertex.
			/// </summary>
			public Size Size;

			/// <summary>
			///     Represents the subpriority of this vertex between the vertices with the same priority.
			/// </summary>
			public int SubPriority;

			/// <summary>
			///     Used in the algorithms for temporary storage.
			/// </summary>
			public double Temp;

			/// <summary>
			///     Constructor of the vertex.
			/// </summary>
			/// <param name="originalVertex">The object which is wrapped by this ComplexVertex.</param>
			/// <param name="size">The size of the original vertex.</param>
			public SugiVertex(TVertex originalVertex, Size size)
				: base(originalVertex)
			{
				Size = size;
			}

			/// <summary>
			///     The index of the layer where this vertex belongs to.
			/// </summary>
			public int LayerIndex
			{
				get => _layerIndex;
				set
				{
					if (_layerIndex != value)
					{
						//change the index
						_layerIndex = value;

						//add to the new layer
						if (_layerIndex == UndefinedLayerIndex)
							Position = UndefinedPosition;
					}
				}
			}

			/// <summary>
			///     Gets that this vertex is a dummy vertex (a point of a replaced long edge) or not.
			/// </summary>
			public bool IsDummyVertex => Original == null;

			public override string ToString()
			{
				return (Original == null ? "Dummy" : Original.ToString()) + " [" + LayerIndex + "]";
			}

			#region Maybe not needed

			public int LeftGeneralEdgeCount;
			public int RightGeneralEdgeCount;

			#endregion
		}
	}
}