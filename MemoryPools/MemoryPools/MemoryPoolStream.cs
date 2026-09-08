using System;
using System.IO;

namespace MemoryPools;

public class MemoryPoolStream : Stream, IDisposable
{
	public MemoryList<byte> Buffer { get; private set; }

	public override long Position { get; set; }

	public MemoryPool<byte> MemoryPool { get; }

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => true;

	public override long Length => Buffer.Count;

	public MemoryPoolStream(MemoryPool<byte> memoryPool)
	{
		MemoryPool = memoryPool;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (Position != Buffer.Count)
		{
			Buffer = new MemoryList<byte>(Buffer.Buffer, Position.VerifyInt());
		}
		Buffer = Buffer.AddRange(buffer.AsSpan().Slice(offset, count));
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		Position = origin switch
		{
			SeekOrigin.Begin => offset, 
			SeekOrigin.Current => Position + offset, 
			SeekOrigin.End => Length + offset, 
			_ => throw new NotImplementedException(), 
		};
		return Position;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public new void Dispose()
	{
		MemoryPool.Reclaim();
	}
}
