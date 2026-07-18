using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Atomics
{

	public readonly struct Bit<TValue> : IShiftOperators<Bit<TValue>, int, Bit<TValue>> where TValue : unmanaged, IBinaryInteger<TValue>
	{
		private readonly int _index;

		internal Bit(int index) => this._index = index;

		public TValue Value => (TValue.One << _index);

		public static Bit<TValue> operator <<(Bit<TValue> value, int shiftAmount) => new(value._index + shiftAmount);
		public static Bit<TValue> operator >>(Bit<TValue> value, int shiftAmount) => new(value._index - shiftAmount);
		static Bit<TValue> IShiftOperators<Bit<TValue>, int, Bit<TValue>>.operator >>>(Bit<TValue> value, int shiftAmount) => value >> shiftAmount;
	}

	public readonly struct Binary<TValue> where TValue : unmanaged, IBinaryInteger<TValue>
	{
		private readonly TValue _value;

		private Binary(TValue value)
		{
			_value = value;
		}

		public static readonly Binary<TValue> Zeros = new(TValue.Zero);

		public static readonly Binary<TValue> Ones = new(TValue.AllBitsSet);

		public static implicit operator TValue(Binary<TValue> source) => source._value;
		public static implicit operator Binary<TValue>(TValue source) => new(source);
	}



}
