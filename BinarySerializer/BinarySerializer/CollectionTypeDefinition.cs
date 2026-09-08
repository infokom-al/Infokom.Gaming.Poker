using System;

namespace BinarySerializer;

public record CollectionTypeDefinition(Type Type, ITypeDefinition ArgumentTypeDefinition) : ITypeDefinition;
