using Infokom.Gaming.Poker.Texas.Internal;
using Infokom.Numerics.Atomics;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace Infokom.Gaming.Poker.Texas
{

	[StructLayout(LayoutKind.Explicit)]
	public readonly partial struct Hand
	{
		[FieldOffset(0)] private readonly InlineArray4<ushort> _data;

		[FieldOffset(0)] public readonly CardSet Cards;
		[FieldOffset(0)] public readonly RankSet S;
		[FieldOffset(2)] public readonly RankSet D;
		[FieldOffset(4)] public readonly RankSet C;
		[FieldOffset(6)] public readonly RankSet H;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Hand(InlineArray4<ushort> data) => _data = data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Hand(CardSet data) => Cards = data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Hand(RankSet s, RankSet d, RankSet h, RankSet c) => (S, D, H, C) = (s, d, h, c);


		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Cards.Count;
		}


		public override string ToString() => this.Cards.ToString();






		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Hand Create(params ReadOnlySpan<Card> cards)
		{
			//TO DO: check cardinality

			return new(CardSet.Select(cards));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Hand Create(Pocket pocket, Board board)
		{
			//TO DO: check cardinality

			return new(pocket.Cards | board.Cards);
		}

		private const string PATTERN = @"\[(?<cards>([2-9TJQKA][sdch],[ ]*)*([2-9TJQKA][sdch]))\]";
		public static Hand Parse(string source)
		{
			ArgumentNullException.ThrowIfNull(source);

			if (!TryParse(source, out var target))
			{
				throw new FormatException($"The string '{source}' was not recognized as a valid Hand.");
			}

			return target;
		}



		public static bool TryParse(string source, out Hand target)
		{
			if (!string.IsNullOrWhiteSpace(source))
			{
				var match = Regex.Match(source, PATTERN);

				if (match.Success)
				{
					var cards = CardSet.Φ;

					foreach (var c in match.Groups["cards"].Value.Split(',', StringSplitOptions.TrimEntries))
					{
						cards = cards.Include(Rank.Cast(c[0], 0) * Suit.Cast(c[1], 0));
					}

					target = new(cards);
					return true;
				}
			}

			target = default;
			return false;
		}














































	}

	public readonly partial struct Hand : IReadOnlyList<Card>
	{
		Card IReadOnlyList<Card>.this[int index] => throw new NotImplementedException();

		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => throw new NotImplementedException();
		IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
	}
}