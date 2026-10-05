using MediatR;

namespace Infokom.Gaming.Poker.Texas.Estimators
{

	public partial class MonteCarloEstimator : IRequestHandler<MonteCarloEstimator.Request, MonteCarloEstimator.Response>
	{
		Task<Response> IRequestHandler<Request, Response>.Handle(Request request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);

			return Task.Run(() => Handle(request)).WaitAsync(cancellationToken);
		}

		public static IRequestHandler<Request, Response> Handler => new MonteCarloEstimator();
	}
}