using MediatR;

using System.Collections.Immutable;


namespace Infokom.Gaming.Poker.Texas.Estimators
{

	public partial class MonteCarloEstimator
	{
		public class Request : IRequest<Response>
		{
			private Request(GTO.Range[] ranges, int trialCount = 10000)
			{
				this.Id = (ulong)Environment.TickCount64;
				this.Players = [.. ranges];
				this.Trials = trialCount;
			}

			/// <summary>
			/// Identifier of this request in order to discrimate ammong others 
			/// </summary>
			public ulong Id { get; }


			/// <summary>
			/// Plyers preflop hand preferences
			/// </summary>
			public ImmutableArray<GTO.Range> Players { get; }

			/// <summary>
			/// Number of game to try by randomly picking players from their preferences
			/// </summary>
			public int Trials { get; }

			/// <summary>
			/// Create new request for heads up Monte Carlo estimation
			/// </summary>
			/// <param name="hero">First player preflop preference</param>
			/// <param name="villain">Second player preflop preference</param>
			/// <param name="trials"></param>
			/// <returns>Request ready for Monte Carlo Estimation</returns>
			public static Request Create(GTO.Range hero, GTO.Range villain, int trials = 10000) => new([hero, villain], trials);

			/// <summary>
			/// Create new request for multiplayer game preference confrontation
			/// </summary>
			/// <param name="players"></param>
			/// <param name="trials">Number of confrontation trials</param>
			/// <returns></returns>
			/// <exception cref="ArgumentOutOfRangeException"></exception>
			public static Request Create(ReadOnlySpan<GTO.Range> players, int trials = 10000)
			{
				ArgumentOutOfRangeException.ThrowIfLessThan(players.Length, 2, nameof(players));
				ArgumentOutOfRangeException.ThrowIfNegative(trials, nameof(trials));


				return new Request(players.ToArray(), trials);
			}
		}
	}
}