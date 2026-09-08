using System;
using System.Collections.Immutable;
using CSharpSerializer.Common;

namespace CSharpSerializer.Serialization;

public class ObjectMap : IObjectMapValue
{
	public ImmutableList<(string name, IObjectMapValue value)> Properties { get; }

	public ObjectMap(ImmutableList<(string name, IObjectMapValue value)> properties)
	{
		Properties = properties;
	}

	public bool TryGetPropertyValue<T>(string name, out T value) where T : IObjectMapValue
	{
		value = default(T);
		if (!TryGetPropertyValue(name, out IObjectMapValue value2))
		{
			return false;
		}
		value = value2.VerifyType<T>();
		return true;
	}

	public IObjectMapValue GetPropertyValue(string name)
	{
		if (!TryGetPropertyValue(name, out IObjectMapValue value))
		{
			throw new InvalidOperationException("Failed to get property " + name.Quoted());
		}
		return value;
	}

	public bool TryGetPropertyValue(string name, out IObjectMapValue value)
	{
		if (Properties.TryGet<(string, IObjectMapValue)>(((string name, IObjectMapValue value) property) => property.name == name, out var result))
		{
			value = result.Item2;
			return true;
		}
		value = null;
		return false;
	}

	public override string ToString()
	{
		return $"{"ObjectMap"} with {Properties.Count} properties";
	}
}
