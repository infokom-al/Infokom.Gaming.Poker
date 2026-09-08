using System.Collections.Immutable;

namespace BinarySerializer;

public record CollectionSerializableValue(ImmutableList<ISerializableValue> Values) : ISerializableValue;
