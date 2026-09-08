using Infokom.Numerics.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.Win32;

using System.Buffers;
using System.Collections;
using System.Collections.Specialized;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Infokom.Numerics.Atomics
{

	

	/// <summary>
	/// Represents a fixed-capacity binary mask whose bits are addressed by <typeparamref name="Tx"/>.
	/// </summary>
	/// <typeparam name="T">The unsigned integer type that stores the mask.</typeparam>
	/// <typeparam name="Tx">A one-byte unmanaged type whose raw value represents a bit offset.</typeparam>
	public readonly struct BitMap<T, Tx> : IEquatable<BitMap<T, Tx>>, IComparable<BitMap<T, Tx>>, IComparable where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T> where Tx : unmanaged
	{
		#region STATIC
		private const int INDEX_SIZE = sizeof(sbyte);

		private static readonly int CAPACITY;

		static BitMap()
		{
			if (Unsafe.SizeOf<Tx>() != INDEX_SIZE)
			{
				throw new InvalidOperationException(
				    $"{typeof(Tx).Name} must occupy exactly one byte.");
			}

			CAPACITY = Unsafe.SizeOf<T>() * 8;
		}

		/// <summary>
		/// Gets the number of bits available in the underlying storage value.
		/// </summary>
		/// <value>The capacity of the binary mask in bits.</value>
		public static int Capacity => CAPACITY;
		#endregion

		private readonly T _value;

		/// <summary>
		/// Initializes a binary mask from an existing raw mask value.
		/// </summary>
		/// <param name="mask">The raw bit mask to encapsulate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private BitMap(T mask)
		{
			_value = mask;
		}

		/// <summary>
		/// Number of 1 bits in this binary, equiv to sum of all bits.
		/// </summary>
		/// <value>The population count of the underlying mask.</value>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => int.CreateChecked(T.PopCount(_value));
		}

		/// <summary>
		/// Predicates the no 1 bit present
		/// </summary>
		/// <value><see langword="true"/> when the mask is empty; otherwise, <see langword="false"/>.</value>
		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => T.IsZero(_value);
		}

		/// <summary>
		/// Check if this binary contains a 1 bit at a specific position
		/// </summary>
		/// <param name="element">The value whose raw one-byte representation is used as the bit offset.</param>
		/// <returns><see langword="true"/> when the bit is active; otherwise, <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Tx element) => (_value & (T.One << Unsafe.As<Tx, sbyte>(ref element))) != T.Zero;



		/// <summary>
		/// Check if this binary contains all 1 bits in a binary raw value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(T value) => (_value & value) == _value;

		/// <summary>
		/// Check if this binary contains all 1 bits of another binary
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(BitMap<T, Tx> other) => (_value & other._value) == other._value;

		/// <summary>
		/// Returns a new binary with 1 bit at a specific index
		/// </summary>
		/// <param name="element">The value whose raw one-byte representation is used as the bit offset.</param>
		/// <returns>A binary mask with the requested bit activated.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitMap<T, Tx> Include(Tx element) => new(_value | (T.One << Unsafe.As<Tx, sbyte>(ref element)));

		/// <summary>
		/// Returns a new binary with 0 bit at a specific index
		/// </summary>
		/// <param name="element">The value whose raw one-byte representation is used as the bit offset.</param>
		/// <returns>A binary mask with the requested bit deactivated.</returns>
		/// <remarks>
		/// $$A \leftarrow A \setminus \{b\}$$
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitMap<T, Tx> Exclude(Tx element) => new(_value & ~(T.One << Unsafe.BitCast<Tx, sbyte>(element)));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a">The source mask.</param>
		/// <param name="other">The mask whose active bits are removed.</param>
		/// <returns>A mask containing bits active in <paramref name="a"/> but not in <paramref name="other"/>.</returns>
		/// <remarks>
		/// $$A \leftarrow A \setminus B$$
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitMap<T, Tx> Exclude(BitMap<T, Tx> other) => new(_value & ~other._value);





		/// <summary>
		/// Returns the complete binary representation of the mask, padded with leading zeroes.
		/// </summary>
		/// <returns>A binary string whose length is equal to <see cref="Capacity"/>.</returns>
		public override string ToString()
		{
			Span<char> characters = stackalloc char[Capacity];
			var value = _value;

			for (var offset = Capacity - 1; offset >= 0; offset--)
			{
				characters[offset] = (value & T.One) == T.Zero ? '0' : '1';
				value >>= 1;
			}

			return new string(characters);
		}

		/// <summary>
		/// Determines whether this mask is equal to another mask.
		/// </summary>
		/// <param name="other">The mask to compare with this instance.</param>
		/// <returns><see langword="true"/> when both masks have the same raw value; otherwise, <see langword="false"/>.</returns>
		public bool Equals(BitMap<T, Tx> other) => _value == other._value;

		/// <summary>
		/// Determines whether this mask is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><see langword="true"/> when the object is an equal binary mask; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj) => obj is BitMap<T, Tx> other && Equals(other);

		/// <summary>
		/// Returns the hash code for this binary mask.
		/// </summary>
		/// <returns>A hash code based on the underlying mask value.</returns>
		public override int GetHashCode() => _value.GetHashCode();

		/// <summary>
		/// Compares this mask with another mask.
		/// </summary>
		/// <param name="other">The mask to compare with this instance.</param>
		/// <returns>A value indicating the relative order of the masks.</returns>
		public int CompareTo(BitMap<T, Tx> other) => _value.CompareTo(other._value);

		/// <summary>
		/// Compares this mask with the specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns>A value indicating the relative order of the masks.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not a compatible binary mask.</exception>
		public int CompareTo(object obj) => obj switch
		{
			null => 1,
			BitMap<T, Tx> other => CompareTo(other),
			_ => throw new ArgumentException($"Object must be of type {typeof(BitMap<T, Tx>).Name}.", nameof(obj))
		};

		//$$A = B \equiv \forall x \in A \cup B: x \in A \cap B$$
		/// <summary>
		/// Determines whether two binary masks are equal.
		/// </summary>
		/// <param name="a">The first mask to compare.</param>
		/// <param name="b">The second mask to compare.</param>
		/// <returns><see langword="true"/> when both masks are equal; otherwise, <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(BitMap<T, Tx> a, BitMap<T, Tx> b) => a._value == b._value;


		//$$A \neq B \equiv \exists x \in A \cup B: \neg(x \in A \cap B)$$
		/// <summary>
		/// Determines whether two binary masks are not equal.
		/// </summary>
		/// <param name="a">The first mask to compare.</param>
		/// <param name="b">The second mask to compare.</param>
		/// <returns><see langword="true"/> when the masks differ; otherwise, <see langword="false"/>.</returns>
		public static bool operator !=(BitMap<T, Tx> a, BitMap<T, Tx> b) => a._value != b._value;

		/// <summary>
		/// Shifts every bit in a binary mask to the a.
		/// </summary>
		/// <param name="x">The mask to shift.</param>
		/// <param name="k">The number of positions to shift.</param>
		/// <returns>The shifted mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> operator <<(BitMap<T, Tx> x, int k) => new(x._value << k);

		/// <summary>
		/// Shifts every bit in a binary mask to the b.
		/// </summary>
		/// <param name="x">The mask to shift.</param>
		/// <param name="k">The number of positions to shift.</param>
		/// <returns>The shifted mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> operator >>(BitMap<T, Tx> x, int k) => new(x._value >> k);

		//$$\hat{A} \equiv \Omega \setminus A$$
		/// <summary>
		/// Returns the complement of a binary mask.
		/// </summary>
		/// <param name="a">The mask to complement.</param>
		/// <returns>A mask with every storage bit inverted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Complement(BitMap<T, Tx> a) => new(~a._value);

		//$$A \cap B$$
		/// <summary>
		/// Returns the intersection of two binary masks.
		/// </summary>
		/// <param name="a">The first mask.</param>
		/// <param name="b">The second mask.</param>
		/// <returns>A mask containing bits active in both masks.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Intersect(BitMap<T, Tx> a, BitMap<T, Tx> b) => new(a._value & b._value);

		//$$A \cup B$$
		/// <summary>
		/// Returns the union of two binary masks.
		/// </summary>
		/// <param name="a">The first mask.</param>
		/// <param name="b">The second mask.</param>
		/// <returns>A mask containing bits active in either mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Union(BitMap<T, Tx> a, BitMap<T, Tx> b) => new(a._value | b._value);


		//$$A \oplus B \equiv A \bigtriangleup B \equiv (A \setminus B\) \cup (B \setminus A\) = (A \setminus B\) \cup (B \setminus A\)$$
		/// <summary>
		/// Returns the symmetric difference of two binary masks.
		/// </summary>
		/// <param name="a">The first mask.</param>
		/// <param name="b">The second mask.</param>
		/// <returns>A mask containing bits active in exactly one mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> SymmetricDifference(BitMap<T, Tx> a, BitMap<T, Tx> b) => new(a._value ^ b._value);


		/// <summary>
		/// Returns the complement of a binary mask.
		/// </summary>
		/// <param name="a">The mask to complement.</param>
		/// <returns>A mask with every storage bit inverted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> operator ~(BitMap<T, Tx> a) => Complement(a);

		//$$A \cap B$$
		/// <summary>
		/// Returns the intersection of two binary masks.
		/// </summary>
		/// <param name="a">The first mask.</param>
		/// <param name="b">The second mask.</param>
		/// <returns>A mask containing bits active in both masks.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> operator &(BitMap<T, Tx> a, BitMap<T, Tx> b) => Intersect(a, b);

		//$$A \cup B$$
		/// <summary>
		/// Returns the union of two binary masks.
		/// </summary>
		/// <param name="a">The first mask.</param>
		/// <param name="b">The second mask.</param>
		/// <returns>A mask containing bits active in either mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> operator |(BitMap<T, Tx> a, BitMap<T, Tx> b) => Union(a, b);


		//$$A \oplus B$$
		/// <summary>
		/// Returns the symmetric difference of two binary masks.
		/// </summary>
		/// <param name="a">The first mask.</param>
		/// <param name="b">The second mask.</param>
		/// <returns>A mask containing bits active in exactly one mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> operator ^(BitMap<T, Tx> a, BitMap<T, Tx> b) => SymmetricDifference(a, b);









		//$$\Phi$$
		/// <summary>
		/// No 1 bit
		/// </summary>
		/// <value>A binary mask with no active bits.</value>
		public static readonly BitMap<T, Tx> NIL = default;

		//$$\Omega$$
		/// <summary>
		/// No 0 bit
		/// </summary>
		public static readonly BitMap<T, Tx> ALL = default;




		/// <summary>
		/// Creates a binary mask with one selected index.
		/// </summary>
		/// <param name="x1">The index to activate.</param>
		/// <returns>A binary mask containing the selected index.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Select(Tx x1) => new((
			(T.One << Unsafe.BitCast<Tx, sbyte>(x1))));

		/// <summary>
		/// Creates a binary mask with two selected indexes.
		/// </summary>
		/// <param name="x1">The first index to activate.</param>
		/// <param name="x2">The second index to activate.</param>
		/// <returns>A binary mask containing the selected indexes.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Select(Tx x1, Tx x2) => new((
			(T.One << Unsafe.BitCast<Tx, sbyte>(x1)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x2))));

		/// <summary>
		/// Creates a binary mask with three selected indexes.
		/// </summary>
		/// <param name="x1">The first index to activate.</param>
		/// <param name="x2">The second index to activate.</param>
		/// <param name="x3">The third index to activate.</param>
		/// <returns>A binary mask containing the selected indexes.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Select(Tx x1, Tx x2, Tx x3) => new((
			(T.One << Unsafe.BitCast<Tx, sbyte>(x1)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x2)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x3))));

		/// <summary>
		/// Creates a binary mask with four selected indexes.
		/// </summary>
		/// <param name="x1">The first index to activate.</param>
		/// <param name="x2">The second index to activate.</param>
		/// <param name="x3">The third index to activate.</param>
		/// <param name="x4">The fourth index to activate.</param>
		/// <returns>A binary mask containing the selected indexes.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Select(Tx x1, Tx x2, Tx x3, Tx x4) => new((
			(T.One << Unsafe.BitCast<Tx, sbyte>(x1)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x2)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x3)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x4))));

		/// <summary>
		/// Creates a binary mask with five selected indexes.
		/// </summary>
		/// <param name="x1">The first index to activate.</param>
		/// <param name="x2">The second index to activate.</param>
		/// <param name="x3">The third index to activate.</param>
		/// <param name="x4">The fourth index to activate.</param>
		/// <param name="x5">The fifth index to activate.</param>
		/// <returns>A binary mask containing the selected indexes.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Select(Tx x1, Tx x2, Tx x3, Tx x4, Tx x5) => new((
			(T.One << Unsafe.BitCast<Tx, sbyte>(x1)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x2)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x3)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x4)) |
			(T.One << Unsafe.BitCast<Tx, sbyte>(x5))));

		/// <summary>
		/// Creates a binary mask with the specified selected indexes.
		/// </summary>
		/// <param name="source">The indexes to activate.</param>
		/// <returns>A binary mask containing all selected indexes.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BitMap<T, Tx> Select(params ReadOnlySpan<Tx> source)
		{
			var target = T.Zero;
			foreach (var x in source) target |= T.One << Unsafe.BitCast<Tx, sbyte>(x);
			return new(target);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator T(BitMap<T, Tx> source) => source._value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator BitMap<T, Tx>(T source) => new(source);











		/// <summary>
		/// Enumerates the indexes of active bits in a binary mask.
		/// </summary>
		public struct Enumerator
		{
			private T _data;
			private sbyte _offset;

			/// <summary>
			/// Initializes an enumerator for the specified spectrum.
			/// </summary>
			/// <param name="value">The raw binary mask to enumerate.</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(BitMap<T, Tx> owner)
			{
				_data = owner._value;
				_offset = -1;
			}

			/// <summary>
			/// Gets the current active element.
			/// </summary>
			/// <value>The index corresponding to the current active bit.</value>
			public readonly Tx Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Unsafe.BitCast<sbyte, Tx>(_offset);
			}

			/// <summary>
			/// Advances the enumerator to the next less significant active element.
			/// </summary>
			/// <returns><see langword="true"/> when another active bit exists; otherwise, <see langword="false"/>.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (_data == T.Zero)
				{
					_offset = default;
					return false;
				}

				var offset = _data.GetShortestBitLength() - 1;

				_data &= ~(T.One << offset);
				return true;
			}
		}






		/// <summary>
		/// Attempts to get the index of the lowest active bit.
		/// </summary>
		/// <param name="a">The binary mask to inspect.</param>
		/// <param name="x">The lowest active index when the method returns <see langword="true"/>.</param>
		/// <returns><see langword="true"/> when an active bit exists; otherwise, <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetLowest(out Tx x)
		{
			if (_value is not 0)
			{
				x = Unsafe.BitCast<sbyte, Tx>((sbyte)int.CreateChecked(T.TrailingZeroCount(_value)));
				return true;
			}

			x = default;
			return false;
		}

		/// <summary>
		/// Attempts to get the index of the highest active bit.
		/// </summary>
		/// <param name="a">The binary mask to inspect.</param>
		/// <param name="x">The highest active index when the method returns <see langword="true"/>.</param>
		/// <returns><see langword="true"/> when an active bit exists; otherwise, <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetUpmost(out Tx x)
		{
			if (_value is not 0)
			{
				x = Unsafe.BitCast<sbyte, Tx>((sbyte)(CAPACITY - 1 - int.CreateChecked(T.LeadingZeroCount(_value))));
				return true;
			}

			x = default;
			return false;
		}

		/// <summary>
		/// Isolates the lowest active bit of a binary mask.
		/// </summary>
		/// <param name="a">The binary mask to inspect.</param>
		/// <returns>A mask containing only the lowest active bit, or an empty mask when <paramref name="a"/> is empty.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitMap<T, Tx> IsolateLowest() => new(_value & (T.Zero - _value));

		/// <summary>
		/// Isolates the highest active bit of a binary mask.
		/// </summary>
		/// <param name="a">The binary mask to inspect.</param>
		/// <returns>A mask containing only the highest active bit, or an empty mask when <paramref name="a"/> is empty.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitMap<T, Tx> IsolateUpmost()
		{
			if (_value == T.Zero)
				return NIL;
			return new(T.One << (CAPACITY - 1 - int.CreateChecked(T.LeadingZeroCount(_value))));
		}

		/// <summary>
		/// Selects up to the specified number of lowest active bits.
		/// </summary>
		/// <param name="a">The binary mask to inspect.</param>
		/// <param name="n">The maximum number of lowest active bits to select.</param>
		/// <returns>A mask containing the selected lowest active bits.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="n"/> is negative.</exception>
		public BitMap<T, Tx> IsolateLowest(int n)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(n);

			var remaining = _value;
			var selected = T.Zero;

			while (n > 0 && remaining != T.Zero)
			{
				var bit = remaining & (T.Zero - remaining);

				selected |= bit;
				remaining ^= bit;
				n--;
			}

			return new(selected);
		}

		/// <summary>
		/// Selects up to the specified number of highest active bits.
		/// </summary>
		/// <param name="a">The binary mask to inspect.</param>
		/// <param name="count">The maximum number of highest active bits to select.</param>
		/// <returns>A mask containing the selected highest active bits.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative.</exception>
		public BitMap<T, Tx> TakeUpmost(int count)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(count);

			var remaining = _value;
			var selected = T.Zero;

			while (count > 0 && remaining != T.Zero)
			{
				var leadingZeros = int.CreateChecked(T.LeadingZeroCount(remaining));
				var offset = Capacity - 1 - leadingZeros;
				var bit = T.One << offset;

				selected |= bit;
				remaining ^= bit;
				count--;
			}

			return new(selected);
		}
	}
}