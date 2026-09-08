using System;

namespace BinarySerializer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
public class TypeTagAttribute : Attribute
{
	public int Tag { get; }

	public Type Type { get; }

	public TypeTagAttribute(int tag, Type type)
	{
		Tag = tag;
		Type = type;
	}
}
