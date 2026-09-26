using Infokom.Numerics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{







		public readonly partial struct Cell : IParsable<Cell>
		{
			public static Cell Parse(string source)
			{
				ArgumentNullException.ThrowIfNull(source);

				return TryParse(source.AsSpan(), out var result)
					? result
					: throw new FormatException($"Invalid cell: '{source}'.");
			}

			public static Cell Parse(ReadOnlySpan<char> source)
			{
				return TryParse(source, out var result)
					? result
					: throw new FormatException($"Invalid cell: '{source.ToString()}'.");
			}

			public static bool TryParse([NotNullWhen(true)] string source, [MaybeNullWhen(false)] out Cell result)
			{
				if (source is null)
				{
					result = default;
					return false;
				}

				return TryParse(source.AsSpan(), out result);
			}

			public static bool TryParse(ReadOnlySpan<char> source, out Cell result)
			{
				source = source.Trim();

				if (source.Length is not (2 or 3))
				{
					result = default;
					return false;
				}

				if (!TryParseRank(source[0], out var r1) || !TryParseRank(source[1], out var r2))
				{
					result = default;
					return false;
				}

				if (source.Length == 2)
				{
					if (r1 != r2)
					{
						result = default;
						return false;
					}

					result = new Cell(r1, r1);
					return true;
				}

				if (r1 == r2)
				{
					result = default;
					return false;
				}

				Rank hi = Rank.Max(r1, r2);
				Rank lo = Rank.Min(r1, r2);

				switch (NormalizeSuitness(source[2]))
				{
					case 's':
						result = new Cell(lo, hi);
						return true;

					case 'o':
						result = new Cell(hi, lo);
						return true;

					default:
						result = default;
						return false;
				}
			}

			static Cell IParsable<Cell>.Parse(string s, IFormatProvider provider) => Parse(s);
			static bool IParsable<Cell>.TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out Cell result) => TryParse(s, out result);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool TryParseRank(char symbol, out Rank rank)
			{
				symbol = char.ToUpperInvariant(symbol);
				return Rank.TryCast(symbol, out rank);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static char NormalizeSuitness(char symbol) => symbol switch
			{
				's' or 'S' or 'ₛ' => 's',
				'o' or 'O' or 'ₒ' => 'o',
				_ => '\0'
			};
		}
	}
}