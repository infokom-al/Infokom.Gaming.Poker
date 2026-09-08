using System.Globalization;
using System.Text;

namespace Poker.Calc;

internal class FluentParser
{
	public string String { get; }

	private int _position { get; set; }

	public int Position => _position;

	public char NextChar => String[_position];

	public bool HasNext => _position < String.Length - 1;

	public char NextNextChar
	{
		get
		{
			if (CharactersLeft <= 1)
			{
				return '\0';
			}
			return String[_position + 1];
		}
	}

	public char PreviousChar => String[_position - 1];

	public int Length => String.Length;

	public bool HasCurrent => String.Length > _position;

	public int CharactersLeft => Length - _position;

	public bool NextIsSpace => NextChar == ' ';

	public bool NextIsDigit => NextChar.IsDigit();

	public FluentParser(string @string)
	{
		String = @string;
	}

	public FluentParser SkipOne()
	{
		_position++;
		return this;
	}

	public FluentParser Skip(int count)
	{
		_position += count;
		return this;
	}

	public FluentParser SkipSpaces()
	{
		while (HasNext && NextChar == ' ')
		{
			SkipOne();
		}
		return this;
	}

	public FluentParser SkipToDigit()
	{
		while (HasCurrent && !NextChar.IsDigit())
		{
			SkipOne();
		}
		return this;
	}

	public FluentParser SkipToEnd()
	{
		_position = String.Length;
		return this;
	}

	public FluentParser RollbackUntilSpace()
	{
		return RollbackUntil(" ");
	}

	public FluentParser RollbackOne()
	{
		return Skip(-1);
	}

	public FluentParser RollbackUntil(string @string)
	{
		int num = String.LastIndexOf(@string, Position, StringComparison.Ordinal);
		if (num == -1)
		{
			_position = 0;
		}
		_position = num + @string.Length;
		return this;
	}

	public bool Next(char @char)
	{
		return NextChar == @char;
	}

	public bool Next(string @string, int offset = 0)
	{
		if (@string.Length > CharactersLeft)
		{
			return false;
		}
		if (String[_position + offset] != @string[0])
		{
			return false;
		}
		for (int i = offset + 1; i < @string.Length; i++)
		{
			if (@string[i] != String[_position + i])
			{
				return false;
			}
		}
		return true;
	}

	public char NextCharAt(int offset)
	{
		return String[_position + offset];
	}

	public char ReadOne()
	{
		char nextChar = NextChar;
		_position++;
		return nextChar;
	}

	public string Read(int count)
	{
		count = Math.Min(count, CharactersLeft);
		string result = String.Substring(_position, count);
		_position += count;
		return result;
	}

	public string ReadUntilPosition(int position)
	{
		if (position < Position)
		{
			throw new ArgumentException($"Can't read until position {position} because now the reader is at the position {Position}");
		}
		return Read(position - Position);
	}

	private string Read(FluentParser other)
	{
		return Read(other.Position - _position);
	}

	public string ReadUntil(char @char, int maxLength)
	{
		if (NextChar == @char)
		{
			return string.Empty;
		}
		int num = Math.Min(_position + maxLength, Length);
		for (int i = _position; i < num; i++)
		{
			if (String[i] == @char)
			{
				string result = String.Substring(_position, i - _position);
				_position = i;
				return result;
			}
		}
		return string.Empty;
	}

	public string ReadUntil(char @char)
	{
		if (!HasNext || NextChar == @char)
		{
			return string.Empty;
		}
		for (int i = _position; i < Length; i++)
		{
			if (String[i] == @char)
			{
				string result = String.Substring(_position, i - _position);
				_position = i;
				return result;
			}
		}
		return string.Empty;
	}

	public string ReadBackUntil(char @char)
	{
		if (Position == 0 || PreviousChar == @char)
		{
			return string.Empty;
		}
		for (int num = _position - 1; num >= 0; num--)
		{
			if (String[num] == @char)
			{
				string result = String.Substring(num + 1, _position - num - 1);
				_position = num + 1;
				return result;
			}
		}
		return string.Empty;
	}

	public string ReadAfter(string @string)
	{
		return ReadUntil(@string) + @string;
	}

	public string ReadUntil(string @string)
	{
		int num = String.IndexOf(@string, _position, StringComparison.Ordinal);
		if (num == _position)
		{
			return string.Empty;
		}
		if (num == -1)
		{
			return ReadToEnd();
		}
		string result = String.Substring(_position, num - _position);
		_position = num;
		return result;
	}

	public string ReadUntilLast(char @char)
	{
		int num = String.LastIndexOf(@char);
		if (num == -1 || num == _position)
		{
			return string.Empty;
		}
		string result = String.Substring(Position, num - Position);
		_position = num;
		return result;
	}

	public bool TryReadUntilSpace(out string result)
	{
		return TryReadUntil(' ', int.MaxValue, out result);
	}

	public bool TryReadUntil(char @char, int maxLength, out string result)
	{
		if (NextChar == @char)
		{
			result = string.Empty;
			return true;
		}
		if (!HasNext)
		{
			result = null;
			return false;
		}
		int num = String.IndexOf(@char, _position, Math.Min(maxLength, CharactersLeft));
		if (num == -1)
		{
			result = null;
			return false;
		}
		result = String.Substring(_position, num - _position);
		_position = num;
		return true;
	}

	public bool TryReadAfter(string @string, out string result)
	{
		if (TryReadUntil(@string, out result))
		{
			_position += @string.Length;
			result += @string;
			return true;
		}
		result = null;
		return false;
	}

	public bool TryReadUntil(string @string, out string result)
	{
		_ = _position;
		int num = String.IndexOf(@string, _position, StringComparison.Ordinal);
		if (num == -1)
		{
			result = null;
			return false;
		}
		result = String.Substring(_position, num - _position);
		_position = num;
		return true;
	}

	public bool TrySkipAfter(string @string, StringComparison comparisonType = StringComparison.Ordinal)
	{
		int num = String.IndexOf(@string, _position, comparisonType);
		if (num == -1)
		{
			return false;
		}
		_position = num + @string.Length;
		return true;
	}

	public bool TrySkipUntil(string @string)
	{
		int num = String.IndexOf(@string, _position, StringComparison.Ordinal);
		if (num == -1)
		{
			return false;
		}
		_position = num;
		return true;
	}

	public bool TryLookWord(out string result, int offset = 0)
	{
		return new FluentParser(String).Skip(_position).TryReadWord(out result, offset);
	}

	public bool TryReadWord(out string result, int offset = 0)
	{
		if (!NextCharAt(offset).IsWordCharacter())
		{
			result = string.Empty;
			return false;
		}
		for (int i = _position + offset; i < Length; i++)
		{
			if (!String[i].IsWordCharacter())
			{
				result = String.Substring(_position + offset, i - _position - offset);
				_position = i;
				return true;
			}
		}
		result = string.Empty;
		return false;
	}

	public bool TryLookWordUntil(string @string, out string result, int offset = 0)
	{
		result = string.Empty;
		if (Next(@string, offset))
		{
			return true;
		}
		if (!HasNext)
		{
			return false;
		}
		for (int i = _position + offset; i < Length; i++)
		{
			if (Next(@string, i))
			{
				result = String.Substring(_position + offset, i - _position - offset);
				return true;
			}
			if (!String[i].IsWordCharacter())
			{
				break;
			}
		}
		return false;
	}

	public FluentParser SkipUntil(char @char)
	{
		if (HasCurrent)
		{
			int num = String.IndexOf(@char, _position);
			_position = ((num == -1) ? String.Length : num);
		}
		return this;
	}

	public FluentParser SkipUntilNextLine()
	{
		SkipAfter('\n');
		if (Next('\r'))
		{
			SkipOne();
		}
		return this;
	}

	public FluentParser SkipAfter(char @char)
	{
		if (HasCurrent)
		{
			int num = String.IndexOf(@char, _position);
			_position = ((num == -1) ? String.Length : (num + 1));
		}
		return this;
	}

	public FluentParser SkipAfter(string @string)
	{
		if (HasCurrent)
		{
			int num = String.IndexOf(@string, _position, StringComparison.Ordinal);
			_position = ((num == -1) ? String.Length : (num + @string.Length));
		}
		return this;
	}

	public bool TryReadXmlNode(out string result)
	{
		result = string.Empty;
		if (!Next('<') || CharactersLeft < 4)
		{
			return false;
		}
		FluentParser fluentParser = Clone();
		if (!fluentParser.SkipOne().TryReadWord(out string result2))
		{
			return false;
		}
		int num = 1;
		while (fluentParser.HasNext)
		{
			switch (fluentParser.ReadOne())
			{
				case '<':
					if (fluentParser.NextChar == '!')
					{
						fluentParser.SkipAfter('>');
					}
					else if (fluentParser.NextChar == '/')
					{
						num--;
						if (num == 0)
						{
							if (fluentParser.Next("/" + result2 + ">"))
							{
								result = Read(fluentParser.Skip(result2.Length + 2));
								return true;
							}
							return false;
						}
					}
					else
					{
						num++;
					}
					break;
				case '/':
					if (fluentParser.NextChar == '>')
					{
						num--;
						if (num == 0)
						{
							result = Read(fluentParser.SkipOne());
							return true;
						}
					}
					break;
			}
		}
		return false;
	}

	public bool TryReadJson<T>(Func<string, T> parser, out T result)
	{
		result = default(T);
		if (!HasNext || NextChar != '{')
		{
			return false;
		}
		int position = _position;
		int num = 0;
		while (HasNext)
		{
			char nextChar = NextChar;
			SkipOne();
			switch (nextChar)
			{
				case '{':
					num++;
					break;
				case '}':
					num--;
					if (num == 0)
					{
						try
						{
							result = parser(String.Substring(position, Position - position));
							return true;
						}
						catch (Exception)
						{
							_position = position;
							return false;
						}
					}
					break;
			}
		}
		_position = position;
		return false;
	}

	public string ReadToEnd()
	{
		string result = String.Substring(Position);
		_position = Length;
		return result;
	}

	public int ReadIntUntil(char @char)
	{
		if (!NextChar.IsDigit())
		{
			throw new InvalidOperationException($"Read must be positioned at a digit but was {this}");
		}
		int num = ReadDigit();
		while (NextChar != @char)
		{
			num = num * 10 + ReadDigit();
		}
		return num;
	}

	public int ReadNextInt()
	{
		return SkipToDigit().ReadInt();
	}

	public int ReadInt()
	{
		int num = 0;
		while (HasCurrent && NextChar.IsDigit())
		{
			num = num * 10 + NextChar.ToDigit();
			SkipOne();
		}
		return num;
	}

	public long ReadLong()
	{
		long num = 0L;
		while (HasCurrent && NextChar.IsDigit())
		{
			num = num * 10 + NextChar.ToDigit();
			SkipOne();
		}
		return num;
	}

	public double ReadNextDouble(CultureInfo cultureInfo = null)
	{
		return SkipToDigit().ReadDouble(cultureInfo);
	}

	public double ReadDouble(CultureInfo cultureInfo = null)
	{
		if (!NextChar.IsDigit())
		{
			throw new InvalidOperationException($"Reader position must be placed on a digit: {this}");
		}
		double result = ReadInt();
		while (HasCurrent)
		{
			if (Next('.') || Next(','))
			{
				if (!String[Position + 1].IsDigit())
				{
					return result;
				}
				SkipOne();
				var (num, num2) = ReadIntLocal();
				if (num2 == 3 && (cultureInfo == null || !object.Equals(cultureInfo, CultureInfo.InvariantCulture)))
				{
					AddIntPart(num, 3);
					continue;
				}
				return result + (double)num / Math.Pow(10.0, num2);
			}
			return result;
		}
		return result;
		void AddIntPart(int number, int digits)
		{
			result = result * Math.Pow(10.0, digits) + (double)number;
		}
		(int number, int digits) ReadIntLocal()
		{
			int position = Position;
			return (number: ReadInt(), digits: Position - position);
		}
	}

	public int ReadDigit()
	{
		int result = NextChar.ToDigit();
		SkipOne();
		return result;
	}

	public string ReadHexString()
	{
		string text = string.Empty;
		while (HasNext)
		{
			char c = ReadOne();
			if (!c.IsHexDigit())
			{
				break;
			}
			text += c;
		}
		return text;
	}

	public FluentParser Clone()
	{
		return new FluentParser(String).Skip(_position);
	}

	public override string ToString()
	{
		int num = Math.Max(_position - 20, 0);
		int num2 = Math.Min(Length, num + 40);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = num; i < num2; i++)
		{
			if (i == _position)
			{
				stringBuilder.Append("*");
			}
			stringBuilder.Append(String[i]);
		}
		return stringBuilder.ToString();
	}
}
