using System.Numerics;

namespace Infokom.Numerics.Extensions
{

	public static class BIT
	{
		public readonly struct Mask<T>(T value) where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IShiftOperators<T, int, T>
		{
			public T Value { get; } = value;

			public bool this[int index] => (Value & (T.One << index)) != T.Zero;
		}

		public readonly struct Flag<T> where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IShiftOperators<T, int, T>
		{
			public Flag(int index) => this.Index = index;

			public int Index { get; }
		}





		extension<T>(T source) where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IShiftOperators<T, int, T>
		{
			public Mask<T> ToMask() => new(source);
		}

		extension<T>(Mask<T> source) where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IShiftOperators<T, int, T>
		{
			public static Mask<T> NONE => new( T.Zero);
			public static Mask<T> FULL => new(~T.Zero);
		}










		extension(Flag<uint> flag)
		{
			public uint Value => 1u << flag.Index;
		}

		extension(Mask<uint> mask)
		{
			public Mask<uint> Include(Flag<uint> flag) => new(mask.Value |  flag.Value);
			public Mask<uint> Exclude(Flag<uint> flag) => new(mask.Value & ~flag.Value);
			public bool Contains(Flag<uint> flag) => (mask.Value & flag.Value) != 0;
		}







		extension(Flag<ulong> flag)
		{
			public uint Value => 1u << flag.Index;
		}

		extension(Mask<ulong> mask)
		{
			public Mask<ulong> Include(Flag<ulong> flag) => new(mask.Value | flag.Value);
			public Mask<ulong> Exclude(Flag<ulong> flag) => new(mask.Value & ~flag.Value);
			public bool Contains(Flag<ulong> flag) => (mask.Value & flag.Value) != 0;
		}
	}


}
