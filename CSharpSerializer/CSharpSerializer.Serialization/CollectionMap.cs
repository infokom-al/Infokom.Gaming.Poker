using System.Collections.Immutable;

namespace CSharpSerializer.Serialization;

public record CollectionMap(ImmutableList<IObjectMapValue> Values) : IObjectMapValue;
