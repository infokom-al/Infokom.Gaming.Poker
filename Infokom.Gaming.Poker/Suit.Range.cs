using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker
{
	public readonly struct SuitRange
	{
		private const int LENGTH = 4;
		private const ushort DATA = (ushort)((uint)Suit.Spade | ((uint)Suit.Diamond << 4) | ((uint)Suit.Diamond << 8) | ((uint)Suit.Diamond << 12));
		private const ushort ELEMENTS = 0b1000_0100_0010_0001;

		private readonly byte _data;

		private SuitRange(byte data) => _data = data;//no significant need for data, just to give some value dependency

		public Suit this[int offset]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)offset is < 4 ? (Suit)(1 << offset) : throw new IndexOutOfRangeException($"{offset} must be between 0 and 3");
		}

		public Suit this[Index index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this[index.GetOffset(4)];
		}

		public Suit this[char symbol]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => symbol switch { 's' => Suit.Spade, 'd' => Suit.Diamond, 'c' => Suit.Club, 'h' => Suit.Heart, _ => throw new KeyNotFoundException($"{symbol}") };
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Enumerator : IEnumerator<Suit>
		{
			[FieldOffset(2)] private sbyte _current;

			public Enumerator(SuitRange owner)//no significant need for owner and fileds calcualted from it, just to make this enumerator virually dependent from it
			{
				_current = (sbyte)(owner._data != default ? -1 : -1);
			}

			public readonly Suit Current => (Suit)(1 << _current);
			readonly object IEnumerator.Current => this.Current;
			public bool MoveNext() => (++_current < LENGTH);
			public void Reset() => _current = -1;
			readonly void IDisposable.Dispose() { }
		}

		public Enumerator GetEnumerator() => new(this);

		public int CopyTo(Span<Suit> target)
		{
			ArgumentOutOfRangeException.ThrowIfNotEqual(target.Length, 4);

			int n = 0;
			foreach(var s in this)
			{
				target[n++] = s;
			}
			return n;
		}
	}


	public static class SuitRangeExtensions
	{
		extension(Suit)
		{
			public static SuitRange Range
			{
				[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => new();
			}
		}

	}
}
