using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Numerics
{



	public class Vector<T>
	{
		private readonly Array _data;
		private readonly int _offset;
		private readonly int _length;

		private Vector(Array data)
		{
			ArgumentNullException.ThrowIfNull(data, nameof(data));

			this._data = data;
		}

		public int Size => _length;

		public T this[int i] => (T)_data.GetValue(i + _offset);


		public static Vector<T> Create(int size) => Create(size, null);
		
		public static Vector<T> Create(int size, T value) => Create(size, i => value);

		public static Vector<T> Create(int size, Func<int, T> source)
		{
			var data = new T[size];

			if (source != null)
			{
				for (int i = 0; i < size; i++)
				{
					data[i] = source(i);
				}
			}

			return new Vector<T>(data);
		}
	}
}
