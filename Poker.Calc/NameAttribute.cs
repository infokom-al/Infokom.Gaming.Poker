namespace Poker.Calc;

public class NameAttribute : Attribute
{
	public string Value { get; set; }

	public NameAttribute(string value)
	{
		Value = value;
	}
}
