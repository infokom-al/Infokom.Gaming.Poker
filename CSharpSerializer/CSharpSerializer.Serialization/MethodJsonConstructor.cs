using System.Reflection;

namespace CSharpSerializer.Serialization;

public record MethodJsonConstructor(MethodInfo Method) : IJsonConstructor;
