using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Infokom.Numerics.Atomics
{
	public readonly struct Binary08 : IVectorial<Binary08, bool>
	{
		private readonly byte _data;

		public bool this[int index] => throw new NotImplementedException();

		public static Size<sbyte> Size => Infokom.Numerics.Atomics.Size.X((sbyte)8);

		public static bool Extract(Binary08 source, int offset) => offset is >= 0 and <= 7 ? ((source._data >> offset) & 1) is 0 : throw new ArgumentOutOfRangeException(nameof(offset));


		public static Binary08 Isolate(Binary08 source, int offset) => throw new NotImplementedException();



		public static bool TryExtract(Binary08 source, int offset, out bool target) => throw new NotImplementedException();
		public static bool TryIsolate(Binary08 source, int offset, out Binary08 target) => throw new NotImplementedException();
	}

	[StructLayout(LayoutKind.Explicit)]
	public partial struct Binary64 : IVector<ulong>, IVector<uint>, IVector<ushort>, IVector<byte>, IVector<bool>
	{

		[FieldOffset(0)] private readonly ulong _data;



		[FieldOffset(0)] private uint _x;
		[FieldOffset(4)] private uint _y;

		[FieldOffset(0)] private ushort _xx;
		[FieldOffset(2)] private ushort _xy;
		[FieldOffset(4)] private ushort _yx;
		[FieldOffset(6)] private ushort _yy;

		[FieldOffset(0)] private byte _xxx;
		[FieldOffset(1)] private byte _xxy;
		[FieldOffset(2)] private byte _xyx;
		[FieldOffset(3)] private byte _xyy;
		[FieldOffset(4)] private byte _yxx;
		[FieldOffset(5)] private byte _yxy;
		[FieldOffset(6)] private byte _yyx;
		[FieldOffset(7)] private byte _yyy;



		/// <summary>
		/// Creates a new spectrum from the given 64 bits integer.
		/// </summary>
		/// <param name="data"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Binary64(ulong data) => _data = data;
		/// <summary>
		/// Creates a new spectrum from the 2 raw 32 binaries
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Binary64(uint x, uint y) => (_x, _y) = (x, y);


		/// <summary>
		/// Creates a new spectrum from 4 raw 16 binaries
		/// </summary>
		/// <param name="xx"></param>
		/// <param name="xy"></param>
		/// <param name="yx"></param>
		/// <param name="yy"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Binary64(ushort xx, ushort xy, ushort yx, ushort yy) => (_xx, _xy, _yx, _yy) = (xx, xy, yx, yy);

		/// <summary>
		/// Creates a new spectrum from 8 raw 8 binaries
		/// </summary>
		/// <param name="xxx"></param>
		/// <param name="xxy"></param>
		/// <param name="xyx"></param>
		/// <param name="xyy"></param>
		/// <param name="yxx"></param>
		/// <param name="yxy"></param>
		/// <param name="yyx"></param>
		/// <param name="yyy"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Binary64(byte xxx, byte xxy, byte xyx, byte xyy, byte yxx, byte yxy, byte yyx, byte yyy) => (_xxx, _xxy, _xyx, _xyy, _yxx, _yxy, _yyx, _yyy) = (xxx, xxy, xyx, xyy, yxx, yxy, yyx, yyy);





		readonly ulong IVector<ulong>.this[int index] => index == 0 ? _data : throw new ArgumentOutOfRangeException(nameof(index));

		readonly uint IVector<uint>.this[int index] => index is 0 ? _x : index is 1 ? _y : throw new ArgumentOutOfRangeException(nameof(index));

		readonly ushort IVector<ushort>.this[int index] => index is 0 ? _xx : index is 1 ? _xy : index is 2 ? _yx : index is 3 ? _yy : throw new ArgumentOutOfRangeException(nameof(index));

		readonly byte IVector<byte>.this[int index] => index is 0 ? _xxx : index is 1 ? _xxy : index is 2 ? _xyx : index is 3 ? _xyy : index is 4 ? _yxx : index is 5 ? _yxy : index is 6 ? _yyx : index is 7 ? _yyy : throw new ArgumentOutOfRangeException(nameof(index));

		readonly bool IVector<bool>.this[int index] => index < 64 ? ((_data >> index) & 1) == 1 : throw new ArgumentOutOfRangeException(nameof(index));

		readonly int IVector<ulong>.Size => 1;

		readonly int IVector<uint>.Size => 2;

		readonly int IVector<ushort>.Size => 4;

		readonly int IVector<byte>.Size => 8;

		readonly int IVector<bool>.Size => 64;

		public readonly IVector<T> AsVector<T>()
		{
			if (typeof(T) == typeof(ulong) || typeof(T) == typeof(uint) || typeof(T) == typeof(ushort) || typeof(T) == typeof(byte))
				return (IVector<T>)(IVector<byte>)this;
			throw new NotSupportedException($"Cannot convert {nameof(Binary64)} to {typeof(T).Name}");
		}



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ulong(Binary64 value) => value._data;



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Binary64(ulong value) => new(value);





	}





	
}