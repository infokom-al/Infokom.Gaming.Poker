using System;
using System.Collections.Immutable;

namespace BinarySerializer;

public record ObjectTypeDefinition(Type Type, ImmutableDictionary<int, ITypeDefinition> Properties) : ITypeDefinition
{
	public ITypeDefinition GetPropertyType(int tag)
	{
		if (!Properties.TryGetValue(tag, out ITypeDefinition value))
		{
			throw new InvalidOperationException($"Property with tag {tag} not found");
		}
		return value;
	}

	public bool TryGetPropertyType(int tag, out ITypeDefinition result)
	{
		return Properties.TryGetValue(tag, out result);
	}
}
