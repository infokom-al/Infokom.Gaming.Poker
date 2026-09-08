namespace CSharpSerializer.Common;

public static class FluentParserFunctions
{
	public static FluentParser ToFluentParser(this string @string)
	{
		return new FluentParser(@string);
	}
}
