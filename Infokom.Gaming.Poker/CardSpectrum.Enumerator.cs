using Infokom.Gaming.Poker.Atomics;

using System.Collections;
using System.Numerics;

namespace Infokom.Gaming.Poker
{
	public readonly partial struct CardSpectrum
	{
		#region IReadOnlyCollection<Card>


		#endregion



		public struct Enumerator : IEnumerator<Card>
		{
			private ulong _residualMask;
			private int _currentFlagIndex;

			public Enumerator(ulong mask)
			{
				_residualMask = mask;
				_currentFlagIndex = default;
			}

			public bool MoveNext()
			{
				if (_residualMask == 0)
					return false;

				_currentFlagIndex = BitOperations.TrailingZeroCount(_residualMask);

				_residualMask &= _residualMask - 1;

				return true;
			}

			public readonly Card Current => Card.ValueOf(_currentFlagIndex);
			readonly object IEnumerator.Current => Current;
			public void Reset() => throw new NotSupportedException();
			public readonly void Dispose() { }
		}
	}
}
