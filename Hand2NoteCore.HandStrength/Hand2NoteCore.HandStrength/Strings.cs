namespace Hand2NoteCore.HandStrength;

internal static class Strings
{
	public static string AddWord(this string @string, string? word)
	{
		return @string = @string?.Trim() + " " + word?.Trim();
	}
}
