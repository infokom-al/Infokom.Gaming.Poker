using System.Reflection;

namespace Poker.Calc;

[Obfuscation(Exclude = true)]
public enum Streets
{
	Preflop,
	Flop,
	Turn,
	River
}
