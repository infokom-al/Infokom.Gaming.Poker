using System.Reflection;

namespace CSharpSerializer.Serialization;

public record DefaultJsonConstructor(ConstructorInfo Constructor) : IJsonConstructor;
