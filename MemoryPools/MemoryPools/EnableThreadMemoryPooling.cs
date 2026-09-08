using System;

namespace MemoryPools;

public class EnableThreadMemoryPooling : IDisposable
{
	public bool OldEnablePooling { get; }

	public bool ReclaimOnDispose { get; }

	public EnableThreadMemoryPooling()
	{
		OldEnablePooling = MemoryPooling.EnableThreadMemoryPooling;
		MemoryPooling.EnableThreadMemoryPooling = true;
	}

	public void Dispose()
	{
		MemoryPooling.EnableThreadMemoryPooling = OldEnablePooling;
	}
}
