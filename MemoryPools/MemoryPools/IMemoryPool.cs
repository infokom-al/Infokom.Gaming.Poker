namespace MemoryPools;

public interface IMemoryPool
{
	MemoryPoolStats GetPoolStats();

	int GetAllocatedMegabytes();

	int GetUsedMegabytes();

	void Reclaim();

	void Clear();

	void DeallocatePartial(double remainingPart);
}
