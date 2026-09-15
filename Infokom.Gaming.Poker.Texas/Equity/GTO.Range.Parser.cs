using Infokom.Numerics;
using Infokom.Numerics.Atomics;

using System.Diagnostics.CodeAnalysis;

using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		public readonly partial struct Range : IParsable<Range>
		{
			public static Range Parse(string s)
			{
				ArgumentNullException.ThrowIfNull(s);

				return TryParse(s.AsSpan(), out var result)
					? result
					: throw new FormatException($"Invalid range: '{s}'.");
			}

			public static Range Parse(ReadOnlySpan<char> source)
			{
				return TryParse(source, out var result)
					? result
					: throw new FormatException($"Invalid range: '{source.ToString()}'.");
			}

			public static bool TryParse([NotNullWhen(true)] string s, [MaybeNullWhen(false)] out Range result)
			{
				if (s is null)
				{
					result = default;
					return false;
				}

				return TryParse(s.AsSpan(), out result);
			}

			public static bool TryParse(ReadOnlySpan<char> source, out Range result)
			{
				source = source.Trim();

				if (source.IsEmpty)
				{
					result = default;
					return false;
				}

				Range aggregate = Φ;
				bool any = false;

				int i = 0;

				while (i < source.Length)
				{
					while (i < source.Length && IsSeparator(source[i]))
						i++;

					if (i >= source.Length)
						break;

					int start = i;

					while (i < source.Length && !IsSeparator(source[i]))
						i++;

					var token = source[start..i].Trim();

					if (token.IsEmpty)
						continue;

					Range item;

					if (token[^1] == '+')
					{
						if (!TryParsePlus(token[..^1], out item))
						{
							result = default;
							return false;
						}
					}
					else
					{
						if (!Cell.TryParse(token, out var cell))
						{
							result = default;
							return false;
						}

						item = Of(cell);
					}

					aggregate |= item;
					any = true;
				}

				result = any ? aggregate : default;
				return any;
			}

			static Range IParsable<Range>.Parse(string s, IFormatProvider provider) => Parse(s);
			static bool IParsable<Range>.TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out Range result) => TryParse(s, out result);

			private static bool TryParsePlus(ReadOnlySpan<char> source, out Range result)
			{
				if (!Cell.TryParse(source, out var cell))
				{
					result = default;
					return false;
				}

				result = ExpandPlus(cell);
				return true;
			}

			private static Range Of(Cell cell)
			{
				var data = new BitMatrix16x16();
				data[(int)cell.Y, (int)cell.X] = true;
				return new Range(data);
			}

			private static Range ExpandPlus(Cell cell)
			{
				var data = new BitMatrix16x16();

				if (cell.IsCoranked)
				{
					for (int r = (int)cell.Hi; r <= (int)Rank.Ace; r++)
						data[r, r] = true;

					return new Range(data);
				}

				int hi = (int)cell.Hi;
				int lo0 = (int)cell.Lo;

				for (int lo = lo0; lo < hi; lo++)
				{
					if (cell.IsCosuited)
						data[hi, lo] = true;
					else
						data[lo, hi] = true;
				}

				return new Range(data);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool IsSeparator(char c) => c is ',' or ';' or '.' || char.IsWhiteSpace(c);
		}
	}
}