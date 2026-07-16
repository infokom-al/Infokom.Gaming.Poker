using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics
{

	public interface ISize<Tx, Ty, Tz>
	{
		public Tx X { get; }
		public Ty Y { get; }
		public Tz Z { get; }
	}

	public readonly record struct Size<Tx>(Tx X) where Tx : unmanaged, INumber<Tx>
	{
		
	}

	public readonly record struct Size<Tx, Ty>(Tx X, Ty Y) where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
	{

	}

	public readonly record struct Size<Tx, Ty, Tz>(Tx X, Ty Y, Tz Z) where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty> where Tz : unmanaged, INumber<Tz>
	{
		
	}

}