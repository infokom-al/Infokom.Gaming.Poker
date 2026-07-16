using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infokom.Numerics
{

	public static class Vector
	{
		public static Vector<T> Create<T>(int size) => Vector<T>.Create(size);

		public static Vector<T> Create<T>(int size, T value) => Vector<T>.Create(size, value);

		public static Vector<T> Create<T>(int size, Func<int, T> source) => Vector<T>.Create(size, source);



		#region addition
		extension<TScalar>(ReadOnlySpan<TScalar>) where TScalar : unmanaged, INumber<TScalar>
		{
			public static TScalar[] operator +(ReadOnlySpan<TScalar> a)
			{
				var n = a.Length;

				var b = new TScalar[n];

				unsafe
				{
					fixed (TScalar* aPtr = &MemoryMarshal.GetReference(a), bPtr = &MemoryMarshal.GetReference(b))
					{
						for (int i = 0; i < n; i++)
						{
							bPtr[i] = +aPtr[i];
						}
					}
				}

				return b;
			}


			public static TScalar[] operator+(ReadOnlySpan<TScalar> v1, ReadOnlySpan<TScalar> v2)
			{
				ArgumentOutOfRangeException.ThrowIfNotEqual(v1.Length, v2.Length, "Array dimension mismatch.");
				
				var n = v1.Length;

				var v = new TScalar[n];

				unsafe
				{
					fixed (TScalar* ptr1 = &MemoryMarshal.GetReference(v1), ptr2 = &MemoryMarshal.GetReference(v2), ptr = &MemoryMarshal.GetReference(v))
					{
						for (int i = 0; i < n; i++)
						{
							ptr[i] = ptr1[i] + ptr2[i];
						}
					}
				}			

				return v;
			}



			public static TScalar[] operator -(ReadOnlySpan<TScalar> a)
			{
				var n = a.Length;

				var b = new TScalar[n];

				unsafe
				{
					fixed (TScalar* aPtr = &MemoryMarshal.GetReference(a), bPtr = &MemoryMarshal.GetReference(b))
					{
						for (int i = 0; i < n; i++)
						{
							bPtr[i] = -aPtr[i];
						}
					}
				}

				return b;
			}



			public static TScalar[] operator -(ReadOnlySpan<TScalar> a, ReadOnlySpan<TScalar> b)
			{
				ArgumentOutOfRangeException.ThrowIfNotEqual(a.Length, b.Length, "Array dimension mismatch.");

				var n = a.Length;

				var c = new TScalar[n];

				unsafe
				{
					fixed (TScalar* aPtr = &MemoryMarshal.GetReference(a), bPtr = &MemoryMarshal.GetReference(b), cPtr = &MemoryMarshal.GetReference(c))
					{
						for (int i = 0; i < n; i++)
						{
							cPtr[i] = aPtr[i] - bPtr[i];
						}
					}
				}


				return c;
			}


			public static TScalar[] operator *(ReadOnlySpan<TScalar> a, TScalar b)
			{
				var n = a.Length;

				var c = new TScalar[n];

				

				unsafe
				{
					fixed (TScalar* aPtr = &MemoryMarshal.GetReference(a), cPtr = &MemoryMarshal.GetReference(c))
					{
						for (int i = 0; i < n; i++)
						{
							cPtr[i] = aPtr[i] * b;
						}
					}
				}

				return c;
			}

			public static TScalar[] operator *(TScalar a, ReadOnlySpan<TScalar> b) => b * a;

			public static TScalar[] operator /(ReadOnlySpan<TScalar> a, TScalar b) => a * (TScalar.One / b);
		}

		#endregion



		#region uint[]

		/// <summary>
		/// Returns the maximum value in the vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Max(this uint[] vector)
		{
			uint max = uint.MinValue;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					uint* pn = p0 + vector.Length;


					uint* pi = p0;
					while (pi < pn)
					{
						if (*pi > max)
							max = *pi;
						pi++;
					}
				}
			}

			return max;
		}

		/// <summary>
		/// Returns the minimum value in the vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Min(this uint[] vector)
		{
			uint min = uint.MaxValue;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					uint* pn = p0 + vector.Length;


					uint* pi = p0;
					while (pi < pn)
					{
						if (*pi < min)
							min = *pi;
						pi++;
					}
				}
			}

			return min;
		}

		/// <summary>
		/// Returns the average value in the vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Avg(this uint[] vector)
		{
			int n = vector.Length;
			uint avg = 0U;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					for (int i = 0; i < n; i++)
					{
						avg += *(p0 + i);
					}
				}
			}

			return avg / (uint)n;
		}

		/// <summary>
		/// Executes the specified action for each element in the vector.
		/// </summary>
		/// <param name="vector"> the vector to iterate over </param>
		/// <param name="action"> the action to execute for each element </param>
		public static void ForEach(this uint[] vector, Action<uint> action)
		{
			ArgumentNullException.ThrowIfNull(vector, nameof(vector));
			ArgumentNullException.ThrowIfNull(action, nameof(action));

			int n = vector.Length;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					for (int i = 0; i < n; i++)
					{
						action(*(p0 + i));
					}
				}
			}
		}

		/// <summary>
		/// Executes the specified action for each element in the vector that satisfies the given predicate.
		/// </summary>
		/// <param name="vector"> the vector to iterate over </param>
		/// <param name="predicate"> the predicate to test each element against </param>
		/// <param name="action"> the action to execute for each element that satisfies the predicate	</param>
		public static void ForEach(this uint[] vector, Func<uint, bool> predicate, Action<uint> action)
		{
			ArgumentNullException.ThrowIfNull(vector, nameof(vector));
			ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
			ArgumentNullException.ThrowIfNull(action, nameof(action));

			int n = vector.Length;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					for (int i = 0; i < n; i++)
					{
						if (predicate(*(p0 + i)))
						{
							action(*(p0 + i));
						}
					}
				}
			}
		}

		/// <summary>
		/// Executes the specified action for each element in the vector, providing the index and value of each element.
		/// </summary>
		/// <param name="vector"> the vector to iterate over </param>
		/// <param name="action"> the action to execute for each element </param>
		public static void ForEach(this uint[] vector, Action<int, uint> action)
		{
			ArgumentNullException.ThrowIfNull(vector, nameof(vector));
			ArgumentNullException.ThrowIfNull(action, nameof(action));

			int n = vector.Length;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					for (int i = 0; i < n; i++)
					{
						action(i, *(p0 + i));
					}
				}
			}
		}

		/// <summary>
		/// Executes the specified action for each element in the vector that satisfies the given predicate.
		/// </summary>
		/// <param name="vector"> the vector to iterate over </param>
		/// <param name="predicate"> the predicate to test each element against </param>
		/// <param name="action"> the action to execute for each element that satisfies the predicate </param>
		public static void ForEach(this uint[] vector, Func<int, uint, bool> predicate, Action<int, uint> action)
		{
			ArgumentNullException.ThrowIfNull(vector, nameof(vector));
			ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
			ArgumentNullException.ThrowIfNull(action, nameof(action));

			int n = vector.Length;

			unsafe
			{
				fixed (uint* p0 = vector)
				{
					for (int i = 0; i < n; i++)
					{
						if (predicate(i, *(p0 + i)))
						{
							action(i, *(p0 + i));
						}
					}
				}
			}
		}

		extension(uint[])
		{
			public static BitArray operator ==(uint[] v, uint k)
			{
				var mask = new BitArray(v.Length);

				var n = v.Length;

				unsafe
				{
					fixed (uint* p0 = v)
					{
						for (int i = 0; i < n; i++)
						{
							var pi = p0 + i;
							if (*pi == k)
								mask[i] = true;
						}
					}
				}

				return mask;
			}

			public static BitArray operator !=(uint[] v, uint k)
			{
				var mask = new BitArray(v.Length);

				var n = v.Length;

				unsafe
				{
					fixed (uint* p0 = v)
					{
						for (int i = 0; i < n; i++)
						{
							var pi = p0 + i;
							if (*pi != k)
								mask[i] = true;
						}
					}
				}

				return mask;
			}


			public static BitArray operator <(uint[] v, uint k)
			{
				var mask = new BitArray(v.Length);

				var n = v.Length;

				unsafe
				{
					fixed (uint* p0 = v)
					{
						for (int i = 0; i < n; i++)
						{
							var pi = p0 + i;
							if (*pi < k)
								mask[i] = true;
						}
					}
				}

				return mask;
			}

			public static BitArray operator <=(uint[] v, uint k)
			{
				var mask = new BitArray(v.Length);

				var n = v.Length;

				unsafe
				{
					fixed (uint* p0 = v)
					{
						for (int i = 0; i < n; i++)
						{
							var pi = p0 + i;
							if (*pi <= k)
								mask[i] = true;
						}
					}
				}

				return mask;
			}

			public static BitArray operator >(uint[] v, uint k)
			{
				var mask = new BitArray(v.Length);

				var n = v.Length;

				unsafe
				{
					fixed (uint* p0 = v)
					{
						for (int i = 0; i < n; i++)
						{
							var pi = p0 + i;
							if (*pi > k)
								mask[i] = true;
						}
					}
				}

				return mask;
			}

			public static BitArray operator >=(uint[] v, uint k)
			{
				var mask = new BitArray(v.Length);

				var n = v.Length;

				unsafe
				{
					fixed (uint* p0 = v)
					{
						for (int i = 0; i < n; i++)
						{
							var pi = p0 + i;
							if (*pi >= k)
								mask[i] = true;
						}
					}
				}

				return mask;
			}
		}
		#endregion



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this BitArray source)
		{
			int setBits = 0;
			foreach (bool bit in source)
			{
				if (bit) setBits++;
			}
			return setBits;
		}


		public static void ForEach(this BitArray vector, Action<int> action)
		{
			ArgumentNullException.ThrowIfNull(vector, nameof(vector));
			ArgumentNullException.ThrowIfNull(action, nameof(action));

			int n = vector.Length;

			for (int i = 0; i < n; i++)
			{
				action(i);
			}
		}
	}


}
