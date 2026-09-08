using System.Collections.Generic;

namespace BinarySerializer;

public record ObjectSerializableValue(int TypeTag, Dictionary<int, ISerializableValue> Properties) : ISerializableValue
{
	public bool TryGetPropertyValue(int tag, out ISerializableValue result)
	{
		return Properties.TryGetValue(tag, out result);
	}
}
