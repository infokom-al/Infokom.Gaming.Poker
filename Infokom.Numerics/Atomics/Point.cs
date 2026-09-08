using System.Numerics;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{
	public interface IPoint<TPoint> where TPoint : IPoint<TPoint>
	{
		public static abstract TPoint Midpoint(TPoint a, TPoint b);
	}





	#region p = ⟨x⟩

	public readonly record struct Point<Tx>(Tx X) : IPoint<Point<Tx>> where Tx : unmanaged, INumber<Tx>
	{
		private static readonly Tx X_0 = Tx.Zero;
		private static readonly Tx X_1 = Tx.One;
		private static readonly Tx X_2 = X_1 + X_1;

		/// <summary><c>(x: <see cref="INumberBase{Tx}.Zero">0</see>)</c></summary>
		public static readonly Point<Tx> Zero = new(X_0);

		/// <summary><c>(x: <see cref="INumberBase{Tx}.One">1</see>)</c></summary>
		public static readonly Point<Tx> Unit = new(X_1);

		public static Point<Tx> Midpoint(Point<Tx> a, Point<Tx> b) => new((a.X - b.X) / X_2);



		public static implicit operator Point<Tx>(Tx source) => new(source);
	}

	public static class Point1
	{
		extension<T>(Point<T>) where T : unmanaged, INumber<T>
		{
			public static Point<T> Zero => new(T.Zero);
			public static Point<T> Unit => new(T.One);
		}
	}
	#endregion

	#region p = ⟨x, y⟩
	public readonly record struct Point<Tx, Ty>(Tx X, Ty Y) where Tx : unmanaged where Ty : unmanaged
	{
	}

	public static class Point2
	{

		extension<T>(Point<T, T>) where T : unmanaged, INumber<T>
		{
			public static Point<T, T> Zeros => new(T.Zero, T.Zero);
			public static Point<T, T> Units => new(T.One, T.One);
			public static Point<T, T> UnitX => new(T.One, T.Zero);
			public static Point<T, T> UnitY => new(T.Zero, T.One);
		}
	}
	#endregion

	#region p = ⟨x, y, z⟩
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="Tx"></typeparam>
	/// <typeparam name="Ty"></typeparam>
	/// <typeparam name="Tz"></typeparam>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <param name="Z"></param>
	public readonly record struct Point<Tx, Ty, Tz>(Tx X, Ty Y, Tz Z) where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty> where Tz : unmanaged, INumber<Tz>
	{

	}

	public static class Point3
	{
		
		extension<T>(Point<T, T, T>) where T : unmanaged, INumber<T>
		{
			public static Point<T, T, T> Zeros => new(T.Zero, T.Zero, T.Zero);
			public static Point<T, T, T> Units => new(T.One, T.One, T.One);
			public static Point<T, T, T> UnitX => new(T.One, T.Zero, T.Zero);
			public static Point<T, T, T> UnitY => new(T.Zero, T.One, T.Zero);
			public static Point<T, T, T> UnitZ => new(T.Zero, T.Zero, T.One);
		}
	}
	#endregion

	#region p = ⟨x, y, z, w⟩
	//tex:
	//$p = (x, y, z, w) \in \mathit{R}^4$
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Point<Tx, Ty, Tz, Tw> where Tx : unmanaged where Ty : unmanaged where Tz : unmanaged where Tw : unmanaged
	{
		private readonly Tx _x;
		private readonly Ty _y;
		private readonly Tz _z;
		private readonly Tw _w;

		public Point(Tx x, Ty y, Tz z, Tw w) => (_x, _y, _z, _w) = (x, y, z, w);

		public Tx X => _x;
		public Ty Y => _y;
		public Tz Z => _z;
		public Tw W => _w;
	}

	public static class Point4
	{
		
		extension<T>(Point<T, T, T, T>) where T : unmanaged, INumber<T>
		{
			public static Point<T, T, T, T> Zeros => new(T.Zero, T.Zero, T.Zero, T.Zero);
			public static Point<T, T, T, T> Units => new(T.One, T.One, T.One, T.One);
			public static Point<T, T, T, T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero);
			public static Point<T, T, T, T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero);
			public static Point<T, T, T, T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero);
			public static Point<T, T, T, T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One);
		}
	}
	#endregion



	public static class Point
	{
		



		#region Point<X,Y,Z,W>
		public static Point<T, T, T, T> ToPoint<T>(this ValueTuple<T, T, T, T> source) where T : unmanaged, INumber<T> => new(source.Item1, source.Item2, source.Item3, source.Item4);

		public static void Deconstruct<T>(this Point<T, T, T, T> source, out T x, out T y, out T z, out T w) where T : unmanaged, INumber<T> => (x, y, z, w) = (source.X, source.Y, source.Z, source.W);
		#endregion
	}

	
	
}
