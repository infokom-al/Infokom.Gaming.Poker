using System;
using System.Linq;
using System.Threading;

namespace MemoryPools;

public class MemoryPoolScope : IDisposable
{
	public ThreadLocal<MemoryPool> ThreadMemoryPools { get; } = new ThreadLocal<MemoryPool>(() => new MemoryPool(), trackAllValues: true);

	public MemoryPool ThreadMemoryPool => ThreadMemoryPools.Value;

	public void Dispose()
	{
		ThreadMemoryPools.Values.SelectMany((MemoryPool value) => value.MemoryPools).ClearAll();
		ThreadMemoryPools.Dispose();
	}
}
