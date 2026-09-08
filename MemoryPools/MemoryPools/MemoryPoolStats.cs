using System;
using System.Collections.Immutable;
using System.Linq;

namespace MemoryPools;

public class MemoryPoolStats
{
	public Type ItemType { get; }

	public int AllocatedSizeKB => (int)(AllocatedItems * ItemSize / 1024);

	public int ItemSize { get; }

	public int ChunksSize { get; }

	public int ChunkSizeKB => ChunksSize * ItemSize / 1024;

	public int AllocatedChunks { get; }

	public long AllocatedItems { get; }

	public long UsedItems { get; }

	public ImmutableList<(int size, int count)> ReturnedItems { get; }

	public int ReturnedItemsCount => ReturnedItems.Sum(((int size, int count) item) => item.count * item.size);

	public MemoryPoolStats(Type itemType, int chunksSize, int itemSize, int allocatedChunks, long allocatedItems, long usedItems, ImmutableList<(int size, int count)> returnedItems)
	{
		ItemType = itemType;
		ChunksSize = chunksSize;
		ItemSize = itemSize;
		AllocatedChunks = allocatedChunks;
		AllocatedItems = allocatedItems;
		UsedItems = usedItems;
		ReturnedItems = returnedItems;
	}
}
