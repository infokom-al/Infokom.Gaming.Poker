using System.Collections;

namespace Poker.Calc
{
	internal struct BitFlagsEnumerator(int flags) : IEnumerator<int>, IEnumerator, IDisposable
	{
		private readonly int _flags = flags;

		private int _currentPosition = -1;

		public int Current => _currentPosition;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			while (_currentPosition < 32)
			{
				_currentPosition++;
				int num = 1 << _currentPosition;
				if ((_flags & num) != 0)
				{
					return true;
				}
				if (num >= _flags)
				{
					return false;
				}
			}
			return false;
		}

		public void Reset()
		{
			_currentPosition = -1;
		}

		public void Dispose()
		{
		}
	}
}
