using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics;
using Infokom.Numerics.Extensions;

using System.Buffers;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Gaming.Poker
{
	public readonly partial struct CardSpectrum : IEquatable<CardSpectrum>, IEnumerable<Card>, IReadOnlyCollection<Card>, IImmutableSet<Card>, IBitwiseOperators<CardSpectrum, CardSpectrum, CardSpectrum>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Encode(Card card) => (1ul << Card.IndexOf(card));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Encode(params ReadOnlySpan<Card> cards)
		{
			ulong mask = default;

			foreach (var card in cards)
			{
				mask |= Encode(card);
			}

			return mask;
		}

		private const ulong EMPTY = 0x0;
		private const ulong ALL = 0x000FFFFFFFFFFFFFul;


		private readonly ulong _id;//spectrum bitmask

		private CardSpectrum(ulong id)
		{
			this._id = id;
		}


		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => BitOperations.PopCount(_id);
		}

		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _id == 0;
		}



		public bool this[Card card]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (this & Create(card)).IsEmpty;
		}




		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CardSpectrum Include(Card card) => this & Create(card);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CardSpectrum Exclude(Card card) => this & ~Create(card);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Card item)
		{
			var a = _id;
			var b = Encode(item);

			return (a & b) != 0;
		}



		public Enumerator GetEnumerator() => new(_id);

		#region  IEquatable<CardSpectrum>
		public override int GetHashCode() => _id.GetHashCode();
		public bool Equals(CardSpectrum other) => _id.Equals(other._id);
		public override bool Equals(object obj) => obj is CardSpectrum other && this.Equals(other);
		public static bool operator ==(CardSpectrum left, CardSpectrum right) => left.Equals(right);
		public static bool operator !=(CardSpectrum left, CardSpectrum right) => !(left == right);

		#endregion

		#region IEnumerable<Card>
		IEnumerator<Card> IEnumerable<Card>.GetEnumerator() => this.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
		#endregion

		#region IReadOnlyCollection<Card>
		int IReadOnlyCollection<Card>.Count => this.Count;
		#endregion

		#region IImmutableSet<Card>
		IImmutableSet<Card> IImmutableSet<Card>.Clear() => CardSpectrum.Empty;
		bool IImmutableSet<Card>.Contains(Card value) => this.Contains(value);
		IImmutableSet<Card> IImmutableSet<Card>.Add(Card value) => this | CardSpectrum.Create(value);
		IImmutableSet<Card> IImmutableSet<Card>.Remove(Card value) => this & ~CardSpectrum.Create(value);
		bool IImmutableSet<Card>.TryGetValue(Card equalValue, out Card actualValue) => (actualValue = this.Contains(equalValue) ? equalValue : default) != default;
		IImmutableSet<Card> IImmutableSet<Card>.Intersect(IEnumerable<Card> other) => this & CardSpectrum.Create(other.ToArray());
		IImmutableSet<Card> IImmutableSet<Card>.Except(IEnumerable<Card> other) => this & ~CardSpectrum.Create(other.ToArray());
		IImmutableSet<Card> IImmutableSet<Card>.SymmetricExcept(IEnumerable<Card> other) => this ^ CardSpectrum.Create(other.ToArray());


		bool IImmutableSet<Card>.IsSubsetOf(IEnumerable<Card> other)
		{
			var a = this;
			var b = CardSpectrum.Create(other.ToArray());

			return (a & b) == a;
		}
		bool IImmutableSet<Card>.IsProperSubsetOf(IEnumerable<Card> other)
		{
			var a = this;
			var b = CardSpectrum.Create(other.Distinct().ToArray());

			return (a & b) == a && (a != b);
		}

		bool IImmutableSet<Card>.IsSupersetOf(IEnumerable<Card> other)
		{
			var a = this;
			var b = CardSpectrum.Create(other.Distinct().ToArray());

			return (a & b) == b;
		}
		bool IImmutableSet<Card>.IsProperSupersetOf(IEnumerable<Card> other)
		{
			var a = this;
			var b = CardSpectrum.Create(other.ToArray());

			return (a & b) == b && a != b;
		}


		bool IImmutableSet<Card>.Overlaps(IEnumerable<Card> other) => !(this & CardSpectrum.Create(other.ToArray())).IsEmpty;

		bool IImmutableSet<Card>.SetEquals(IEnumerable<Card> other) => this == CardSpectrum.Create(other.ToArray());
		IImmutableSet<Card> IImmutableSet<Card>.Union(IEnumerable<Card> other) => this | CardSpectrum.Create(other.ToArray());
		#endregion


		public static CardSpectrum Empty { get; } = new(EMPTY);

		public static readonly CardSpectrum All = new(ALL);







		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSpectrum Create(params ReadOnlySpan<Card> cards) => new(Encode(cards));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSpectrum operator &(CardSpectrum left, CardSpectrum right) => new(left._id & right._id);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSpectrum operator |(CardSpectrum left, CardSpectrum right) => new(left._id | right._id);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSpectrum operator ^(CardSpectrum left, CardSpectrum right) => new(left._id ^ right._id);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CardSpectrum operator ~(CardSpectrum value) => new(ALL & ~value._id);


		public static explicit operator CardSpectrum(ulong source) => new(source & ALL);
	}

}
