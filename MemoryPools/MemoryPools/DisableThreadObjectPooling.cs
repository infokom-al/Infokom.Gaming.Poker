using System;

namespace MemoryPools;

public struct DisableThreadObjectPooling : IDisposable
{
	public bool OldEnableThreadPooling { get; } = Pooling.EnableThreadObjectPooling;

	public DisableThreadObjectPooling()
	{
		Pooling.EnableThreadObjectPooling = false;
	}

	public void Dispose()
	{
		Pooling.EnableThreadObjectPooling = OldEnableThreadPooling;
	}
}
