using System;
using System.Runtime.CompilerServices;

namespace MemoryPools;

internal static class Threads
{
	public static void VerifySingleThreadId(this int threadId, [CallerMemberName] string? callerMemberName = null)
	{
		if (Environment.CurrentManagedThreadId != threadId)
		{
			throw new InvalidOperationException("This operation can't be called from mutiple threads (" + callerMemberName.Quoted() + ")");
		}
	}
}
