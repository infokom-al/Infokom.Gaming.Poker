using Holdem.Core;

namespace TexasHandEvaluator
{
	internal class Program
	{
		static void Main()
		{
			Console.WriteLine(Hand.Evaluate(Hand.Parse("[Ac, Ad, Ah, As, Kc]")));
			Console.WriteLine(Hand.Evaluate(Hand.Parse("[Ac, Ad, Ah, Ks, Kc]")));
			Console.WriteLine(Hand.Evaluate(Hand.Parse("[Ac, Ad, Ah, Ks, Qc]")));
			Console.WriteLine(Hand.Evaluate(Hand.Parse("[Ac, Ad, Kh, Qs, Jc]")));
			Console.WriteLine(Hand.Evaluate(Hand.Parse("[Ac, Kd, Qh, Js, Tc]")));
			Console.WriteLine(Hand.Evaluate(Hand.Parse("[Ac, Kc, Qc, Jc, Tc]")));
		}
	}
}
