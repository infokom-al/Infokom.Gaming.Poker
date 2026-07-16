using System.Numerics;

namespace Infokom.Numerics
{
	public readonly record struct Point<Tx>(Tx X) where Tx : IFloatingPoint<Tx>;
	public readonly record struct Point<Tx, Ty>(Tx X, Ty Y) where Tx : IFloatingPoint<Tx> where Ty : IFloatingPoint<Ty>;
	public readonly record struct Point<Tx, Ty, Tz>(Tx X, Ty Y, Tz Z) where Tx : IFloatingPoint<Tx> where Ty : IFloatingPoint<Ty> where Tz : IFloatingPoint<Tz>;
}
