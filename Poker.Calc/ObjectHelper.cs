namespace Poker.Calc;

internal static class ObjectHelper
{
	public static T VerifyType<T>(this object @object)
	{
		if (@object is T)
		{
			return (T)@object;
		}
		throw new InvalidOperationException("Expected object of type " + typeof(T).Name + " but was " + @object.GetType().Name);
	}
}
