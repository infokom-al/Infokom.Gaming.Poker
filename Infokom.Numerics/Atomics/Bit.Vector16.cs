using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{

	[StructLayout(LayoutKind.Sequential)]
	public struct BitVector16 : IEquatable<BitVector16>, IEqualityOperators<BitVector16, BitVector16, bool>, IShiftOperators<BitVector16, int, BitVector16>
	{
		private const ushort ZEROS = ushort.MinValue;
		private const ushort UNITS = ushort.MaxValue;

		private ushort _bits;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private BitVector16(ushort bits) => _bits = bits;

		/// <summary>
		/// Gets or sets the value of the bit at the specified offset.
		/// </summary>
		/// <param name="offset">The offset of the target bit.</param>
		/// <returns>The value of the bit at the specified offset.</returns>
		/// <remarks>
		/// It is expected an <paramref name="offset"/> from 0 (LSB) to 15 (MSB); no guarantee about the behavior out of these bounds.
		/// </remarks>
		public Bit this[int offset]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (_bits & (1 << offset)) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _bits = value ? (ushort)(_bits | (1 << offset)) : (ushort)(_bits & ~(1 << offset));
		}

		/// <summary>
		/// Gets or sets the value of the bit at the specified index in the <see cref="BitVector16"/> instance.
		/// </summary>
		/// <param name="index">The zero-based index of the bit to get or set.</param>
		/// <returns>The value of the bit at the specified index.</returns>
		/// <remarks>
		/// It is expected an <paramref name="index"/> from 0 (LSB) to 15 (MSB); no guarantee about the behavior out of these bounds.
		/// </remarks>
		public Bit this[Index index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => this[index.GetOffset(16)];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => this[index.GetOffset(16)] = value;
		}



		/// <summary>
		/// Indexer for all bits inside a specific range.
		/// </summary>
		/// <param name="range">The range to access.</param>
		public Bit this[Range range]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				var (offset, length) = range.GetOffsetAndLength(16);
				if (value)
				{
					_bits |= (ushort)(((1 << length) - 1) << offset);
				}
				else
				{
					_bits &= (ushort)(~(((1 << length) - 1) << offset));
				}
			}
		}

		public readonly override string ToString()
		{
			var source = _bits.ToString("B16").AsSpan();
			return string.Create(16 * 3, source, (target, source) =>
			{
				target.Fill(' ');
				target[0] = '[';				
				for (int i = 0; i < source.Length; i++)
				{
					target[3 * i + 1] = source[^(i+1)];
				}
				target[^1] = ']';
			});
		}

		public readonly override int GetHashCode() => _bits.GetHashCode();
		public readonly bool Equals(BitVector16 other) => _bits.Equals(other._bits);
		public readonly override bool Equals(object obj) => obj is BitVector16 other && Equals(other);
		public static bool operator ==(BitVector16 v1, BitVector16 v2) => v1._bits == v2._bits;
		public static bool operator !=(BitVector16 v1, BitVector16 v2) => v1._bits != v2._bits;

		/// <summary>
		/// A <see cref="BitVector16">16-bit vector</see> with all elements set to <see cref="Bit.Zero">0</see>.
		/// </summary>
		public static readonly BitVector16 Zeros = new(ZEROS);

		/// <summary>
		/// A <see cref="BitVector16">16-bit vector</see> instance with all elements set to <see cref="Bit.Unit">1</see>.
		/// </summary>
		public static readonly BitVector16 Units = new(UNITS);

		/// <summary>
		/// A <see cref="BitVector16">16-bit vector</see> with all elements set to <see cref="Bit.Zero">0</see>.
		/// </summary>
		public static readonly BitVector16 Φ = Zeros;


		/// <summary>
		/// A <see cref="BitVector16">16-bit vector</see> instance with all elements set to <see cref="Bit.Unit">1</see>.
		/// </summary>
		public static readonly BitVector16 Ω = ~Φ;


		public static BitVector16 Create(Func<int, Bit> source) 
		{
			var result = new BitVector16();
			for (int i = 0; i < 16; i++)
				if (source(i))
					result._bits |= (ushort)(1 << i);
			return result;
		}

		public static BitVector16 Unit(int index = 0) => new((ushort)(1 << index));

		public static BitVector16 operator ~(BitVector16 vector) => new((ushort)(~vector._bits));
		public static BitVector16 operator &(BitVector16 v1, BitVector16 v2) => new((ushort)(v1._bits & v2._bits));
		public static BitVector16 operator |(BitVector16 v1, BitVector16 v2) => new((ushort)(v1._bits | v2._bits));
		public static BitVector16 operator ^(BitVector16 v1, BitVector16 v2) => new((ushort)(v1._bits ^ v2._bits));	

		/// <summary>
		/// Performs a bitwise left shift operation on the specified <see cref="BitVector16"/> instance by the specified number of bits.
		/// </summary>
		/// <param name="source">The <see cref="BitVector16"/> instance to shift.</param>
		/// <param name="offset">The number of bits to shift.</param>
		/// <returns>A new <see cref="BitVector16"/> instance with the bits shifted to the left by the specified offset.</returns>
		/// <remarks>
		/// <c>[xxxxxxxxxxxxxxxx] ≪ k = [xxxxxxxxxxx00000]</c>
		/// </remarks>
		public static BitVector16 operator <<(BitVector16 source, int offset) => new((ushort)(source._bits << offset));

		/// <summary>
		/// Performs a bitwise right shift operation on a <see cref="BitVector16"/> instance by certain number of bits.
		/// </summary>
		/// <param name="source">The <see cref="BitVector16"/> instance to shift.</param>
		/// <param name="offset">The number of bits to shift.</param>
		/// <returns>A new <see cref="BitVector16"/> instance with the bits shifted to the right by the specified offset.</returns>
		/// <remarks>
		/// <c>[xxxxxxxxxxxxxxxx] ≫ k = [00000xxxxxxxxxxx]</c>
		/// </remarks>
		public static BitVector16 operator >>(BitVector16 source, int offset) => new((ushort)(source._bits >> offset));

		/// <summary>
		/// Performs a bitwise left shift operation on the specified <see cref="BitVector16"/> instance by the specified number of bits.
		/// </summary>
		/// <param name="source">The <see cref="BitVector16"/> instance to shift.</param>
		/// <param name="offset">The number of bits to shift.</param>
		/// <returns>A new <see cref="BitVector16"/> instance with the bits shifted to the right by the specified offset.</returns>
		/// <remarks>
		/// <c>[xxxxxxxxxxxxxxxx] ⋙ k = [00000xxxxxxxxxxx]</c>
		/// </remarks>
		static BitVector16 IShiftOperators<BitVector16, int, BitVector16>.operator >>>(BitVector16 source, int offset) => source >> offset;


		public static implicit operator BitVector16(bool source) => source ? Units : Zeros;


		public static implicit operator ushort(BitVector16 vector) => vector._bits;
		public static implicit operator BitVector16(ushort value) => new(value);
		public static explicit operator BitVector16(uint value) => new((ushort)value);
		public static explicit operator checked BitVector16(uint value) => new(checked((ushort)value));
		public static explicit operator BitVector16(ulong value) => new((ushort)value);
		public static explicit operator checked BitVector16(ulong value) => new(checked((ushort)value));


		/// <summary>
		/// True operator for <see cref="BitVector16">16-bit vectors</see>
		/// </summary>
		/// <param name="vector"></param>
		/// <returns>true if the sum of all elements in <paramref name="vector"/> is exactly 16, and false otherwise.</returns>
		public static bool operator true(BitVector16 vector) => vector._bits == 0b__1111_1111_1111_1111;

		/// <summary>
		/// False operator for <see cref="BitVector16">16-bit vectors</see>
		/// </summary>
		/// <param name="vector"></param>
		/// <returns>true if the sum of all elements in the <paramref name="vector"/> is exactly 0, and false otherwise.</returns>
		public static bool operator false(BitVector16 vector) => vector._bits == 0b__0000_0000_0000_0000;
	}
}