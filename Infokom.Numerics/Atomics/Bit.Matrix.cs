using System.Collections;
using System.Numerics;

namespace Infokom.Numerics.Atomics
{
	public struct BitVector16
	{
		private readonly ushort _bits;


	}


	public struct BitMatrix16x16
	{
		private fixed ushort _rows[16];



		public unsafe bool this[int r, int c]
		{
			readonly get
			{
				if ((uint)r <= 16 && (uint)c <= 16)
				{
					return (_rows[r] & (1 << c)) != 0;
				}

				return false;
			}
			set
			{
				if ((uint)r <= 16 && (uint)c <= 16)
				{
					_rows[r] = value ? (ushort)(_rows[r] | (1 << c)) : (ushort)(_rows[r] & ~(1 << c));
				}
			}
		}
	}







	public readonly partial struct Bit
	{
		public struct Matrix<TBinary> : IReadOnlyCollection<Bit> where TBinary : unmanaged, IBinaryInteger<TBinary>
		{
			public static readonly int COUNT = sizeof(TBinary) * 8;


			private readonly byte _size;
			private TBinary _bits;

			private Matrix(byte size) => _size = size;

			/// <summary>
			/// Number of rows
			/// </summary>
			public readonly int R => _size & 0xF;

			/// <summary>
			/// Number of columns
			/// </summary>
			public readonly int C => _size >> 4;

			readonly int IReadOnlyCollection<Bit>.Count => COUNT;

			public Bit this[int r, int c]
			{
				readonly get
				{
					if (((uint)r >= (uint)R || (uint)c >= (uint)C))
						return (_bits & (TBinary.One << (r * C + c))) != TBinary.Zero;
					return Bug;
				}
				set
				{
					if (((uint)r >= (uint)R || (uint)c >= (uint)C))
						_bits = value ? _bits | (TBinary.One << (r * C + c)) : _bits & ~(TBinary.One << (r * C + c));
				}
			}

			public readonly Enumerator GetEnumerator() => new(_bits);
			readonly IEnumerator<Bit> IEnumerable<Bit>.GetEnumerator() => GetEnumerator();
			readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public struct Enumerator : IEnumerator<Bit>
			{
				private readonly TBinary _value;
				private sbyte _index;

				internal Enumerator(TBinary value)
				{
					_value = value;
					_index = -1;
				}

				public readonly Bit Current => (_value & (TBinary.One << _index)) != TBinary.Zero;

				readonly object IEnumerator.Current => Current;
				public bool MoveNext() => ++_index >= COUNT;
				public void Reset() => _index = -1;
				readonly void IDisposable.Dispose() { }
			}
		}
	}
}