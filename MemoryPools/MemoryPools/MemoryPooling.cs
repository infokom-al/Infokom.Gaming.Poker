using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MemoryPools;

public static class MemoryPooling
{
	private static ThreadLocal<bool> _enableThreadMemoryPooling = new ThreadLocal<bool>(() => false);

	public const bool EnableMemoryPoolingGlobally = true;

	public static ThreadLocal<Stack<MemoryPool>> MemoryPoolStack = new ThreadLocal<Stack<MemoryPool>>(delegate
	{
		Stack<MemoryPool> stack = new Stack<MemoryPool>();
		stack.Push(new MemoryPool());
		return stack;
	});

	public static bool EnableThreadMemoryPooling
	{
		get
		{
			return _enableThreadMemoryPooling.Value;
		}
		set
		{
			_enableThreadMemoryPooling.Value = value;
		}
	}

	public static MemoryPool ThreadMemoryPool => MemoryPoolStack.Value.Peek();

	public static IEnumerable<IMemoryPool> ThreadMemoryPools => ThreadMemoryPool.MemoryPools;

	public static void PushThreadMemoryPool(MemoryPool factory)
	{
		MemoryPoolStack.Value.Push(factory);
	}

	public static void PopThreadMemoryPool(MemoryPool expectedPoppedPool = null)
	{
		MemoryPool memoryPool = MemoryPoolStack.Value.Pop();
		if (expectedPoppedPool != null && memoryPool != expectedPoppedPool)
		{
			throw new InvalidOperationException("Wrong pool was on top of the stack");
		}
	}

	public static int GetItemSize<T>()
	{
		return Unsafe.SizeOf<T>();
	}

	public static int GetChunkSize<T>()
	{
		return 1048576.GetNumberOfBuckets(GetItemSize<T>()).GetPowerOfTwoGreaterOfEqual();
	}

	public static void VerifyThreadPoolDisabled()
	{
		if (EnableThreadMemoryPooling)
		{
			throw new InvalidOperationException("Memory pool must be disabled at this moment.");
		}
	}
}
