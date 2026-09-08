using System;
using System.Collections.Generic;

namespace CSharpSerializer.Serialization;

public class Deserializer
{
	public Dictionary<string, Type> AllDeserializableTypes { get; }

	public Deserializer(Dictionary<string, Type> allDeserializableTypes)
	{
		AllDeserializableTypes = allDeserializableTypes;
	}
}
