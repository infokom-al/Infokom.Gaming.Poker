using System;
using System.Collections.Immutable;

namespace BinarySerializer;

public record InterfaceTypeDefinition(Type Type, ImmutableDictionary<int, Type> SubTypes) : ITypeDefinition
{
	public bool TryGetSubTypeDefinition(int tag, out ObjectTypeDefinition result)
	{
		if (SubTypes.TryGetValue(tag, out Type value))
		{
			result = value.GetTypeDefinition().VerifyType<ObjectTypeDefinition>();
			return true;
		}
		result = null;
		return false;
	}
}
