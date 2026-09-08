using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Infokom.Numerics.Intrinsics
{
	public static class Vector64Extensions
	{
		extension(ulong source)
		{
			public Vector64<ulong> ToVector64() => Vector64.Create(source);
			public Vector64<T> ToVector64<T>() => Vector64.Create(source).As<ulong, T>();
		}
		


	}
}
