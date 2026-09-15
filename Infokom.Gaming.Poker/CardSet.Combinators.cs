using Infokom.Numerics.Extensions;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Infokom.Gaming.Poker
{
	public readonly partial struct CardSet
	{
		public readonly struct Combinator : IReadOnlyCollection<CardSet>
		{
			/// <summary>
			/// Available cards to choose from.
			/// </summary>
			public readonly CardSet Source;

			/// <summary>
			/// The total number of available cards.
			/// </summary>
			public int N => BitOperations.PopCount(Source);

			/// <summary>
			/// The number of cards in each combination.
			/// </summary>
			public readonly int K;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Combinator(CardSet source, int k) => (Source, K) = (source, k);

			/// <summary>
			/// The total number of combinations of <see cref="K"/> cards that can be chosen from <see cref="N"/> available cards.
			/// </summary>
			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Math.Choose(N, K);
			}

			public struct Enumerator : IEnumerator<CardSet>
			{
				private readonly ulong _source;
				private readonly int _k;
				private ulong _current, _bmiIdx, _bmiMax, _submask;
				private bool _zeroCase;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Enumerator(Combinator owner)
				{
					(_source, _k) = (owner.Source, owner.K);
					int pop = BitOperations.PopCount(_source);
					if (_k < 0 || _k > pop) return;
					_zeroCase = (_k == 0);

					if (Bmi2.X64.IsSupported)
					{
						_bmiIdx = (1UL << _k) - 1UL;
						_bmiMax = _bmiIdx << (pop - _k);
					}
					else _submask = _source;
				}

				public readonly CardSet Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => (CardSet)_current;
				}

				readonly object IEnumerator.Current => this.Current;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext()
				{
					if (Bmi2.X64.IsSupported)
					{
						if (_bmiIdx == 0) return false;
						_current = Bmi2.X64.ParallelBitDeposit(_bmiIdx, _source);
						if (_bmiIdx == _bmiMax) _bmiIdx = 0;
						else
						{
							ulong c = _bmiIdx & (ulong)-(long)_bmiIdx;
							ulong r = _bmiIdx + c;
							_bmiIdx = (((r ^ _bmiIdx) >> 2) / c) | r;
						}
						return true;
					}

					while (_submask > 0)
					{
						ulong pot = _submask;
						_submask = (_submask - 1) & _source;
						if (BitOperations.PopCount(pot) == _k) { _current = pot; return true; }
					}

					if (_zeroCase) { _zeroCase = false; _current = 0; return true; }
					return false;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Reset()
				{
					int pop = BitOperations.PopCount(_source);
					if (_k < 0 || _k > pop) return;
					_zeroCase = (_k == 0);

					if (Bmi2.X64.IsSupported)
					{
						_bmiIdx = (1UL << _k) - 1UL;
						_bmiMax = _bmiIdx << (pop - _k);
					}
					else _submask = _source;
				}
				readonly void IDisposable.Dispose() { }
			}
			public Enumerator GetEnumerator() => new(this);
			IEnumerator<CardSet> IEnumerable<CardSet>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
		}

		/// <summary>
		/// Get the <paramref name="k"/>-cards combinations from this <see cref="CardSet"/>.
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public Combinator Choose(int k) => new(this, k);
	}
}
