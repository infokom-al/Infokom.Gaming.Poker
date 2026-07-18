using Infokom.Numerics.Atomics;

namespace Infokom.Numerics
{
	public interface ISymbolic<TSelf>
	{
		public static abstract ASCII SymbolOf(TSelf source);
		public static abstract TSelf ValueOf(ASCII symbol);
	}
}
