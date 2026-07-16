using System.Numerics;
using System.Runtime.CompilerServices;

namespace Holdem.Core.Extensions
{

	internal static class Tuples
	{
		#region 2-uple
		extension<TScalar>((TScalar X, TScalar Y)) where TScalar : IAdditionOperators<TScalar, TScalar, TScalar> 
		{
			public static (TScalar X, TScalar Y) operator +((TScalar X, TScalar Y) v, (TScalar X, TScalar Y) w) => (v.X + w.X, v.Y + w.Y);
		}

		extension<TScalar>((TScalar X, TScalar Y)) where TScalar : ISubtractionOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y) operator -((TScalar X, TScalar Y) v, (TScalar X, TScalar Y) w) => (v.X - w.X, v.Y - w.Y);
		}

		extension<TScalar>(ValueTuple<TScalar, TScalar>) where TScalar : IMultiplyOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y) operator *((TScalar X, TScalar Y) v, TScalar k) => (v.X * k, v.Y * k);
		}

		extension<TScalar>(ValueTuple<TScalar, TScalar>) where TScalar : IDivisionOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y) operator /((TScalar X, TScalar Y) v, TScalar k) => (v.X / k, v.Y / k);
		}
		#endregion


		#region 3-uple
		extension<TScalar>((TScalar X, TScalar Y, TScalar Z)) where TScalar : IAdditionOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y, TScalar Z) operator +((TScalar X, TScalar Y, TScalar Z) v, (TScalar X, TScalar Y, TScalar Z) w) => (v.X + w.X, v.Y + w.Y, v.Z + w.Z);
		}

		extension<TScalar>((TScalar X, TScalar Y, TScalar Z)) where TScalar : ISubtractionOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y, TScalar Z) operator -((TScalar X, TScalar Y, TScalar Z) v, (TScalar X, TScalar Y, TScalar Z) w) => (v.X - w.X, v.Y - w.Y, v.Z - w.Z);
		}

		extension<TScalar>((TScalar X, TScalar Y, TScalar Z)) where TScalar : IMultiplyOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y, TScalar Z) operator *((TScalar X, TScalar Y, TScalar Z) v, TScalar k) => (v.X * k, v.Y * k, v.Z * k);
		}

		extension<TScalar>((TScalar X, TScalar Y, TScalar Z)) where TScalar : IDivisionOperators<TScalar, TScalar, TScalar>
		{
			public static (TScalar X, TScalar Y, TScalar Z) operator /((TScalar X, TScalar Y, TScalar Z) v, TScalar k) => (v.X / k, v.Y / k, v.Z / k);
		}
		#endregion








		public static T[] ToArray<T>(this ValueTuple<T> source) => [source.Item1];
		public static T[] ToArray<T>(this ValueTuple<T, T> source) => [source.Item1, source.Item2];
		public static T[] ToArray<T>(this ValueTuple<T, T, T> source) => [source.Item1, source.Item2, source.Item3];
		public static T[] ToArray<T>(this ValueTuple<T, T, T, T> source) => [source.Item1, source.Item2, source.Item3, source.Item4];
		public static T[] ToArray<T>(this ValueTuple<T, T, T, T, T> source) => [source.Item1, source.Item2, source.Item3, source.Item4, source.Item5];
		public static T[] ToArray<T>(this ValueTuple<T, T, T, T, T, T> source) => [source.Item1, source.Item2, source.Item3, source.Item4, source.Item5, source.Item6];
		public static T[] ToArray<T>(this ValueTuple<T, T, T, T, T, T, T> source) => [source.Item1, source.Item2, source.Item3, source.Item4, source.Item5, source.Item6, source.Item7];
	}
}
