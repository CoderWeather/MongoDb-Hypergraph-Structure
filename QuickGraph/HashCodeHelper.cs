using System.Runtime.CompilerServices;

namespace QuickGraph
{
	internal static class HashCodeHelper
	{
		private const int FNV1_prime_32 = 16777619;
		private const int FNV1_basis_32 = unchecked((int) 2166136261);
		private const long FNV1_prime_64 = 1099511628211;
		private const long FNV1_basis_64 = unchecked((int) 14695981039346656037);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetHashCode(long x)
		{
			return Combine((int) x, (int) ((ulong) x >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Fold(int hash, byte value)
		{
			return (hash * FNV1_prime_32) ^ value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Fold(int hash, int value)
		{
			return Fold(Fold(Fold(Fold(hash,
							(byte) value),
						(byte) ((uint) value >> 8)),
					(byte) ((uint) value >> 16)),
				(byte) ((uint) value >> 24));
		}

        /// <summary>
        ///     Combines two hashcodes in a strong way.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Combine(int x, int y)
		{
			return Fold(Fold(FNV1_basis_32, x), y);
		}

        /// <summary>
        ///     Combines three hashcodes in a strong way.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Combine(int x, int y, int z)
		{
			return Fold(Fold(Fold(FNV1_basis_32, x), y), z);
		}
	}
}