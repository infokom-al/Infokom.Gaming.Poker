namespace Poker.Calc;

public class StreetAttribute : Attribute
{
	public Streets Street { get; }

	public StreetAttribute(Streets street)
	{
		Street = street;
	}
}
