﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace GraphSharp.Algorithms.Layout
{
	[DebuggerDisplay("First = {First}, Second = {Second}")]
	public class Pair
	{
		public int First;
		public int Second;
		public int Weight = 1;
	}

	public static class LayoutUtil
	{
		public static int BiLayerCrossCount(IEnumerable<Pair> pairs, int firstLayerVertexCount,
			int secondLayerVertexCount)
		{
			if (pairs == null)
				return 0;

			//radix sort of the pair, order by First asc, Second asc

			#region Sort by Second ASC

			var radixBySecond = new List<Pair>[secondLayerVertexCount];
			List<Pair> r;
			var pairCount = 0;
			foreach (var pair in pairs)
			{
				//get the radix where the pair should be inserted
				r = radixBySecond[pair.Second];
				if (r == null)
				{
					r = new List<Pair>();
					radixBySecond[pair.Second] = r;
				}

				r.Add(pair);
				pairCount++;
			}

			#endregion

			#region Sort By First ASC

			var radixByFirst = new List<Pair>[firstLayerVertexCount];
			foreach (var list in radixBySecond)
			{
				if (list == null)
					continue;

				foreach (var pair in list)
				{
					//get the radix where the pair should be inserted
					r = radixByFirst[pair.First];
					if (r == null)
					{
						r = new List<Pair>();
						radixByFirst[pair.First] = r;
					}

					r.Add(pair);
				}
			}

			#endregion

			//
			// Build the accumulator tree
			//
			var firstIndex = 1;
			while (firstIndex < pairCount)
				firstIndex *= 2;
			var treeSize = 2 * firstIndex - 1;
			firstIndex -= 1;
			var tree = new int[treeSize];

			//
			// Count the crossings
			//
			var crossCount = 0;
			foreach (var list in radixByFirst)
			{
				if (list == null)
					continue;

				foreach (var pair in list)
				{
					var index = pair.Second + firstIndex;
					tree[index] += pair.Weight;
					while (index > 0)
					{
						if (index % 2 > 0)
							crossCount += tree[index + 1] * pair.Weight;
						index = (index - 1) / 2;
						tree[index] += pair.Weight;
					}
				}
			}

			return crossCount;
		}

		public static Point GetClippingPoint(Size size, Point s, Point t)
		{
			var sides = new double[4];
			sides[0] = (s.X - size.Width / 2.0 - t.X) / (s.X - t.X);
			sides[1] = (s.Y - size.Height / 2.0 - t.Y) / (s.Y - t.Y);
			sides[2] = (s.X + size.Width / 2.0 - t.X) / (s.X - t.X);
			sides[3] = (s.Y + size.Height / 2.0 - t.Y) / (s.Y - t.Y);

			double fi = 0;
			for (var i = 0; i < 4; i++)
				if (sides[i] <= 1)
					fi = Math.Max(fi, sides[i]);
			if (!(Math.Abs(fi) < 0.0005)) return t + fi * (s - t);
			{
				fi = double.PositiveInfinity;
				for (var i = 0; i < 4; i++)
					fi = Math.Min(fi, Math.Abs(sides[i]));
				fi *= -1;
			}

			return t + fi * (s - t);
		}


		public static bool IsSameDirection(Vector a, Vector b)
		{
			return Math.Sign(a.X) == Math.Sign(b.X) && Math.Sign(a.Y) == Math.Sign(b.Y);
		}

		public static int BiLayerCrossCount(List<Pair> edgePairs)
		{
			var firsts = edgePairs.Select(e => e.First).Distinct().OrderBy(f => f).ToArray();
			var seconds = edgePairs.Select(e => e.Second).Distinct().OrderBy(f => f).ToArray();
			var firstMap = new Dictionary<int, int>(firsts.Length);
			var secondMap = new Dictionary<int, int>(seconds.Length);
			for (var i = 0; i < firsts.Length; i++) firstMap.Add(firsts[i], i);
			for (var i = 0; i < seconds.Length; i++) secondMap.Add(seconds[i], i);
			foreach (var pair in edgePairs)
			{
				pair.First = firstMap[pair.First];
				pair.Second = secondMap[pair.Second];
			}

			return BiLayerCrossCount(edgePairs, firsts.Length, seconds.Length);
		}
	}
}