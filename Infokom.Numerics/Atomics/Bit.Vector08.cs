using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BitVector08 : IEquatable<BitVector08>, IEqualityOperators<BitVector08, BitVector08, bool>, IShiftOperators<BitVector08, int, BitVector08>
	{
		private const byte ZEROS = byte.MinValue;
		private const byte UNITS = byte.MaxValue;
		private byte _bits;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private BitVector08(byte bits) => _bits = bits;

		/// <summary>
		/// Gets or sets the value of the bit at the specified index in the <see cref="BitVector08"/> instance.
		/// </summary>
		/// <param name="index">The zero-based index of the bit to get or set.</param>
		/// <returns>The value of the bit at the specified index.</returns>
		/// <remarks>
		/// It is expected that <paramref name="index"/> is in [0..8] range, where 0 represents the least significant bit (LSB) and 7 represents the most significant bit (MSB), otherwise unexpected behavior may occur.
		/// </remarks>
		public bool this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (_bits & (1 << index)) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _bits = value ? (byte)(_bits | (1 << index)) : (byte)(_bits & ~(1 << index));
		}

		public readonly override string ToString() => _bits.ToString("B8");
		public readonly override int GetHashCode() => _bits.GetHashCode();
		public readonly bool Equals(BitVector08 other) => _bits.Equals(other._bits);
		public readonly override bool Equals(object obj) => obj is BitVector08 other && Equals(other);
		public static bool operator ==(BitVector08 v1, BitVector08 v2) => v1._bits == v2._bits;
		public static bool operator !=(BitVector08 v1, BitVector08 v2) => v1._bits != v2._bits;

		/// <summary>
		/// A <see cref="BitVector08"/> instance with all bits <see cref="Bit.Zero">0</see>.
		/// </summary>
		public static readonly BitVector08 Zeros = new(ZEROS);

		/// <summary>
		/// A <see cref="BitVector08"/> instance with all bits <see cref="Bit.Unit">1</see>.
		/// </summary>
		public static readonly BitVector08 Units = new(UNITS);

		/// <summary>
		/// Performs a bitwise left shift operation on the specified <see cref="BitVector08"/> instance by the specified number of bits.
		/// </summary>
		/// <param name="source">The <see cref="BitVector08"/> instance to shift.</param>
		/// <param name="offset">The number of bits to shift.</param>
		/// <returns>A new <see cref="BitVector08"/> instance with the bits shifted to the left by the specified offset.</returns>
		public static BitVector08 operator <<(BitVector08 source, int offset) => new((byte)(source._bits << offset));

		/// <summary>
		/// Performs a bitwise right shift operation on a <see cref="BitVector08"/> instance by certain number of bits.
		/// </summary>
		/// <param name="source">The <see cref="BitVector08"/> instance to shift.</param>
		/// <param name="offset">The number of bits to shift.</param>
		/// <returns>A new <see cref="BitVector08"/> instance with the bits shifted to the right by the specified offset.</returns>
		public static BitVector08 operator >>(BitVector08 source, int offset) => new((byte)(source._bits >> offset));

		/// <summary>
		/// Performs a bitwise left shift operation on the specified <see cref="BitVector08"/> instance by the specified number of bits.
		/// </summary>
		/// <param name="source">The <see cref="BitVector08"/> instance to shift.</param>
		/// <param name="offset">The number of bits to shift.</param>
		/// <returns>A new <see cref="BitVector08"/> instance with the bits shifted to the right by the specified offset.</returns>
		static BitVector08 IShiftOperators<BitVector08, int, BitVector08>.operator >>>(BitVector08 source, int offset) => source >> offset;



		public static implicit operator ushort(BitVector08 vector) => vector._bits;
		public static implicit operator BitVector08(byte value) => new(value);

		public static explicit operator BitVector08(ushort value) => new((byte)value);
		public static explicit operator checked BitVector08(ushort value) => new(checked((byte)value));
		
		public static explicit operator BitVector08(uint value) => new((byte)value);
		public static explicit operator checked BitVector08(uint value) => new(checked((byte)value));
		
		public static explicit operator BitVector08(ulong value) => new((byte)value);
		public static explicit operator checked BitVector08(ulong value) => new(checked((byte)value));
	}
}