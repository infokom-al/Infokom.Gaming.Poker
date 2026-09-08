using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BinarySerializer;

public class BinarySerializerWriter
{
	public static ThreadLocal<BinarySerializerWriter> _threadShared = new ThreadLocal<BinarySerializerWriter>(() => new BinarySerializerWriter(new byte[1048576], null));

	private Stream Destination;

	public byte[] Buffer;

	public int Position { get; private set; }

	public int BytesLeft => Buffer.Length - Position;

	public BinarySerializerWriter(byte[] buffer, Stream destination)
	{
		Destination = destination;
		Buffer = buffer;
	}

	public static BinarySerializerWriter ThreadShared(Stream stream)
	{
		_threadShared.Value.Destination = stream;
		_threadShared.Value.Position = 0;
		return _threadShared.Value;
	}

	public Span<byte> GetNextSpan(int length)
	{
		AllocateSpace(length);
		return Buffer.AsSpan().Slice(Position, length);
	}

	public BinarySerializerWriter AllocateSpace(int size)
	{
		if (BytesLeft < size)
		{
			if (Position + size > Array.MaxLength)
			{
				throw new InvalidOperationException($"Can't allocate so much space ({size:N0} bytes).");
			}
			byte[] array = new byte[Math.Max(Position + size, (Buffer.Length <= Array.MaxLength / 2) ? (Buffer.Length * 2) : Array.MaxLength)];
			if (Position > 0)
			{
				Array.Copy(Buffer, 0, array, 0, Position);
			}
			Buffer = array;
		}
		return this;
	}

	public BinarySerializerWriter AdvancePosition(int offset = 1)
	{
		AllocateSpace(offset);
		Position += offset;
		return this;
	}

	public BinarySerializerWriter Flush()
	{
		if (Position != 0)
		{
			Destination.Write(Buffer, 0, Position);
			Position = 0;
		}
		return this;
	}

	public void WriteByte(byte @byte)
	{
		AllocateSpace(1);
		Buffer[Position] = @byte;
		AdvancePosition();
	}

	public BinarySerializerWriter MoveTo(int position)
	{
		position.VerifyNotNegative("position");
		Position = position;
		return this;
	}

	public void Skip(int count)
	{
		Position += count;
	}

	public unsafe void WritePlainBytes<T>(T value)
	{
		int num = Unsafe.SizeOf<T>();
		AllocateSpace(num);
		fixed (byte* ptr = &Buffer[Position])
		{
			Unsafe.Write(ptr, value);
			Skip(num);
		}
	}

	public void WriteBytes(Span<byte> bytes)
	{
		AllocateSpace(bytes.Length);
		bytes.CopyTo(GetNextSpan(bytes.Length));
		Skip(bytes.Length);
	}
}
