//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.XPath;

using Infokom.Numerics.Extensions;
using static Infokom.Numerics.Extensions.Functions;

namespace Infokom.Numerics.Atomics
{
	public interface IColor
	{
		public float Red { get; }
		public float Green { get; }
		public float Blue { get; }
		public float Alpha { get; }
	}



	/// <summary>
	/// Binary data for ARGB32 color format.
	/// </summary>
	/// <remarks>
	/// 0xRR_GG_BB
	/// </remarks>
	[StructLayout(LayoutKind.Explicit, Size = 3)]
	public readonly struct RGB24 : IColor
	{
		private const uint B = 0x00_00_FF;
		private const uint G = 0x00_FF_00;
		private const uint R = 0xFF_00_00;

		[FieldOffset(0)] private readonly uint _data;
		[FieldOffset(0)] private readonly byte _b;
		[FieldOffset(1)] private readonly byte _g;
		[FieldOffset(2)] private readonly byte _r;

		[FieldOffset(0)] private readonly InlineArray3<BitVector8> _channels;

		private RGB24(uint data) => _data = data;

		private RGB24(byte r, byte g, byte b) => (_r, _g, _b) = (r, g, b);

		public float Red => _r / 255f;
		public float Green => _g / 255f;
		public float Blue => _b / 255f;
		float IColor.Alpha => throw new NotSupportedException();



		public override string ToString() => $"{_r:X2}{_g:X2}{_b:X2}";


		/// <summary>
		/// Creates a new RGB24 color from normalized float values for red, green, and blue channels.
		/// </summary>
		/// <param name="r">The red component (0.0 to 1.0).</param>
		/// <param name="g">The green component (0.0 to 1.0).</param>
		/// <param name="b">The blue component (0.0 to 1.0).</param>
		/// <returns>A new RGB24 color.</returns>
		/// <remarks>
		/// (r,g,b) is assumed to be normalized so that their sum equals 1.0. If not, a normalization step is performed to ensure the resulting color is valid.
		/// </remarks>
		public static RGB24 Create(float r, float g, float b) 
		{
			(r, g, b) = 255f * (r, g, b) / (r + g + b);

			return new((byte)r, (byte)g, (byte)b);
		}




		public static explicit operator RGB24(uint data) => new(data);
		public static implicit operator uint(RGB24 color) => color._data;


		public static RGB24 operator ~(RGB24 x) => new(~x._data);
		public static RGB24 operator &(RGB24 x, RGB24 y) => new(x._data & y._data);
		public static RGB24 operator |(RGB24 x, RGB24 y) => new(x._data | y._data);
		public static RGB24 operator ^(RGB24 x, RGB24 y) => new(x._data ^ y._data);
	}






	/// <summary>
	/// Binary data for ARGB32 color format.
	/// </summary>
	/// <remarks>
	/// 0xAA_RR_GG_BB
	/// </remarks>
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct ARGB32 : IColor
	{
		private const uint B = 0x00_00_00_FF;
		private const uint G = 0x00_00_FF_00;
		private const uint R = 0x00_FF_00_00;
		private const uint A = 0xFF_00_00_00;

		[FieldOffset(0)] private readonly uint _data;
		[FieldOffset(0)] private readonly byte _r;
		[FieldOffset(1)] private readonly byte _g;
		[FieldOffset(2)] private readonly byte _b;
		[FieldOffset(3)] private readonly byte _a;

		[FieldOffset(0)] public readonly ByteVector4 Channels;		

		private ARGB32(uint data) => _data = data;

		public float Red => _r / 255f;
		public float Green => _g / 255f;
		public float Blue => _b / 255f;
		public float Alpha => _a / 255f;

		public static implicit operator ARGB32(uint data) => new(data);
		public static implicit operator uint(ARGB32 color) => color._data;

		public static ARGB32 operator ~(ARGB32 x) => new (~x._data);
		public static ARGB32 operator &(ARGB32 x, ARGB32 y) => new(x._data & y._data);
		public static ARGB32 operator |(ARGB32 x, ARGB32 y) => new(x._data | y._data);
		public static ARGB32 operator ^(ARGB32 x, ARGB32 y) => new(x._data ^ y._data);
	}

	

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct ByteVector4
	{
		[FieldOffset(0)] private readonly uint _data;
		[FieldOffset(0)] private fixed byte _elements[4];

		[FieldOffset(0)] public byte X;
		[FieldOffset(1)] public byte Y;
		[FieldOffset(2)] public byte Z;
		[FieldOffset(3)] public byte W;

		private ByteVector4(uint data) => _data = data;

		private ByteVector4(byte x, byte y, byte z, byte w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public byte this[int index]
		{
			readonly get => (uint)index < 4 ? _elements[index] : throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be in the range [0, 3].");
			set => _elements[index] = (uint)index < 4 ? value : throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be in the range [0, 3].");
		}




		public static implicit operator ByteVector4(uint data) => new(data);

		public static implicit operator uint(ByteVector4 vector) => vector._data;


		public static readonly ByteVector4 Zeros = 0u;
		public static readonly ByteVector4 UnitX = new(1, 0, 0, 0);
		public static readonly ByteVector4 UnitY = new(0, 1, 0, 0);
		public static readonly ByteVector4 UnitZ = new(0, 0, 1, 0);
		public static readonly ByteVector4 UnitW = new(0, 0, 0, 1);
	}


	[StructLayout(LayoutKind.Explicit)]
	public struct BitMatrix4x8
	{
		[FieldOffset(0)] private readonly uint _data;
		[FieldOffset(0)] private ByteVector4 _bytes;
		[FieldOffset(0)] private BitVector32 _bits;
		[FieldOffset(0)] public InlineArray4<BitVector8> Rows;




		/// <summary>
		/// Access a bit in this matrix by row and column index.
		/// </summary>
		/// <param name="i">The row index (0-3).</param>
		/// <param name="j">The column index (0-7).</param>
		/// <returns>The bit at the specified row and column.</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Bit this[int i, int j]
		{
			readonly get => i switch
			{
				0 => Rows[0][j],
				1 => Rows[1][j],
				2 => Rows[2][j],
				3 => Rows[3][j],
				_ => throw new ArgumentOutOfRangeException($"(R:{i}, C:{j})")
			};

			set
			{
				switch (i)
				{
					case 0: Rows[0][j] = value; break;
					case 1: Rows[1][j] = value; break;
					case 2: Rows[2][j] = value; break;
					case 3: Rows[3][j] = value; break;
					default: throw new ArgumentOutOfRangeException($"(R:{i}, C:{j})");
				}
			}
		}
	}
}