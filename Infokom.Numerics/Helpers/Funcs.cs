using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Infokom.Numerics.Helpers
{
	public interface IFunctor<TSource, TTarget>
	{
		TTarget Invoke(TSource input);
	}

	public static class Func
	{

		


		//tex: If $X$ is a set of $n$ elements, and ${\cal{P}}(X)$ the power set of $X$, then:
		//$$\binom{X}{k} = \{ C_{k} | C_k \in {\cal{P}}(X), |C_{k}| = k \}$$
		//is the family of $k$-element subsets of $X$.
		//The the number of $k$-element subsets of an $n$-element set is given by the binomial coefficient:
		//$$|\binom{X}{k}| = \binom{|X|}{k} =  \frac{|X|!}{k!(|X|-k)!}$$
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Choose<T>(T n, T k) where T : unmanaged, IBinaryInteger<T>
		{
			var C = T.One;

			if (n > T.Zero)
			{
				k = T.Clamp(k, T.Zero, n);	
				if(n - k > T.Zero) 
				{
					if (k > n - k) k = n - k;

					for (var i = T.One; i <= k; i++)
					{
						C = (C * (n + T.One - i)) / i;//NOTE: is not equivalent to C *= (n + T.One - i) / i, because of integer division truncation.
					}
				}
			}

			return C;
		}
	}
}
