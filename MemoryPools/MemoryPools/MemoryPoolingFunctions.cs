using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MemoryPools;

public static class MemoryPoolingFunctions
{
	public static void ReturnToThreadMemoryPool<T>(this Memory<T> memory)
	{
		MemoryPool<T>.ThreadShared.Return(memory);
	}

	public static Memory<T> ReplaceFromPool<T>(this Memory<T> memory, int newSize)
	{
		Memory<T> memory2 = MemoryPool<T>.ThreadShared.GetMemory(newSize);
		if (memory.Length == 0)
		{
			return memory2;
		}
		memory.CopyTo(memory2);
		memory.ReturnToThreadMemoryPool();
		return memory2;
	}

	public static Memory<T> ExpandFromPool<T>(this Memory<T> memory, int newSize)
	{
		Memory<T> memory2 = MemoryPool<T>.ThreadShared.GetMemory(newSize);
		memory.CopyTo(memory2);
		memory.ReturnToThreadMemoryPool();
		return memory2;
	}

	public static Memory<T> CloneFromPool<T>(this Memory<T> memory)
	{
		if (memory.IsEmpty)
		{
			return Memory<T>.Empty;
		}
		Memory<T> memory2 = MemoryPool<T>.ThreadShared.GetMemory(memory.Length);
		memory.CopyTo(memory2);
		return memory2;
	}

	public static int GetSizeOf<T>()
	{
		return Unsafe.SizeOf<T>();
	}

	public static MemoryPoolStats GetMemoryPoolStats<T>(this MemoryPool<T> pool)
	{
		return new MemoryPoolStats(typeof(T), MemoryPool<T>.ChunkSize, GetSizeOf<T>(), pool.Buffer.ChunkCount, pool.Buffer.AllocatedLength, pool.UsedItemCount, pool.ReturnedMemory.MapToImmutableList<KeyValuePair<int, Stack<Memory<T>>>, (int, int)>((KeyValuePair<int, Stack<Memory<T>>> item) => (size: item.Key, count: item.Value.Count)));
	}

	public static MemoryPoolStats MergeAll(this IEnumerable<MemoryPoolStats> stats)
	{
		List<MemoryPoolStats> list = stats.ToList();
		MemoryPoolStats memoryPoolStats = list[0];
		foreach (MemoryPoolStats item in list.Skip(1))
		{
			memoryPoolStats = memoryPoolStats.Merge(item);
		}
		return memoryPoolStats;
	}

	public static MemoryPoolStats Merge(this MemoryPoolStats stats, MemoryPoolStats other)
	{
		ImmutableDictionary<int, int> immutableDictionary = stats.ReturnedItems.ToImmutableDictionary();
		foreach (var returnedItem in other.ReturnedItems)
		{
			int item = returnedItem.size;
			int item2 = returnedItem.count;
			immutableDictionary = immutableDictionary.AddOrIncrement(item, item2);
		}
		if (!(stats.ItemType == other.ItemType))
		{
			throw new InvalidOperationException("Can't merge stats of different pool types");
		}
		Type itemType = stats.ItemType;
		if (stats.ChunksSize != other.ChunksSize)
		{
			throw new InvalidOperationException("Chunks sizes should be equal");
		}
		int chunksSize = stats.ChunksSize;
		if (stats.ItemSize != other.ItemSize)
		{
			throw new InvalidOperationException("Item sizes should be equal");
		}
		return new MemoryPoolStats(itemType, chunksSize, stats.ItemSize, stats.AllocatedChunks + other.AllocatedChunks, stats.AllocatedItems + other.AllocatedItems, stats.UsedItems + other.UsedItems, immutableDictionary.OrderBy((KeyValuePair<int, int> keyValuePair) => keyValuePair.Key).MapToImmutableList((KeyValuePair<int, int> keyValuePair) => (size: keyValuePair.Key, count: keyValuePair.Value)));
	}

	public static void ReclaimAll(this IEnumerable<IMemoryPool> pools)
	{
		foreach (IMemoryPool pool in pools)
		{
			pool.Reclaim();
		}
	}

	public static void ClearAll(this IEnumerable<IMemoryPool> pools)
	{
		foreach (IMemoryPool pool in pools)
		{
			pool.Clear();
		}
	}

	public static void DeallocatePartialAll(this IEnumerable<IMemoryPool> pools, double remainingPart)
	{
		pools.ForEach(delegate(IMemoryPool pool)
		{
			pool.DeallocatePartial(remainingPart);
		});
	}

	public static Memory<T> ToMemoryFromPool<T>(this Span<T> span)
	{
		if (span.Length == 0)
		{
			return Memory<T>.Empty;
		}
		if (MemoryPool<T>.AllowNonPowerOfTwoChunks)
		{
			Memory<T> memory = MemoryPool<T>.ThreadShared.GetMemory(span.Length);
			span.CopyTo(memory.Span);
			return memory;
		}
		Memory<T> memory2 = MemoryPool<T>.ThreadShared.GetMemory(span.Length.GetPowerOfTwoGreaterOfEqual());
		span.CopyTo(memory2.Span);
		return memory2.Slice(0, span.Length);
	}

	public static string PrintStatsToString(this IEnumerable<IMemoryPool> pools)
	{
		return pools.ToList().PrintStatsToString();
	}

	public static string PrintStatsToString(this IList<IMemoryPool> pools)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder2);
		handler.AppendFormatted(pools.Count);
		handler.AppendLiteral(" thread shared pools");
		stringBuilder2.AppendLine(ref handler);
		int minHeaderWidth = pools.MaxOrDefault((IMemoryPool pool) => pool.GetType().GetGenericArguments().First()
			.GetNameWithGenerics()
			.Length);
		foreach (IGrouping<Type, IMemoryPool> item in from pool in pools
			group pool by pool.GetType())
		{
			List<IMemoryPool> list = item.ToList();
			if (list.Count > 0)
			{
				stringBuilder = stringBuilder.AppendLine(list.Select((IMemoryPool pool) => pool.GetPoolStats()).MergeAll().PrintToString(minHeaderWidth));
			}
		}
		return stringBuilder.ToString();
	}

	public static string WritePoolStatsToConsole(this IList<IMemoryPool> pools)
	{
		string text = pools.PrintStatsToString();
		Console.WriteLine(text);
		return text;
	}

	public static string PrintToString(this MemoryPoolStats stats, int minHeaderWidth)
	{
		return (stats.ItemType.GetNameWithGenerics().AddSpacesToMatchWidth(minHeaderWidth) + ":").AddNameValue("AllocatedSize", string.Concat(str1: (stats.AllocatedSizeKB > 1048576) ? "GB" : ((stats.AllocatedSizeKB > 1024) ? "MB" : "KB"), str0: GetRelevantSize().ToString())).AddNameValue("AllocatedItems", stats.AllocatedItems.Shorten()).AddNameValue("AllocatedChunks", stats.AllocatedChunks.Shorten())
			.AddNameValue("sizeof(T)", stats.ItemSize)
			.AddNameValue("ChunkSizeKB", stats.ChunkSizeKB)
			.AddNameValue("UsedItems", stats.UsedItems.Shorten())
			.AddNameValue("ReturnedItems", stats.ReturnedItemsCount.Shorten() + ": " + stats.ReturnedItems.AggregateToString(((int size, int count) item) => $"{item.size}:{item.count}", ", "))
			.Replace(":, ", ":");
		double GetRelevantSize()
		{
			return Math.Round((stats.AllocatedSizeKB > 1048576) ? ((double)stats.AllocatedSizeKB / 1024.0 / 1024.0) : ((stats.AllocatedSizeKB > 1024) ? ((double)stats.AllocatedSizeKB / 1024.0) : ((double)stats.AllocatedSizeKB)), 1);
		}
	}

	public static TemporaryMemoryPool GetTemporaryThreadMemoryPool(this MemoryPoolScope scope, bool reclaimOnDispose)
	{
		return new TemporaryMemoryPool(scope.ThreadMemoryPool, reclaimOnDispose);
	}

	public static byte[] ToArrayFromPool(this MemoryStream memoryStream, out int length)
	{
		length = memoryStream.Position.VerifyInt();
		if (length == 0)
		{
			return EmptyArray<byte>.SingleItemArray;
		}
		int size = ((length < Array.MaxLength / 2) ? length.GetPowerOfTwoGreaterOfEqual() : Array.MaxLength);
		byte[] array = ArrayPool<byte>.ThreadShared.GetArray(size);
		memoryStream.Position = 0L;
		using MemoryStream destination = new MemoryStream(array);
		memoryStream.CopyTo(destination);
		return array;
	}
}
