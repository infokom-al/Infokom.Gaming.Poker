using Infokom.Numerics.Helpers;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{
	public static class MathExtensions
	{
		extension(Math)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T Choose<T>(T n, T k) where T : unmanaged, IBinaryInteger<T> => Func.Choose(n, k);





			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static long Permuations(long n, long k) => throw new NotImplementedException();
		}
	}
}