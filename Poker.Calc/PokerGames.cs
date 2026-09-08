namespace Poker.Calc;

[Flags]
public enum PokerGames : byte
{
	[Name("Texas Hold'em")]
	TexasHoldem = 1,
	[Name("Omaha")]
	Omaha = 2,
	[Name("6+ Hold'em")]
	ShortDeck = 4,
	[Name("Omaha 5 Cards")]
	OmahaFive = 8,
	[Name("6+ Hold'em (Trips Beats Straight)")]
	ShortDeckTbs = 0x10,
	[Name("Omaha 6 Cards")]
	OmahaSix = 0x20,
	[Name("All games")]
	AllGames = 0x3F,
	[Name("6+ Hold'em Family")]
	ShortDeckFamily = 0x14,
	[Name("Omaha Family")]
	OmahaFamily = 0x2A
}
