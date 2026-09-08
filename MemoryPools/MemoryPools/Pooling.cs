using System;
using System.Collections.Generic;
using System.Threading;

namespace MemoryPools;

public static class Pooling
{
	public static ThreadLocal<bool> _enableThreadObjectPooling = new ThreadLocal<bool>(() => false);

	public const bool EnableObjectPoolingGlobally = true;

	public static ThreadLocal<bool> _throwIfMissingPooledObject = new ThreadLocal<bool>();

	private static ThreadLocal<List<IPool>> _threadSharedPools = new ThreadLocal<List<IPool>>(() => new List<IPool>());

	public static bool EnableThreadObjectPooling
	{
		get
		{
			return _enableThreadObjectPooling.Value;
		}
		set
		{
			_enableThreadObjectPooling.Value = value;
		}
	}

	public static List<string> AllPools { get; } = new List<string>();

	public static bool ThrowIfMissingPooledObject
	{
		get
		{
			return _throwIfMissingPooledObject.Value;
		}
		set
		{
			_throwIfMissingPooledObject.Value = value;
		}
	}

	public static List<IPool> ThreadSharedPools => _threadSharedPools.Value;

	public static string PrintAllPools()
	{
		return AllPools.AggregateToString("\n");
	}

	public static void ClearAllThreadSharedPools()
	{
		ThreadSharedPools.ForEach(delegate(IPool pool)
		{
			pool.Clear();
		});
	}

	public static void ReclaimAllThreadSharedPools()
	{
		ThreadSharedPools.ReclaimAll();
	}

	public static void ReturnToThreadSharedPool<T>(this T[] array)
	{
		ArrayPool<T>.ThreadShared.Return(array);
	}

	public static void ReturnToThreadSharedPool<T>(this T @object)
	{
		ObjectPool<T>.ThreadShared.Return(@object);
	}

	public static void WithThreadPoolEnabled(Action action)
	{
		EnableThreadObjectPooling = true;
		try
		{
			action();
		}
		finally
		{
			EnableThreadObjectPooling = false;
		}
	}

	public static void VerifyThreadObjectPoolingDisabled()
	{
		if (EnableThreadObjectPooling)
		{
			throw new InvalidOperationException("Thread object pooling must be disabled at this moment");
		}
	}
}
