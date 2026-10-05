using Infokom.Numerics;
using Infokom.Numerics.Atomics;

using System.Runtime.InteropServices;

using static Infokom.Gaming.Poker.Texas.GTO;

namespace Infokom.Gaming.Poker.Texas
{
	public static partial class GTO
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Cell<TValue> where TValue : unmanaged
		{
			public readonly Cell Key;
			public TValue Value;

			public Cell(Cell key, TValue value)
			{
				Key = key;
				Value = value;
			}

			public readonly Point<sbyte, sbyte> Position => Key.Position;


			public static implicit operator Cell<TValue>(Cell source) => new(source, default);
			public static implicit operator Cell<TValue>((Cell key, TValue value) source) => new(source.key, source.value);
		}



		public static Cell<TValue> To<TValue>(this Cell source, TValue value) where TValue : unmanaged => new(source, value);
	}
}