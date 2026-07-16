using System.Numerics;

using Infokom.Numerics.Operators;

namespace Infokom.Numerics
{

	public static class IndexOperators
	{
		extension(Index<int, int, int>)
		{


			public static Index<int, int, int> Zeros => (0, 0, 0);

			public static Index<int, int, int> Ones => (1, 1, 1);

			public static Index<int, int, int> UnitX => (1, 0, 0);
			
			public static Index<int, int, int> UnitY => (0, 1, 0);

			public static Index<int, int, int> UnitZ => (0, 0, 1);




			public static Index<int, int, int> operator +(in Index<int, int, int> a, in Index<int, int, int> b)
			{
				var (ax, ay, az) = a;
				var (bx, by, bz) = b;

				return (ax, ay, az) + (bx, by, bz);
			}


			public static Index<int, int, int> operator -(in Index<int, int, int> a, in Index<int, int, int> b)
			{
				var (ax, ay, az) = a;
				var (bx, by, bz) = b;

				return (ax, ay, az) - (bx, by, bz);
			}

			public static Index<int, int, int> operator *(in Index<int, int, int> a, int b)
			{
				var (ax, ay, az) = a;

				return (ax, ay, az) * b;
			}

			public static Index<int, int, int> operator /(in Index<int, int, int> a, int b)
			{
				var (ax, ay, az) = a;

				return (ax, ay, az) / b;
			}
		}
	}



	public readonly record struct Index<Tx>(Tx X) where Tx : unmanaged, IBinaryInteger<Tx>
	{
		public static implicit operator Index<Tx>(ValueTuple<Tx> source) => new(source.Item1);
		public static explicit operator ValueTuple<Tx>(Index<Tx> source) => ValueTuple.Create(source.X);
	}

	public readonly record struct Index<Tx, Ty>(Tx X, Ty Y) where Tx : unmanaged, IBinaryInteger<Tx> where Ty : unmanaged, IBinaryInteger<Ty>
	{
		public static implicit operator Index<Tx, Ty>(ValueTuple<Tx, Ty> source) => new(source.Item1, source.Item2);
		public static explicit operator ValueTuple<Tx, Ty>(Index<Tx, Ty> source) => ValueTuple.Create(source.X, source.Y);
	}

	public readonly record struct Index<Tx, Ty, Tz>(Tx X, Ty Y, Tz Z) : IAdditiveIdentity<Index<Tx, Ty, Tz>, ValueTuple<Tx, Ty, Tz>> , IMultiplicativeIdentity<Index<Tx, Ty, Tz>, ValueTuple<Tx, Ty, Tz>>
	where Tx : unmanaged, IBinaryInteger<Tx> where Ty : unmanaged, IBinaryInteger<Ty> where Tz : unmanaged, IBinaryInteger<Tz>
	{

		public static implicit operator Index<Tx, Ty, Tz>(ValueTuple<Tx, Ty, Tz> source) => new(source.Item1, source.Item2, source.Item3);
		public static explicit operator ValueTuple<Tx, Ty, Tz>(Index<Tx, Ty, Tz> source) => ValueTuple.Create(source.X, source.Y, source.Z);


		static (Tx, Ty, Tz) IAdditiveIdentity<Index<Tx, Ty, Tz>, ValueTuple<Tx, Ty, Tz>>.AdditiveIdentity { get; } = (Tx.AdditiveIdentity, Ty.AdditiveIdentity, Tz.AdditiveIdentity);
		static (Tx, Ty, Tz) IMultiplicativeIdentity<Index<Tx, Ty, Tz>, ValueTuple<Tx, Ty, Tz>>.MultiplicativeIdentity { get; } = (Tx.MultiplicativeIdentity, Ty.MultiplicativeIdentity, Tz.MultiplicativeIdentity);



		public static Index<Tx, Ty, Tz> Zero { get; } = (Tx.Zero, Ty.Zero, Tz.Zero);
	}


}
