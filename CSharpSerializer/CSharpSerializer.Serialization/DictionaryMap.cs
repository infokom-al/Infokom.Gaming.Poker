using System.Collections.Immutable;

namespace CSharpSerializer.Serialization;

public record DictionaryMap(ImmutableList<(IObjectMapValue key, IObjectMapValue value)> KeyValues) : IObjectMapValue;
