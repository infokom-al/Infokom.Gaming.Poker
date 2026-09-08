using Infokom.Numerics.Atomics;

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

using static Infokom.Gaming.Poker.CardExtensions;

namespace Infokom.Gaming.Poker
{
	// $R=\{2,3,4,5,6,7,8,9,T,J,Q,K,A\}$
	public enum Rank : sbyte
	{
		Two = 2,
		Three = 3,
		Four = 4,
		Five = 5,
		Six = 6,
		Seven = 7,
		Eight = 8,
		Nine = 9,
		Ten = 10,
		Jack = 11,
		Queen = 12,
		King = 13,
		Ace = 14,

		MIN = Two,
		MAX = Ace,
	}


	public static class RankExtensions
	{

		private static void Foo()
		{
		}

		private const Rank T = (Rank)(sbyte)0xA, J = (Rank)(sbyte)0xB, Q = (Rank)(sbyte)0xC, K = (Rank)(sbyte)0xD, A = (Rank)(sbyte)0xE;



		private const Rank MIN = (Rank)0x2;
		private const Rank MAX = (Rank)0xE;

		private static readonly Comparison<Rank> INCREMENTAL = (a, b) => ((sbyte)a).CompareTo((sbyte)b);
		private static readonly Comparison<Rank> DECREMENTAL = (a, b) => ((sbyte)b).CompareTo((sbyte)a);



		extension(Rank)
		{
			#region CAST AND PARSE

			/// <summary>
			/// Get the <see cref="Rank">rank</see> having a provided <see cref="char">character</see> as symbol
			/// </summary>
			/// <param name="source"></param>
			/// <param name="target"></param>
			/// <returns>
			/// <see langword="true"/> if <paramref name="target"/> .Symbol == <paramref name="source"/>, <see langword="false"/> if no well known <see cref="Rank"/> value having <paramref name="source"/> as symbol was found.
			/// </returns>
			public static bool TryCast(in char source, out Rank target)
			{
				switch (source)
				{
					case >= '2' and <= '9': target = (Rank)(source - '0'); return true;
					case 'T': target = T; return true;
					case 'J': target = J; return true;
					case 'Q': target = Q; return true;
					case 'K': target = K; return true;
					case 'A': target = A; return true;
					default: target = default; return false;
				}
			}


			/// <summary>
			/// Get the <see cref="Rank">rank</see> having a provided <see cref="char">character</see> as symbol
			/// </summary>
			/// <param name="source"></param>
			/// <returns></returns>
			/// <exception cref="InvalidCastException"></exception>
			/// <remarks>
			/// It is expected that <see cref="Cast(in char)"/> (<paramref name="source"/>).<see cref="get_Symbol(in Rank)">Symbol</see> == <paramref name="source"/>
			/// </remarks>
			public static Rank Cast(in char source) => source switch
			{
				>= '2' and <= '9' => (Rank)(source - '0'),
				'T' => T,
				'J' => J,
				'Q' => Q,
				'K' => K,
				'A' => A,
				_ => throw new InvalidCastException($"Cannot cast from {source}.")
			};

			/// <summary>
			/// Get the <see cref="Rank">rank</see> having a provided <see cref="char">character</see> as symbol or return a default value if no well known <see cref="Rank"/> value having <paramref name="symbol"/> as symbol was found.
			/// </summary>
			/// <param name="symbol"></param>
			/// <param name="defaultResult"></param>
			/// <returns>
			/// A well known <see cref="Rank">rank</see> having <paramref name="symbol"/> as symbol or <paramref name="defaultResult"/> nothing was found. 
			/// </returns>
			/// <remarks>
			/// In some scenarios, throwing an exception is not desirable. This method provides a way to safely cast
			/// by delegating the failure handling to the caller, allowing them to specify a default result.
			/// </remarks>	
			public static Rank Cast(in char symbol, Rank defaultResult) => symbol switch
			{
				>= '2' and <= '9' => (Rank)(symbol - '0'),
				'T' => T,
				'J' => J,
				'Q' => Q,
				'K' => K,
				'A' => A,
				_ => defaultResult
			};


			/// <summary>
			/// Parse a <see cref="ReadOnlySpan{char}"/> as a <see cref="Rank"/>.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="target"></param>
			/// <returns>
			/// <see langword="true"/> if the <paramref name="target"/> was assigned bny parsing the <paramref name="source"/> or false if the parse failed.
			/// </returns>
			public static bool TryParse(ReadOnlySpan<char> source, out Rank target)
			{
				if (source.Length != 1)
				{
					target = default;
					return false;
				}

				return Rank.TryCast(source[0], out target);
			}

			/// <summary>
			/// Parse a <see cref="ReadOnlySpan{char}"/> as a <see cref="Rank"/>.
			/// </summary>
			/// <param name="source">The span to be parsed</param>
			/// <returns><see cref="Rank"/> value</returns>
			/// <exception cref="FormatException">
			/// Content of <paramref name="source"/> has an invalid format; i.e. <paramref name="source"/> has not exactly one element or the cast of that element failed.
			/// </exception>
			public static Rank Parse(ReadOnlySpan<char> source) => source.Length == 1 ? Rank.Cast(source[0]) : throw new FormatException($"Cannot parse from {source}");
			#endregion






			#region ARITHMETICS

			/// <summary>
			/// Lowest well known <see cref="Rank">rank</see>
			/// </summary>
			public static Rank MinValue => MIN;

			/// <summary>
			/// Upmost well known <see cref="Rank">rank</see>
			/// </summary>
			public static Rank MaxValue => MAX;




			/// <summary>
			/// Find the upmost of two given <see cref="Rank">ranks</see>
			/// </summary>
			/// <param name="a">left operand</param>
			/// <param name="b">right operand</param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank Min(Rank a, Rank b) => a <= b ? a : b;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank Min(Rank a, Rank b, Rank c) => a <= b ? (a <= c ? a : c) : (b <= c ? b : c);


			/// <summary>
			/// </summary>
			/// <param name="span"></param>
			/// <returns>The lowest rank if <paramref name="span"/> is not empty, otherwise the smallest value greater then <see cref=Rank.MAX">maximum</see> the well known rank</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank Min(params ReadOnlySpan<Rank> span)
			{
				var y = MAX + 1;

				foreach (var x in span)
					if (x < y)
						y = x;

				return y;
			}





			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank Max(Rank a, Rank b) => a >= b ? a : b;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank Max(Rank a, Rank b, Rank c) => a >= b ? (a >= c ? a : c) : (b >= c ? b : c);


			/// <summary>
			/// </summary>
			/// <param name="span"></param>
			/// <returns>The upmost rank found in the <paramref name="span">span</paramref> if it is not empty, otherwise the upmost value lower then <see cref=Rank.MAX">minimum</see> well known rank</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Rank Max(params ReadOnlySpan<Rank> span)
			{
				var y = (Rank)sbyte.MinValue;

				foreach (var x in span)
					if (x > y)
						y = x;

				return y;
			}



			/// <summary>
			/// Clamp a <see cref="Rank">rank</see> value between provided bounds.
			/// </summary>
			/// <param name="ρ">Value to be clamped</param>
			/// <param name="low">Lower bound</param>
			/// <param name="upp">Upper bound</param>
			/// <returns></returns>
			/// <exception cref="ArgumentException">If <paramref name="low"/> > <paramref name="upp"/></exception>
			public static Rank Clamp(Rank ρ, Rank low = Rank.Two, Rank upp = Rank.Ace) => low <= upp ? (ρ < low ? low : ρ > upp ? upp : ρ) : throw new ArgumentException($"{low} = min ≰ max = {upp}");

			#endregion


			#region EQUALITY AND COMAPRISON

			public static int operator +(Rank r) => +(sbyte)r;
			public static int operator -(Rank r) => -(sbyte)r;

			public static int operator +(int x, Rank y) => x + (sbyte)y;
			public static int operator -(int x, Rank y) => x - (sbyte)y;



			public static bool operator <(Rank x, int y) => +x < y;
			public static bool operator >(Rank x, int y) => +x > y;
			public static bool operator <=(Rank x, int y) => +x <= y;
			public static bool operator >=(Rank x, int y) => +x >= y;
			public static bool operator ==(Rank x, int y) => +x == y;
			public static bool operator !=(Rank x, int y) => +x != y;

			public static bool operator <(int x, Rank y) => x < +y;
			public static bool operator >(int x, Rank y) => x > +y;
			public static bool operator <=(int x, Rank y) => x <= +y;
			public static bool operator >=(int x, Rank y) => x >= +y;
			public static bool operator ==(int x, Rank y) => x == +y;
			public static bool operator !=(int x, Rank y) => x != +y;

			

			#endregion


		}


		extension(in Rank source)
		{
			public char Symbol => source switch
			{
				Rank.Two => '2',
				Rank.Three => '3',
				Rank.Four => '4',
				Rank.Five => '5',
				Rank.Six => '6',
				Rank.Seven => '7',
				Rank.Eight => '8',
				Rank.Nine => '9',
				Rank.Ace => 'A',
				Rank.King => 'K',
				Rank.Queen => 'Q',
				Rank.Jack => 'J',
				Rank.Ten => 'T',
				_ => '\0'
			};
		}





		extension<T>(T) where T : unmanaged, IShiftOperators<T, int, T>
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T operator <<(T x, Rank y) => (x << (int)y);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static T operator >>(T x, Rank y) => (x << (int)y);
		}



		extension<T>(IList<T> source)
		{
			public T this[Rank index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source[(int)index];

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				set => source[(int)index] = value;
			}
		}

		extension<T>(Span<T> source)
		{
			public T this[Rank index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source[(int)index];

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				set => source[(int)index] = value;
			}
		}

		extension<T>(ReadOnlySpan<T> source)
		{
			public T this[Rank index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source[(int)index];
			}
		}


		extension(BitMatrix16x16 source)
		{

			
			public bool this[Rank row, Rank col]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => source[(int)row, (int)col];
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				set => source[(int)row, (int)col] = value;
			}
		}
	}
}
