using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Infokom.Numerics.Atomics
{
	public readonly partial struct Bit
	{
		private readonly byte _data;

		private Bit(byte data) => _data = data;

		public static readonly Bit Zero;
		public static readonly Bit One = new(1);
		public static readonly Bit Bug = new(unchecked((byte)~1));


		public static bool operator true(Bit source) => source._data == 1;
		public static bool operator false(Bit source) => source._data == 0;


		public static implicit operator Bit(bool source) => source ? One : Zero;
	}


	
}