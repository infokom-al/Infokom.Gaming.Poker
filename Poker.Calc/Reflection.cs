using System.Reflection;

namespace Poker.Calc;

internal static class Reflection
{
	public static string GetNameWithGenerics(this Type type)
	{
		if (!type.IsGenericType)
		{
			return type.Name;
		}
		string name = type.GetGenericTypeDefinition().Name;
		name = name.Substring(0, name.IndexOf('`'));
		string text = type.GetGenericArguments().Select(GetNameWithGenerics).AggregateToString(", ");
		return name + "<" + text + ">";
	}

	public static ConstructorInfo GetParameterlessConstructor(this Type type)
	{
		return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault((ConstructorInfo constructor) => constructor.GetParameters().Length == 0) ?? throw new InvalidOperationException("Failed to get constructor of type " + type.Name.Quoted());
	}

	public static bool TryGetAttribute<T>(this Type objectType, out T result) where T : Attribute
	{
		object[] customAttributes = objectType.GetCustomAttributes(typeof(T), inherit: true);
		if (customAttributes.Length == 0)
		{
			result = null;
			return false;
		}
		result = (T)customAttributes[0];
		return true;
	}
}
