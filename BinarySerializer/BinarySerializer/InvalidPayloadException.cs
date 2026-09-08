using System;

namespace BinarySerializer;

public class InvalidPayloadException : Exception
{
	public InvalidPayloadException(string message)
		: base(message)
	{
	}
}
