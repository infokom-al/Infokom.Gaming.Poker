using System.Numerics;

namespace Infokom.Numerics.Atomics
{
	public readonly struct Range<Tx> where Tx : unmanaged, INumber<Tx>, IMinMaxValue<Tx>
	{

		private Range(Point<Tx> low, Point<Tx> upp) => (this.LowerBound, this.UpperBound) = (low, upp);

		public Point<Tx> LowerBound { get; }

		public Point<Tx> UpperBound { get; }

		public bool IsEmpty => LowerBound == UpperBound;

		/// <summary>
		/// [<see cref="Point{Tx, Ty, Tz}.Zeros">0³</see> ... <see cref="Point{Tx, Ty, Tz}.Units">0³</see>]
		/// </summary>
		public static readonly Range<Tx> Zero;

		/// <summary>
		/// [<see cref="Point{Tx, Ty, Tz}.Zeros">0³</see> - <see cref="Point{Tx, Ty, Tz}.Units">1³</see>]
		/// </summary>
		public static readonly Range<Tx> Unit = new(Point<Tx>.Zero, Point<Tx>.Unit);

		public static readonly Range<Tx> All = new(new(Tx.MinValue), new(Tx.MaxValue));
	}

	public readonly struct Range<Tx, Ty>
		where Tx : unmanaged, INumber<Tx>
		where Ty : unmanaged, INumber<Ty>
	{

		private Range(Point<Tx, Ty> low, Point<Tx, Ty> upp) => (this.LowerBound, this.UpperBound) = (low, upp);

		public Point<Tx, Ty> LowerBound { get; }

		public Point<Tx, Ty> UpperBound { get; }

		/// <summary>
		/// [<see cref="Point{Tx, Ty, Tz}.Zeros">0³</see> ... <see cref="Point{Tx, Ty, Tz}.Units">0³</see>]
		/// </summary>
		public static readonly Range<Tx, Ty> Zero;

	}

	public readonly struct Range<Tx, Ty, Tz> 
		where Tx : unmanaged, INumber<Tx>
		where Ty : unmanaged, INumber<Ty>
		where Tz : unmanaged, INumber<Tz>
	{

		private Range(Point<Tx, Ty, Tz> low, Point<Tx, Ty, Tz> upp) => (this.LowerBound, this.UpperBound) = (low, upp);

		public Point<Tx, Ty, Tz> LowerBound { get; }

		public Point<Tx, Ty, Tz> UpperBound { get; }

		public bool IsEmpty => this.LowerBound == this.UpperBound;
	}
}
