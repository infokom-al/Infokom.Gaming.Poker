using System;

namespace BinarySerializer;

public class GeneratedDefaultValueAttribute : Attribute
{
	public string Expression { get; }

	public GeneratedDefaultValueAttribute(string expression)
	{
		Expression = expression;
	}
}
