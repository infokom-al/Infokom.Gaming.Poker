using Infokom.Numerics.Atomics;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{


	public static class Functions
	{
#pragma warning disable IDE1006 // Naming Styles

		extension<T>(T) where T : INumber<T>
		{
			public static T Σ(params T[] x)
			{
				T sum = T.Zero;
				foreach (var value in x)
					sum += value;
				return sum;
			}




			/// <summary>
			/// Returns the absolute value of a number.
			/// </summary>
			/// <param name="x">The number to get the absolute value of.</param>
			/// <returns>|<paramref name="x"/>|</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T abs(T x) => T.IsNegative(x) ? -x : x;


			/// <summary>
			/// Returns the minimum of two values.
			/// </summary>
			/// <param name="x">The first value to compare.</param>
			/// <param name="y">The second value to compare.</param>
			/// <returns>x if x &lt; y, otherwise y.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T min(T x, T y) => T.Min(x, y);

			/// <summary>
			/// Returns the minimum of three values.
			/// </summary>
			/// <param name="x">The first value to compare.</param>
			/// <param name="y">The second value to compare.</param>
			/// <param name="z">The third value to compare.</param>
			/// <returns>x if x &lt; y &amp;&amp; x &lt; z, y if y &lt; z, otherwise z.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T min(T x, T y, T z) => T.Min(T.Min(x, y), z);

			/// <summary>
			/// Returns the maximum of two values.
			/// </summary>
			/// <param name="x">The first value to compare.</param>
			/// <param name="y">The second value to compare.</param>
			/// <returns>x if x &gt; y, otherwise y.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T max(T x, T y) => T.Max(x, y);

			/// <summary>
			/// Returns the maximum of three values.
			/// </summary>
			/// <param name="x">The first value to compare.</param>
			/// <param name="y">The second value to compare.</param>
			/// <param name="z">The third value to compare.</param>
			/// <returns>x if x &gt; y &amp;&amp; x &gt; z, y if y &gt; z, otherwise z.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T max(T x, T y, T z) => T.Max(T.Max(x, y), z);


			/// <summary>
			/// Clamp a value between a minimum and maximum value.
			/// </summary>
			/// <param name="x">The value to clamp.</param>
			/// <param name="a">The minimum value.</param>
			/// <param name="b">The maximum value.</param>
			/// <returns>x if a ⩽ x ⩽ b, a if x &lt; a, b if x &gt; b</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T clamp(T x, T a, T b) => T.Clamp(x, a, b);


		}


		extension<T>(T) where T : IRootFunctions<T>
		{
			/// <summary>
			/// Returns the square root of a number.
			/// </summary>
			/// <param name="x">The value to calculate the square root of.</param>
			/// <returns>√x</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T sqrt(T x) => T.Sqrt(x);

			/// <summary>
			/// Returns the cube root of a number.
			/// </summary>
			/// <param name="x">The value to calculate the cube root of.</param>
			/// <returns>∛x</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T cbrt(T x) => T.Cbrt(x);

			/// <summary>∜<paramref name="x"/></summary>½
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T qbrt(T x) => T.Sqrt(T.Sqrt(x));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T hypot(T x, T y) => T.Hypot(x, y);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T rootn(T x, int n) => T.RootN(x, n);
		}

		extension<T>(T) where T : ITrigonometricFunctions<T>
		{
			/// <summary>
			/// Sine trigonometric function.
			/// </summary>
			/// <param name="x">The value to calculate the sine of.</param>
			/// <returns>sin(x)</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T sin(T ϑ) => T.Sin(ϑ);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T cos(T ϑ) => T.Cos(ϑ);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T tan(T ϑ) => T.Tan(ϑ);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T asin(T ϑ) => T.Asin(ϑ);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T acos(T ϑ) => T.Acos(ϑ);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T atan(T x) => T.Atan(x);

			/// <summary>
			/// Computes the arctangent of a number and returns the angle in radians.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="θ"></param>
			public static void atan(in T x, out T θ) => θ = T.Atan(x);
		}


		extension<T>(T) where T : IFloatingPoint<T>
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T round(T x) => T.Round(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T floor(T x) => T.Floor(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T ceil(T x) => T.Ceiling(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T truncate(T x) => T.Truncate(x);
		}

		extension<T>(T) where T : IFloatingPointIeee754<T>
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T exp(T x) => T.Exp(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T log(T x) => T.Log(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T log2(T x) => T.Log2(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T log10(T x) => T.Log10(x);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T atan2(T y, T x) => T.Atan2(y, x);
		}

		extension<T>(T) where T : unmanaged, INumber<T>
		{
			/// <summary>
			/// Check if a number is non-negative (i.e., greater than or equal to zero).
			/// </summary>
			/// <param name="x">The number to check.</param>
			/// <returns><see langword="true"/> if: <code><paramref name="x"/> ≮ 0 ≡ <paramref name="x"/> ≥ 0</code> <see langword="false"/> otherwise.</returns>
			public static bool IsNotNegative(T x) => !(x < T.Zero);

			/// <summary>
			/// Check if a number is non-positive (i.e., less than or equal to zero).
			/// </summary>
			/// <param name="x">The number to check.</param>
			/// <returns><code><paramref name="x"/> ≯ 0 ≡ <paramref name="x"/> ≤ 0</code></returns>
			public static bool IsNotPositive(T x) => !(x > T.Zero);

			/// <summary>
			/// Check if a number is not zero (i.e., not equal to zero).
			/// </summary>
			/// <param name="x">The number to check.</param>
			/// <returns><code><paramref name="x"/> ≠ 0</code></returns>
			public static bool IsNotZero(T x) => !(x == T.Zero);

			/// <summary>
			/// Check if a number is a non-negative integer (i.e., an integer greater than or equal to zero).
			/// </summary>
			/// <param name="x">The number to check.</param>
			/// <returns><code><paramref name="x"/> ∈ ℕ and <paramref name="x"/> ≥ 0</code></returns>
			public static bool IsNonNegativeInteger(T x) => T.IsInteger(x) && T.IsPositive(x);

			/// <summary>
			/// Check if a number is a non-positive integer (i.e., an integer less than or equal to zero).
			/// </summary>
			/// <param name="x">The number to check.</param>
			/// <returns><see langword="true"/> if: <code><paramref name="x"/> ≯ 0 ≡ <paramref name="x"/> ≤ 0</code> <see langword="false"/> otherwise.</returns>
			public static bool IsNonPositiveInteger(T x) => T.IsInteger(x) && T.IsNegative(x);


			//tex: If $X$ is a set of $n$ elements, and ${\cal{P}}(X)$ the power set of $X$, then:
			//$$\binom{X}{k} = \{ C_{k} | C_k \in {\cal{P}}(X), |C_{k}| = k \}$$
			//is the family of $k$-element subsets of $X$.
			//The the number of $k$-element subsets of an $n$-element set is given by the binomial coefficient:
			//$$|\binom{X}{k}| = \binom{|X|}{k} =  \frac{|X|!}{k!(|X|-k)!}$$
			[MethodImpl(MethodImplOptions.AggressiveInlining)]

			public static T choose(T n, T k)
			{
				if(!T.IsInteger(n) || !T.IsPositive(n) || !T.IsInteger(k))
				{
					
				}
				else
				{
					throw new ArgumentException("Both n and k must be integers.");
				}

				var Cnk = T.One;

				if (n > T.Zero)
				{
					k = T.Clamp(k, T.Zero, n);
					if (n - k > T.Zero)
					{
						if (k > n - k) k = n - k;

						for (var i = T.One; i <= k; i++)
						{
							Cnk = (Cnk * (n + T.One - i)) / i;//NOTE: is not equivalent to C *= (n + T.One - i) / i, because of integer division truncation.
						}
					}
				}

				return Cnk;
			}
		}
	}
#pragma warning restore IDE1006 // Naming Styles


	public static class Number
	{

	}
}