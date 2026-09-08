using System;
using System.Text;
using System.Threading;

namespace MemoryPools;

public class StringPool : IPool
{
	public static ThreadLocal<StringPool> _threadShared = new ThreadLocal<StringPool>(CreateThreadShared, trackAllValues: true);

	private readonly StringPoolNode?[] buckets;

	private int _bucketCount;

	private const int DefaultBucketsCount = 32768;

	public static StringPool ThreadShared => _threadShared.Value;

	public int ThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

	public static StringPool CreateThreadShared()
	{
		StringPool stringPool = new StringPool();
		Pooling.ThreadSharedPools.Add(stringPool);
		return stringPool;
	}

	public StringPool(int bucketsCount = 32768)
	{
		bucketsCount.VerifyIsPowerOfTwo("Buckets count must be a power of 2");
		buckets = new StringPoolNode[bucketsCount];
		_bucketCount = bucketsCount;
	}

	public static string GetString(ReadOnlySpan<byte> span, Encoding encoding)
	{
		return ThreadShared.GetOrAdd(span, encoding);
	}

	public static string GetString(Span<byte> span, Encoding encoding)
	{
		return ThreadShared.GetOrAdd(span, encoding);
	}

	private unsafe string GetOrAdd(ReadOnlySpan<byte> span, Encoding encoding)
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadId)
		{
			throw new InvalidOperationException($"{GetType()} is accessed from different threads");
		}
		if (span.IsEmpty)
		{
			return string.Empty;
		}
		int maxCharCount = encoding.GetMaxCharCount(span.Length);
		char[] array = ArrayPool<char>.ThreadShared.GetArray(maxCharCount);
		try
		{
			fixed (byte* bytes = span)
			{
				int chars2;
				fixed (char* chars = array)
				{
					chars2 = encoding.GetChars(bytes, span.Length, chars, maxCharCount);
				}
				Span<char> span2 = new Span<char>(array, 0, chars2);
				return GetOrAdd(span2);
			}
		}
		finally
		{
			array.ReturnToThreadSharedPool();
		}
	}

	public string GetOrAdd(Span<char> span)
	{
		int num = span.GetSpanHashCode() & (_bucketCount - 1);
		for (StringPoolNode stringPoolNode = buckets[num]; stringPoolNode != null; stringPoolNode = stringPoolNode.Next)
		{
			if (stringPoolNode.Value.AsSpan().SequenceEqual(span))
			{
				return stringPoolNode.Value;
			}
		}
		string text = span.ToString();
		StringPoolNode stringPoolNode2 = new StringPoolNode(text, buckets[num]);
		buckets[num] = stringPoolNode2;
		return text;
	}

	public void Clear()
	{
		for (int i = 0; i < buckets.Length; i++)
		{
			buckets[i] = null;
		}
	}

	public void Reclaim()
	{
	}

	public void Refill()
	{
	}
}
