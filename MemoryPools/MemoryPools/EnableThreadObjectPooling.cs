using System;

namespace MemoryPools;

public struct EnableThreadObjectPooling : IDisposable
{
	public bool OldEnableThreadPooling { get; } = Pooling.EnableThreadObjectPooling;

	public bool ReclaimOnDispose { get; }

	public EnableThreadObjectPooling()
		: this(reclaimOnDispose: false)
	{
	}

	public EnableThreadObjectPooling(bool reclaimOnDispose)
	{
		Pooling.EnableThreadObjectPooling = true;
		ReclaimOnDispose = reclaimOnDispose;
	}

	public void Dispose()
	{
		Pooling.EnableThreadObjectPooling = OldEnableThreadPooling;
		if (ReclaimOnDispose)
		{
			Pooling.ReclaimAllThreadSharedPools();
		}
	}
}
