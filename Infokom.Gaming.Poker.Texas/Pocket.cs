using Infokom.Numerics.Atomics;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Infokom.Gaming.Poker;

namespace Infokom.Gaming.Poker.Texas
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Pocket
	{
		private readonly Cards _data;

		private Pocket(Cards cards) => _data = cards;

		public Cards Cards
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _data;
		}


		public void Deconstruct(out Card x, out Card y)
		{
			//var up = CardSet.Select(_data.Ranks.Upmost());
			//var lo = CardSet.Select(_data.Ranks.Lowest());


			x = _data.Top();
			y = _data.Exclude(x).Top();

			(x, y) = (Card.Hi(x, y), Card.Lo(x, y));
		}

		public override string ToString() 
		{
			var (x, y) = this;
			if (x < Math.Max(x.R, y.R)) (x, y) = (y, x);

			return $"[{x.Symbol},{y.Symbol}]";
		}


		public static readonly Pocket Empty;

		internal static Pocket Create(Cards cards) => new(cards);



		public static implicit operator Pocket((Card X, Card Y) source) => new(Cards.Select(source.X, source.Y));

		public static implicit operator Cards(Pocket source) => source.Cards;
		
		private static readonly Regex REGEX = new(@"\[([2-9TJQKA][sdch]), ([2-9TJQKA][sdch])\]", RegexOptions.Compiled);

		public static Pocket Parse(string source)
		{
			var match = REGEX.Match(source);

			if (!match.Success)
				throw new FormatException();

			return (X: Card.Parse(match.Groups[1].Value), Y: Card.Parse(match.Groups[2].Value));
		}


		
	}
}
