using System.Collections.Immutable;
using System.Runtime.InteropServices;

using static System.Collections.Specialized.BitVector32;

namespace Infokom.Numerics.Extensions
{
	public static class EnumExtensions
	{
		extension<TEnum>(TEnum source) where TEnum : struct, Enum
		{
			/// <summary>
			/// Returns a <see cref="bool"/> telling whether a given value is declared in <typeparamref name="TEnum"/> enumeration.</summary>
			/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
			/// <param name="value">The value of <typeparamref name="TEnum"/> to be checked.</param>
			/// <returns><see langword="true"/> if the value of <paramref name="source"/> is declared in <typeparamref name="TEnum"/>; <see langword="false"/> otherwise.</returns>
			public bool IsKnown => Enum.IsDefined(source);

			public static TEnum[] GetValues() => Enum.GetValues<TEnum>();
		}
	}
}
