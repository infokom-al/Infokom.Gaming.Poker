using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MemoryPools;

namespace BinarySerializer;

public static class StreamFunctions
{
	private static readonly FieldInfo StreamOrigin = typeof(MemoryStream).GetPrivateFieldOrThrow("_origin");

	private static readonly FieldInfo StreamBuffer = typeof(MemoryStream).GetPrivateFieldOrThrow("_buffer");

	public static Stream WriteString(this Stream stream, string? value)
	{
		if (value == null)
		{
			return stream;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		stream.Write(bytes, 0, bytes.Length);
		return stream;
	}

	public static Stream WriteStringLengthPrefixed(this Stream stream, string? value)
	{
		if (value == null)
		{
			return stream;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		stream.WriteInt32(bytes.Length);
		stream.Write(bytes, 0, bytes.Length);
		return stream;
	}

	public static Stream WriteInt64(this Stream stream, long value)
	{
		Span<byte> span = stackalloc byte[8];
		if (!BitConverter.TryWriteBytes(span, value))
		{
			throw new InvalidOperationException($"Failed to write int64 {value}");
		}
		stream.Write(span);
		return stream;
	}

	public static Stream WriteUlong(this Stream stream, ulong value)
	{
		Span<byte> span = stackalloc byte[8];
		if (!BitConverter.TryWriteBytes(span, value))
		{
			throw new InvalidOperationException($"Failed to write ulong {value}");
		}
		stream.Write(span);
		return stream;
	}

	public static Stream WriteInt32(this Stream stream, int value)
	{
		Span<byte> span = stackalloc byte[4];
		if (!BitConverter.TryWriteBytes(span, value))
		{
			throw new InvalidOperationException($"Failed to write int32 {value}");
		}
		stream.Write(span);
		return stream;
	}

	public static Stream WriteDouble(this Stream stream, double value)
	{
		Span<byte> span = stackalloc byte[8];
		if (!BitConverter.TryWriteBytes(span, value))
		{
			throw new InvalidOperationException($"Failed to write double {value}");
		}
		stream.Write(span);
		return stream;
	}

	public static Stream WriteFloat(this Stream stream, float value)
	{
		Span<byte> span = stackalloc byte[4];
		if (!BitConverter.TryWriteBytes(span, value))
		{
			throw new InvalidOperationException($"Failed to write float {value}");
		}
		stream.Write(span);
		return stream;
	}

	public static Stream Skip(this Stream stream, int bytesCount)
	{
		stream.Position += bytesCount;
		return stream;
	}

	public static int ReadInt32(this Stream stream)
	{
		Span<byte> span = stackalloc byte[4];
		int num = stream.Read(span);
		if (num != 4)
		{
			throw new InvalidOperationException($"Expected 4 bytes but was {num}");
		}
		return BitConverter.ToInt32(span);
	}

	public static long ReadInt64(this Stream stream)
	{
		Span<byte> span = stackalloc byte[8];
		int num = stream.Read(span);
		if (num != 8)
		{
			throw new InvalidOperationException($"Expected 8 bytes but was {num}");
		}
		return BitConverter.ToInt64(span);
	}

	public static ulong ReadUInt64(this Stream stream)
	{
		Span<byte> span = stackalloc byte[8];
		int num = stream.Read(span);
		if (num != 8)
		{
			throw new InvalidOperationException($"Expected 8 bytes but was {num}");
		}
		return BitConverter.ToUInt64(span);
	}

	public static double ReadDouble(this Stream stream)
	{
		Span<byte> span = stackalloc byte[8];
		int num = stream.Read(span);
		if (num != 8)
		{
			throw new InvalidOperationException($"Expected 8 bytes but was {num}");
		}
		return BitConverter.ToDouble(span);
	}

	public static float ReadFloat(this Stream stream)
	{
		Span<byte> span = stackalloc byte[4];
		int num = stream.Read(span);
		if (num != 4)
		{
			throw new InvalidOperationException($"Expected 4 bytes but was {num}");
		}
		return BitConverter.ToSingle(span);
	}

	public static string ReadStringTillEnd(this Stream stream)
	{
		Span<byte> span = ((stream.Length >= 16000) ? ((Span<byte>)new byte[(int)stream.Length]) : stackalloc byte[(int)stream.Length]);
		Span<byte> buffer = span;
		int length = stream.Read(buffer);
		return Encoding.UTF8.GetString(buffer.Slice(0, length));
	}

	public static StreamSlice Slice(this Stream stream, int length)
	{
		return new StreamSlice(stream, length);
	}

	public static T Reset<T>(this T stream) where T : Stream
	{
		stream.Position = 0L;
		return stream;
	}

	public static bool HasNext(this Stream stream)
	{
		return stream.Position != stream.Length;
	}

	public static string Print(this Stream stream)
	{
		if (!stream.CanSeek)
		{
			return "Not Seekable stream";
		}
		int num = (int)stream.Position;
		byte[] array = new byte[stream.Reset().Length];
		stream.Reset().CopyTo(array.ToMemoryStream());
		stream.Position = num;
		return string.Concat(array.Take(num).Reverse().Take(4)
			.Reverse()
			.JoinWithSpace() + " *", array.Skip(num).Take(10).JoinWithSpace());
	}

	public static MemoryStream ToMemoryStream(this byte[] bytes)
	{
		return new MemoryStream(bytes);
	}

	internal static bool TryGetArraySegment(this Stream source, out ArraySegment<byte> data)
	{
		if (!(source is MemoryStream { CanSeek: not false } memoryStream) || (!memoryStream.TryGetBuffer(out var buffer) && !memoryStream.TryGetArraySegment(out buffer)) || buffer.Array == null)
		{
			data = default(ArraySegment<byte>);
			return false;
		}
		int num = checked((int)memoryStream.Position);
		int num2 = buffer.Count - num;
		int offset = buffer.Offset + num;
		data = new ArraySegment<byte>(buffer.Array, offset, num2);
		memoryStream.Seek(num2, SeekOrigin.Current);
		return true;
	}

	private static bool TryGetArraySegment(this MemoryStream ms, out ArraySegment<byte> arraySegment)
	{
		try
		{
			int offset = (int)(StreamOrigin.GetValue(ms) ?? throw new InvalidOperationException("StreamOrigin was null"));
			byte[] array = ((byte[])StreamBuffer.GetValue(ms)) ?? throw new InvalidOperationException("StreamBuffer was null");
			arraySegment = new ArraySegment<byte>(array, offset, checked((int)ms.Length));
			return true;
		}
		catch
		{
		}
		arraySegment = default(ArraySegment<byte>);
		return false;
	}

	public static byte[] GetBytesFromPool(this Stream stream, MemoryStream bufferStream, out int length)
	{
		bufferStream.SetLength(0L);
		stream.CopyTo(bufferStream);
		return bufferStream.ToArrayFromPool(out length);
	}

	public static byte[] GetBytesFromPool(this Stream stream, out int length)
	{
		using MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		return memoryStream.ToArrayFromPool(out length);
	}

	public static byte[] GetBytes(this Stream stream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] GetBytes(this Stream stream, int count, byte[] buffer)
	{
		stream.Read(buffer, 0, count);
		return buffer;
	}

	public static byte[] GetBytes(this Stream stream, int count)
	{
		byte[] array = new byte[count];
		stream.Read(array, 0, count);
		return array;
	}
}
