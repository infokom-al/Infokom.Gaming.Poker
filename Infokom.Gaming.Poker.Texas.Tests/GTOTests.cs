using Xunit;

namespace Infokom.Gaming.Poker.Texas.Tests;

public sealed class GTOTests
{
	

	[Fact]
	public void DemoEstimation()
	{
		var h1 = GTO.Coranked.Above(Rank.Ace);
		var h2 = GTO.Coranked.Above(Rank.King).Below(Rank.King);

		Equity.Estimation.Request request = Equity.Estimation.Request.Create(h1, h2);

		var result = Equity.Estimation.EstimateMonteCarlo(request, 10000000);
		var winRate0 = result.WinRateOf(0);
		var winRate1 = result.WinRateOf(1);

		Assert.True(winRate0 > 0.65);
		Assert.True(winRate1 < 0.35);
	}
}
