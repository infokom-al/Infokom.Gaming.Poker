using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MemoryPools;

public class ObjectPool<T> : IPool
{
	public static ThreadLocal<ObjectPool<T>> _threadShared = new ThreadLocal<ObjectPool<T>>(() => CreateThreadShared(), trackAllValues: true);

	private ConstructorInfo _constructor;

	private readonly int _chunkSize;

	private int _chunksCount;

	public static ObjectPool<T> ThreadShared => _threadShared.Value;

	public static bool EnableThreadPooling
	{
		get
		{
			return ThreadShared.EnablePooling;
		}
		set
		{
			ThreadShared.EnablePooling = value;
		}
	}

	public ConstructorInfo Constructor => _constructor ?? (_constructor = typeof(T).GetParameterlessConstructor());

	public List<T>[] Chunks { get; private set; }

	public int Position { get; private set; }

	public int ThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

	public bool EnablePooling { get; set; } = true;

	public int Length => _chunksCount * _chunkSize;

	private List<T> _currentChunk => Chunks[ChunkId];

	public int ChunkId => Position / _chunkSize;

	public int ChunkPosition => Position - ChunkId * _chunkSize;

	public T? LastItem
	{
		get
		{
			if (Position == -1)
			{
				return default(T);
			}
			return _currentChunk[ChunkPosition];
		}
	}

	public ObjectPool(int chunkSize = 10)
	{
		_chunkSize = chunkSize;
		Chunks = new List<T>[100];
		Position = -1;
	}

	public static ObjectPool<T> CreateThreadShared(int chunkSize = 10)
	{
		ObjectPool<T> objectPool = new ObjectPool<T>(chunkSize);
		Pooling.ThreadSharedPools.Add(objectPool);
		return objectPool;
	}

	public ObjectPool(List<T> objects)
	{
		_chunkSize = objects.Count;
		Chunks = new List<T>[1];
		Chunks[0] = objects;
		Position = -1;
	}

	public T RentObject()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadId)
		{
			throw new InvalidOperationException($"{GetType()} is accessed from different threads");
		}
		if (!Pooling.EnableThreadObjectPooling || !EnablePooling)
		{
			return GetNew();
		}
		MoveCursor();
		if (ChunkPosition < _currentChunk.Count)
		{
			return _currentChunk[ChunkPosition];
		}
		if (Pooling.ThrowIfMissingPooledObject)
		{
			throw new InvalidOperationException("Missing object " + typeof(T).Name + " in the pool.");
		}
		T val = GetNew();
		_currentChunk.Add(val);
		return val;
	}

	private T GetNew()
	{
		return (T)Constructor.Invoke(Array.Empty<object>());
	}

	public void Reclaim()
	{
		Position = -1;
	}

	public void Clear()
	{
		Position = -1;
		Chunks = new List<T>[100];
		_chunksCount = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MoveCursor()
	{
		Position++;
		if (ChunkId >= Chunks.Length)
		{
			ExtendChunks();
		}
		if (ChunkId >= _chunksCount)
		{
			Chunks[ChunkId] = new List<T>(_chunkSize);
			_chunksCount++;
		}
	}

	private void ExtendChunks()
	{
		List<T>[] array = new List<T>[Chunks.Length + 100];
		for (int i = 0; i < Chunks.Length; i++)
		{
			array[i] = Chunks[i];
		}
		Chunks = array;
	}

	public void Return(T @object)
	{
		if (!Pooling.EnableThreadObjectPooling || !EnablePooling)
		{
			return;
		}
		if ((object)_currentChunk[ChunkPosition] != (object)@object)
		{
			throw new NotImplementedException("We support only returning of the last item in the pool");
		}
		throw new NotImplementedException();
	}

	public void Refill()
	{
		Refill(0);
	}

	public void Refill(int minCount)
	{
		foreach (List<T> item in Chunks.TakeWhile((List<T> chunk) => chunk != null))
		{
			for (int num = 0; num < item.Count && item[num] != null; num++)
			{
				item[num] = GetNew();
			}
		}
		Position = -1;
	}

	public override string ToString()
	{
		return $"Pool of {typeof(T).Name} with {Position} items";
	}
}
