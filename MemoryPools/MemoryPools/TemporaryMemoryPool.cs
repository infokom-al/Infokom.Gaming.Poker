using System;

namespace MemoryPools;

public class TemporaryMemoryPool : IDisposable
{
	public MemoryPool InnerPool { get; }

	public bool ReclaimOnDispose { get; }

	public EnableThreadMemoryPooling EnableThreadMemoryPooling { get; private set; }

	public TemporaryMemoryPool(MemoryPool memoryPool, bool reclaimOnDispose = false)
	{
		InnerPool = memoryPool;
		MemoryPooling.PushThreadMemoryPool(memoryPool);
		ReclaimOnDispose = reclaimOnDispose;
		EnableThreadMemoryPooling = new EnableThreadMemoryPooling();
	}

	public void Reclaim()
	{
		InnerPool.MemoryPools.ReclaimAll();
	}

	public void Clear()
	{
		InnerPool.MemoryPools.ClearAll();
	}

	public void Dispose()
	{
		if (ReclaimOnDispose)
		{
			Reclaim();
		}
		EnableThreadMemoryPooling.Dispose();
		MemoryPooling.PopThreadMemoryPool(InnerPool);
	}
}
