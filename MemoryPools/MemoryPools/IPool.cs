namespace MemoryPools;

public interface IPool
{
	void Clear();

	void Reclaim();

	void Refill();
}
