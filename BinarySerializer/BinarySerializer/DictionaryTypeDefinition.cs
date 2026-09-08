using System;

namespace BinarySerializer;

public record DictionaryTypeDefinition(Type Type, ObjectTypeDefinition KeyValueDefinition) : ITypeDefinition;
