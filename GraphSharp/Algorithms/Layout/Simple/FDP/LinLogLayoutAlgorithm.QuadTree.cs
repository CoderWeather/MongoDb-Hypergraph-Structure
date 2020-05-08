using System;
using System.Windows;
using QuickGraph;

namespace GraphSharp.Algorithms.Layout.Simple.FDP
{
	public partial class LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
		private class QuadTree
		{
			protected const int maxDepth = 20;

			public QuadTree(int index, Point position, double weight, Point minPos, Point maxPos)
			{
				Index = index;
				this.position = position;
				Weight = weight;
				this.minPos = minPos;
				this.maxPos = maxPos;
			}

			public double Width => Math.Max(maxPos.X - minPos.X, maxPos.Y - minPos.Y);

			public void AddNode(int nodeIndex, Point nodePos, double nodeWeight, int depth)
			{
				if (depth > maxDepth)
					return;

				if (Index >= 0)
				{
					AddNode2(Index, position, Weight, depth);
					Index = -1;
				}

				position.X = (position.X * Weight + nodePos.X * nodeWeight) / (Weight + nodeWeight);
				position.Y = (position.Y * Weight + nodePos.Y * nodeWeight) / (Weight + nodeWeight);
				Weight += nodeWeight;

				AddNode2(nodeIndex, nodePos, nodeWeight, depth);
			}

			protected void AddNode2(int nodeIndex, Point nodePos, double nodeWeight, int depth)
			{
				//Debug.WriteLine( string.Format( "AddNode2 {0} {1} {2} {3}", nodeIndex, nodePos, nodeWeight, depth ) );
				var childIndex = 0;
				var middleX = (minPos.X + maxPos.X) / 2;
				var middleY = (minPos.Y + maxPos.Y) / 2;

				if (nodePos.X > middleX)
					childIndex += 1;

				if (nodePos.Y > middleY)
					childIndex += 2;

				//Debug.WriteLine( string.Format( "childIndex: {0}", childIndex ) );               


				if (Children[childIndex] == null)
				{
					var newMin = new Point();
					var newMax = new Point();
					if (nodePos.X <= middleX)
					{
						newMin.X = minPos.X;
						newMax.X = middleX;
					}
					else
					{
						newMin.X = middleX;
						newMax.X = maxPos.X;
					}

					if (nodePos.Y <= middleY)
					{
						newMin.Y = minPos.Y;
						newMax.Y = middleY;
					}
					else
					{
						newMin.Y = middleY;
						newMax.Y = maxPos.Y;
					}

					Children[childIndex] = new QuadTree(nodeIndex, nodePos, nodeWeight, newMin, newMax);
				}
				else
				{
					Children[childIndex].AddNode(nodeIndex, nodePos, nodeWeight, depth + 1);
				}
			}

			/// <summary>
			///     Az adott rész pozícióját újraszámítja, levonva belőle a mozgatott node részét.
			/// </summary>
			/// <param name="oldPos"></param>
			/// <param name="newPos"></param>
			/// <param name="nodeWeight"></param>
			public void MoveNode(Point oldPos, Point newPos, double nodeWeight)
			{
				position += (newPos - oldPos) * (nodeWeight / Weight);

				var childIndex = 0;
				var middleX = (minPos.X + maxPos.X) / 2;
				var middleY = (minPos.Y + maxPos.Y) / 2;

				if (oldPos.X > middleX)
					childIndex += 1;
				if (oldPos.Y > middleY)
					childIndex += 1 << 1;

				if (Children[childIndex] != null)
					Children[childIndex].MoveNode(oldPos, newPos, nodeWeight);
			}

			#region Properties

			public QuadTree[] Children { get; } = new QuadTree[4];

			public int Index { get; private set; }

			private Point position;

			public Point Position => position;

			public double Weight { get; private set; }

			private Point minPos;
			private Point maxPos;

			#endregion
		}
	}
}