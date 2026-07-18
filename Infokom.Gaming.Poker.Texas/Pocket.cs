using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;

namespace Infokom.Gaming.Poker.Texas
{
	public interface IPlayer<TPocket>
	{
		
	}



	
	public readonly record struct Pocket
	{
		private readonly Cards _cards;
			
		private Pocket(Cards cards) => _cards = cards;

		public Card this[int index] => index switch { 0 => _cards.First, 1 => _cards.Last, _ => throw new IndexOutOfRangeException() };

		public bool IsPaired => this.Ranks.Count == 1;

		public bool IsSuited => this.Suits.Count == 1;

		public ulong ID => (ulong)_cards;


		public Ranks Ranks => _cards.Ranks;

		public Suits Suits => _cards.Suits;

		public Cards Cards => _cards;



		public void Deconstruct(out Card hi, out Card lo)
		{
			hi = _cards.First;
			lo = _cards.Last;


			var d = hi.Rank.Index.CompareTo(lo.Rank.Index);

			if(d == 0)
				d = hi.Suit.Index.CompareTo(lo.Suit.Index);

			(hi, lo) = d > 0 ? (hi, lo) : (lo, hi);
		}



		public bool Contains(Card element) => element.IsKnown && this._cards.IsIncluded(element);

		public bool Overlaps(Pocket other) => !(this._cards & other._cards).IsEmpty;


		public override string ToString()
		{
			var (c1, c2) = this;
			

			return $"[{c1.Symbol}, {c2.Symbol}]";
		}

		public static Pocket Create(Card x, Card y)
		{
			
			return new Pocket(Cards.Select(x, y));
		}






		private const string PATTERN = @"^\[(?<pocket>((?<card>[23456789TJQKA][shdc])[, ]*){2})\]$";
		public static Pocket Parse(string text)
		{
			var match = Regex.Match(text, PATTERN);
			if (!match.Success)
			{
				throw new ArgumentException($"Invalid text format '{text}'", nameof(text));
			}

			var cards = match.Groups["card"].Captures.Select(c => Card.Parse(c.Value)).ToArray();

			return Pocket.Create(cards[0], cards[1]);
		}
	}

	
}
