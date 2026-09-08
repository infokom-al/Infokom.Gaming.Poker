using Infokom.Numerics.Atomics;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Infokom.Gaming.Poker.Texas
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Board
	{
		private readonly Cards _data;

		private Board(Cards data) => _data = data;

		public Cards Cards
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _data;
		}




		public override string ToString()
		{
			var buffer = new string[_data.Count];

			int i = 0;
			foreach (var card in _data)
			{
				buffer[i++] = card.Symbol;
			}
			return "[" + string.Join(", ", buffer) + "]";
		}

		public static readonly Board Empty;

		internal static Board Create(Cards cards) => new(cards);








		[InlineArray(5)]
		public struct RiverStreet
		{
			private Card _0;

			public readonly Board GetBoard() => new(Cards.Select(_0));
		}

		public static Board.PreFlopStreet PreFlop => default;

		[InlineArray(4)]
		public struct TurnStreet
		{
			private Card _0;

			public readonly Board GetBoard() => new(Cards.Select(this[0], this[1], this[2], this[3]));


			public readonly RiverStreet Next(Card river) 
			{
				var result = new RiverStreet();

				result[0] = this[0];
				result[1] = this[1];
				result[2] = this[2];
				result[3] = this[3];
				result[4] = river;

				return result;
			}
		}

		[InlineArray(3)]
		public struct FlopStreet
		{
			private Card _0;

			public readonly Board GetBoard() => new(Cards.Select(this[0], this[1], this[2]));


			public readonly TurnStreet Next(Card turn)
			{
				var result = new TurnStreet();

				result[0] = this[0];
				result[1] = this[1];
				result[2] = this[2];
				result[3] = turn;

				return result;
			}
		}

		public struct PreFlopStreet
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
			public readonly FlopStreet Next(Card element1, Card element2, Card element3)
			{
				var result = new FlopStreet();

				result[0] = element1;
				result[1] = element2;
				result[2] = element3;

				return result;
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
			public readonly Board GetBoard() => Board.Empty;
		}

		public static readonly PreFlopStreet Preflop;



		private static readonly Regex REGEX = new(@"\A\[(?<card>[2-9TJQKA][sdch])(?:, (?<card>[2-9TJQKA][sdch])){2,4}\]\z", RegexOptions.Compiled);

		public static Board Parse(string source)
		{
			var match = REGEX.Match(source);

			if (!match.Success)
				throw new FormatException();

			Cards cards = 0;

			var cardGroup = match.Groups["card"];
			for (int i = 0; i < cardGroup.Captures.Count; i++)
			{
				cards = cards.Include(Card.Parse(cardGroup.Captures[i].Value));
			}

			return new Board(cards);
		}
	}
}
