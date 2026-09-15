using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker
{
	[StructLayout(LayoutKind.Sequential, Size = 2)]
	public readonly partial struct RankSet : IEquatable<RankSet>, IEqualityOperators<RankSet, RankSet, bool>, IEquatable<ushort>, IEqualityOperators<RankSet, ushort, bool>
	{
		#region constants
		private const ushort _Φ = 0b0000000000000000;
		private const ushort _Ω = 0b0111111111111100;
		private const ushort _2 = 0b0000000000000100;
		private const ushort _3 = 0b0000000000001000;
		private const ushort _4 = 0b0000000000010000;
		private const ushort _5 = 0b0000000000100000;
		private const ushort _6 = 0b0000000001000000;
		private const ushort _7 = 0b0000000010000000;
		private const ushort _8 = 0b0000000100000000;
		private const ushort _9 = 0b0000001000000000;
		private const ushort _T = 0b0000010000000000;
		private const ushort _J = 0b0000100000000000;
		private const ushort _Q = 0b0001000000000000;
		private const ushort _K = 0b0010000000000000;
		private const ushort _A = 0b0100000000000000;
		#endregion

		private readonly ushort _bits;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private RankSet(ushort bits) => _bits = bits;

		/// <summary>
		/// <c><see langword="this"/> == <see cref="Φ">Φ</see></c> ⟺ <c>|<see langword="this"/>| == 0</c>
		/// </summary>
		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _bits == _Φ;
		}

		/// <summary>
		/// <c>|<see langword="this"/>|</c>
		/// </summary>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => BitOperations.PopCount(_bits);
		}

		/// <summary>
		/// Extend the <paramref name="source"/> including an element
		/// </summary>
		/// <param name="element">the element to add</param>
		/// <returns>
		/// <see langword="this"/> ∪ { <paramref name="element"/> }
		/// </returns>
		public RankSet Include(Rank element) => new((ushort)(_bits | (1u << element)));

		/// <summary>
		/// Restrict the <paramref name="source"/> set excluding an element
		/// </summary>
		/// <param name="element">the element to exclude</param>
		/// <returns>
		/// <see langword="this"/> \ { <paramref name="element"/> }
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RankSet Exclude(Rank element) => new((ushort)(_bits & ~(1u << element)));

		/// <summary>
		/// Get the highest rank in the set
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rank Upmost() => (Rank)(_bits == 0 ? -1 : BitOperations.Log2((uint)_bits));

		/// <summary>
		/// Get the lowest rank in the set
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rank Lowest() => (Rank)(_bits == 0 ? -1 : BitOperations.TrailingZeroCount((uint)_bits));

		public bool Equals(RankSet other) => _bits == other._bits;
		public bool Equals(ushort other) => _bits == other;
		public override bool Equals(object obj) => obj is RankSet other && Equals(other);
		public override int GetHashCode() => _bits.GetHashCode();

		public static RankSet Select(Rank element) => new((ushort)(1u << element));

		#region operators
		public static bool operator ==(RankSet a, RankSet b) => a._bits == b._bits;
		public static bool operator !=(RankSet a, RankSet b) => a._bits != b._bits;
		public static bool operator ==(RankSet a, ushort b) => a._bits == b;
		public static bool operator !=(RankSet a, ushort b) => a._bits != b;
		public static bool operator ==(ushort a, RankSet b) => a == b._bits;
		public static bool operator !=(ushort a, RankSet b) => a != b._bits;

		public static RankSet operator &(RankSet a, RankSet b) => new((ushort)(a._bits & b._bits));
		public static RankSet operator |(RankSet a, RankSet b) => new((ushort)(a._bits | b._bits));
		public static RankSet operator ^(RankSet a, RankSet b) => new((ushort)(a._bits ^ b._bits));
		public static RankSet operator ~(RankSet a) => new((ushort)~a._bits);

		public static implicit operator ushort(RankSet x) => x._bits;
		public static implicit operator uint(RankSet x) => x._bits;
		public static implicit operator ulong(RankSet x) => x._bits;

		public static explicit operator RankSet(ushort x) => new(x);
		public static explicit operator checked RankSet(ushort x) => (x & ~_Ω) == 0 ? new(x) : throw new OverflowException();

		public static explicit operator RankSet(uint x) => new((ushort)x);
		public static explicit operator checked RankSet(uint x) => (x & ~_Ω) == 0 ? new((ushort)x) : throw new OverflowException();

		public static explicit operator RankSet(ulong x) => new((ushort)x);
		public static explicit operator checked RankSet(ulong x) => (x & ~(ulong)_Ω) == 0 ? new((ushort)x) : throw new OverflowException();
		#endregion

		#region static fields
		public static readonly RankSet Φ = new(_Φ);
		public static readonly RankSet Ω = new(_Ω);
		public static readonly RankSet Two = new(_2);
		public static readonly RankSet Three = new(_3);
		public static readonly RankSet Four = new(_4);
		public static readonly RankSet Five = new(_5);
		public static readonly RankSet Six = new(_6);
		public static readonly RankSet Seven = new(_7);
		public static readonly RankSet Eight = new(_8);
		public static readonly RankSet Nine = new(_9);
		public static readonly RankSet Ten = new(_T);
		public static readonly RankSet Jack = new(_J);
		public static readonly RankSet Queen = new(_Q);
		public static readonly RankSet King = new(_K);
		public static readonly RankSet Ace = new(_A);
		#endregion
	}
}
