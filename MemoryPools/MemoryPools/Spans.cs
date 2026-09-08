using System;

namespace MemoryPools;

internal static class Spans
{
	public static Span<T> GetHead<T>(this Span<T> span, int headLength)
	{
		return span.Slice(0, headLength);
	}

	public static Span<T> CutHead<T>(this Span<T> span, int headLength)
	{
		return span.Slice(headLength, span.Length - headLength);
	}

	public static Span<T> CutTail<T>(this Span<T> span, int tailLength)
	{
		return span.Slice(0, span.Length - tailLength);
	}

	public static Span<T> GetTail<T>(this Span<T> span, int tailLength)
	{
		return span.Slice(span.Length - tailLength, tailLength);
	}
}
