using System;

namespace BinarySerializer;

public class TagAttribute : Attribute
{
	public int Value { get; }

	public TagAttribute(int value)
	{
		Value = value;
		if (value <= 0)
		{
			throw new ArgumentException("Tag must be positive");
		}
	}
}
