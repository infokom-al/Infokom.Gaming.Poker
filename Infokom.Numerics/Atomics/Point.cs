using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Infokom.Numerics.Atomics
{

	

	public struct Scalar<T> where T : unmanaged, IBinaryNumber<T>
	{






	}

	public readonly record struct Point<Tx>(Tx X) where Tx : unmanaged, IBinaryNumber<Tx>
	{
		
		/// <summary>(0)</summary>
		public static readonly Point<Tx> Zero = new(Tx.Zero);
		/// <summary>(1)</summary>
		public static readonly Point<Tx> Unit = new(Tx.One);
	}

	public readonly record struct Point<Tx, Ty>(Tx X, Ty Y) where Tx : unmanaged, IFloatingPoint<Tx> where Ty : unmanaged, IFloatingPoint<Ty>
	{
		/// <summary>(0,0)</summary>
		public static readonly Point<Tx, Ty> Zeros = new(Tx.Zero, Ty.Zero);
		/// <summary>(1,1)</summary>
		public static readonly Point<Tx, Ty> Units = new(Tx.One, Ty.Zero);

		/// <summary>(1,0)</summary>
		public static readonly Point<Tx, Ty> UnitX = new(Tx.One, Ty.Zero);

		/// <summary>(0,1)</summary>
		public static readonly Point<Tx, Ty> UnitY = new(Tx.Zero, Ty.One);

	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="Tx"></typeparam>
	/// <typeparam name="Ty"></typeparam>
	/// <typeparam name="Tz"></typeparam>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <param name="Z"></param>
	public readonly record struct Point<Tx, Ty, Tz>(Tx X, Ty Y, Tz Z) where Tx : unmanaged, IFloatingPoint<Tx> where Ty : unmanaged, IFloatingPoint<Ty> where Tz : unmanaged, IFloatingPoint<Tz>
	{
		/// <summary>(0,0,0)</summary>
		public static readonly Point<Tx, Ty, Tz> Zeros = new(Tx.Zero, Ty.Zero, Tz.Zero);

		/// <summary>(1,1,1)</summary>
		public static readonly Point<Tx, Ty, Tz> Units = new(Tx.One, Ty.One, Tz.One);

		/// <summary>(1,0,0)</summary>
		public static readonly Point<Tx, Ty, Tz> UnitX = new(Tx.One, Ty.Zero, Tz.Zero);

		/// <summary>(0,1,0)</summary>
		public static readonly Point<Tx, Ty, Tz> UnitY = new(Tx.Zero, Ty.One, Tz.Zero);

		/// <summary>(0,0,1)</summary>
		public static readonly Point<Tx, Ty, Tz> UnitZ = new(Tx.Zero, Ty.Zero, Tz.One);
	}



	public static class Point
	{

	}

}
