namespace MemoryPools;

public class ChunkedArrayCursor<T>
{
	public ChunkedArray<T> ChunkedArray { get; private set; }

	public int Cursor { get; private set; }

	public long UsedLength => (long)Cursor * (long)ChunkedArray.ChunkSize;

	public long AlocatedLength => ChunkedArray.AllocatedLength * ChunkedArray.ChunkSize;

	public ChunkedArrayCursor(ChunkedArray<T> chunkedArray, int cursor)
	{
		ChunkedArray = chunkedArray;
		Cursor = cursor;
	}

	public T[] GetNextChunk()
	{
		if (Cursor >= ChunkedArray.AllocatedLength)
		{
			ChunkedArray.Expand();
		}
		T[] result = ChunkedArray.Chunks[Cursor];
		Cursor++;
		return result;
	}

	public void Reclaim()
	{
		Cursor = 0;
	}

	public void Clear()
	{
		ChunkedArray.Clear();
	}

	public ChunkedArrayCursor(int chunkSize)
		: this(new ChunkedArray<T>(chunkSize), 0)
	{
	}
}
