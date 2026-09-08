using System;

namespace MemoryPools;

public class DisableThreadMemoryPooling : IDisposable
{
	public bool OldEnablePooling { get; }

	public bool ReclaimOnDispose { get; }

	public DisableThreadMemoryPooling()
	{
		OldEnablePooling = MemoryPooling.EnableThreadMemoryPooling;
		MemoryPooling.EnableThreadMemoryPooling = false;
	}

	public void Dispose()
	{
		MemoryPooling.EnableThreadMemoryPooling = OldEnablePooling;
	}
}
