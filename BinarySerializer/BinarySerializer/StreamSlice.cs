using System;
using System.IO;

namespace BinarySerializer;

public class StreamSlice : Stream
{
	private Stream _innerStream;

	private long _originalPosition;

	private long _length;

	private long _position;

	public override bool CanRead => _innerStream.CanRead;

	public override bool CanSeek => _innerStream.CanSeek;

	public override bool CanWrite => false;

	public override long Length => _length;

	public override long Position
	{
		get
		{
			return _position;
		}
		set
		{
			Seek(value, SeekOrigin.Begin);
		}
	}

	public StreamSlice(Stream innerstream, int length)
	{
		_innerStream = innerstream;
		_originalPosition = innerstream.Position;
		_length = length;
		_position = 0L;
	}

	public override void Flush()
	{
		_innerStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (_position >= _length)
		{
			return 0;
		}
		long val = _length - _position;
		int count2 = (int)Math.Min(count, val);
		int num = _innerStream.Read(buffer, offset, count2);
		_position += num;
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		long num = origin switch
		{
			SeekOrigin.Begin => _originalPosition + offset, 
			SeekOrigin.Current => _position + offset, 
			SeekOrigin.End => _originalPosition + _length + offset, 
			_ => throw new ArgumentOutOfRangeException("origin", "Invalid seek origin"), 
		};
		if (num < _originalPosition || num > _originalPosition + _length)
		{
			throw new ArgumentOutOfRangeException("offset", "Seek operation attempted to exceed slice bounds");
		}
		_position = num - _originalPosition;
		_innerStream.Seek(num, SeekOrigin.Begin);
		return _position;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException("StreamSlice does not support SetLength");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("StreamSlice is read-only");
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_innerStream = null;
		}
		base.Dispose(disposing);
	}
}
