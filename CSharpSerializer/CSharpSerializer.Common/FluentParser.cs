using System;
using System.Linq;
using System.Text;

namespace CSharpSerializer.Common;

public class FluentParser
{
	public string String { get; }

	public int Cursor { get; private set; }

	public char PreviousChar => String[Cursor - 1];

	public bool HasCurrent => String.Length > Cursor;

	public char NextChar => String[Cursor];

	public bool HasNext => Cursor < String.Length - 1;

	public bool HasNextNext => Cursor < String.Length - 2;

	public char NextNextChar => String[Cursor + 1];

	public int Length => String.Length;

	public bool IsNextCharEscaped
	{
		get
		{
			if (Cursor > 0 && String[Cursor - 1] == '\\')
			{
				if (Cursor > 1)
				{
					return String[Cursor - 2] != '\\';
				}
				return true;
			}
			return false;
		}
	}

	public FluentParser(string @string)
	{
		String = @string;
	}

	public FluentParser SkipOne()
	{
		Cursor++;
		return this;
	}

	public FluentParser MoveTo(int position)
	{
		Cursor = position;
		return this;
	}

	public bool TryReadAfterNotEscaped(char @char, out string result)
	{
		if (TryReadUntilUnescaped(@char, out result))
		{
			result += @char;
			SkipOne();
			return true;
		}
		return false;
	}

	public bool TryReadUntilUnescaped(char @char, out string result)
	{
		int cursor = Cursor;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		while (HasCurrent)
		{
			char nextChar = NextChar;
			if (nextChar == @char && !flag)
			{
				result = stringBuilder.ToString();
				return true;
			}
			flag = ((nextChar == '\\' && !flag) ? true : false);
			stringBuilder.Append(nextChar);
			SkipOne();
		}
		Cursor = cursor;
		result = null;
		return false;
	}

	public bool TryReadUntil(string @string, out string result)
	{
		_ = Cursor;
		int num = String.IndexOf(@string, Cursor, StringComparison.Ordinal);
		if (num == -1)
		{
			result = null;
			return false;
		}
		result = String.Substring(Cursor, num - Cursor);
		Cursor = num;
		return true;
	}

	public string ReadToEnd()
	{
		string result = String.Substring(Cursor);
		Cursor = String.Length;
		return result;
	}

	public FluentParser SkipAfterLast(string @string)
	{
		int num = String.LastIndexOf(@string);
		if (num <= Cursor)
		{
			return this;
		}
		Cursor = num + @string.Length;
		return this;
	}

	public FluentParser SkipAfter(string @string)
	{
		if (HasCurrent)
		{
			int num = String.IndexOf(@string, Cursor, StringComparison.Ordinal);
			Cursor = ((num == -1) ? String.Length : (num + @string.Length));
		}
		return this;
	}

	public bool TrySkipbackwardsAfter(char @char)
	{
		if (Cursor == 0)
		{
			return false;
		}
		int cursor = Cursor;
		for (int num = Cursor - 1; num >= 0; num--)
		{
			Skip(-1);
			if (NextChar == @char)
			{
				return true;
			}
		}
		Cursor = cursor;
		return false;
	}

	public string ReadUntilSpace()
	{
		return ReadUntil(" ");
	}

	public string ReadUntilSpaceOrEnd()
	{
		return ReadUntilOrEnd(" ");
	}

	public string ReadUntil(string @string)
	{
		int num = String.IndexOf(@string, Cursor, StringComparison.Ordinal);
		if (num == -1 || num == Cursor)
		{
			return string.Empty;
		}
		string result = String.Substring(Cursor, num - Cursor);
		Cursor = num;
		return result;
	}

	public string ReadAfterOrEnd(string @string)
	{
		string text = ReadUntilOrEnd(@string);
		if (Next(@string))
		{
			text += @string;
			Skip(@string.Length);
			return text;
		}
		return text;
	}

	public string ReadUntilOrEnd(string @string)
	{
		int num = String.IndexOf(@string, Cursor, StringComparison.Ordinal);
		if (num == -1)
		{
			return String.Substring(Cursor);
		}
		if (num == Cursor)
		{
			return string.Empty;
		}
		string result = String.Substring(Cursor, num - Cursor);
		Cursor = num;
		return result;
	}

	public string ReadBackUntil(char @char)
	{
		if (Cursor == 0 || PreviousChar == @char)
		{
			return string.Empty;
		}
		for (int num = Cursor - 1; num >= 0; num--)
		{
			if (String[num] == @char)
			{
				string result = String.Substring(num + 1, Cursor - num - 1);
				Cursor = num + 1;
				return result;
			}
		}
		return string.Empty;
	}

	public string ReadBackAll()
	{
		return String.Substring(0, Cursor);
	}

	public FluentParser SkipToEnd()
	{
		Cursor = String.Length;
		return this;
	}

	public FluentParser SkipBackOne()
	{
		return Skip(-1);
	}

	public FluentParser Skip(int count)
	{
		Cursor += count;
		return this;
	}

	public int ReadNextInt()
	{
		return SkipToDigit().ReadInt();
	}

	public int ReadInt()
	{
		int num = 0;
		bool flag = HasCurrent && NextChar == '-';
		if (flag)
		{
			SkipOne();
		}
		while (HasCurrent && NextChar.IsDigit())
		{
			num = num * 10 + NextChar.ToDigit();
			SkipOne();
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public long ReadLong()
	{
		long num = 0L;
		bool flag = HasCurrent && NextChar == '-';
		if (flag)
		{
			SkipOne();
		}
		while (HasCurrent && NextChar.IsDigit())
		{
			num = num * 10 + NextChar.ToDigit();
			SkipOne();
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public FluentParser SkipToDigit()
	{
		while (HasCurrent && !NextChar.IsDigit())
		{
			SkipOne();
		}
		return this;
	}

	public bool TrySkipAfter(string @string, StringComparison comparisonType = StringComparison.Ordinal)
	{
		int num = String.IndexOf(@string, Cursor, comparisonType);
		if (num == -1)
		{
			return false;
		}
		Cursor = num + @string.Length;
		return true;
	}

	public FluentParser SkipSpaces()
	{
		while (HasNext && NextChar == ' ')
		{
			SkipOne();
		}
		return this;
	}

	public FluentParser SkipWhiteSpace()
	{
		while (HasNext && char.IsWhiteSpace(NextChar))
		{
			SkipOne();
		}
		return this;
	}

	public bool Next(string substring)
	{
		if (String.Length - Cursor < substring.Length)
		{
			return false;
		}
		for (int i = 0; i < substring.Length; i++)
		{
			if (String[Cursor + i] != substring[i])
			{
				return false;
			}
		}
		return true;
	}

	public FluentParser VerifyNext(string expected)
	{
		if (!Next(expected))
		{
			throw new InvalidOperationException($"Expected '{expected}' at position {Cursor} in '{String}'");
		}
		return this;
	}

	public string ReadUntilLast(char @char)
	{
		int num = String.LastIndexOf(@char);
		if (num == -1 || num == Cursor)
		{
			return string.Empty;
		}
		string result = String.Substring(Cursor, num - Cursor);
		Cursor = num;
		return result;
	}

	public string ReadUntilAnyOf(params char[] chars)
	{
		int cursor = Cursor;
		while (HasCurrent && !Enumerable.Contains(chars, NextChar))
		{
			SkipOne();
		}
		return String.Substring(cursor, Cursor - cursor);
	}

	public override string ToString()
	{
		int num = Math.Max(Cursor - 20, 0);
		int num2 = Math.Min(Length, num + 40);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = num; i < num2; i++)
		{
			if (i == Cursor)
			{
				stringBuilder.Append("*");
			}
			stringBuilder.Append(String[i]);
		}
		return stringBuilder.ToString();
	}

	public FluentParser Clone()
	{
		return new FluentParser(String)
		{
			Cursor = Cursor
		};
	}
}
