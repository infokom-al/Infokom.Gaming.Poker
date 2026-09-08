using System.Collections.Immutable;

namespace BinarySerializer;

public record DictionarySerializableValue(ImmutableList<ObjectSerializableValue> KeyValues) : ISerializableValue;
