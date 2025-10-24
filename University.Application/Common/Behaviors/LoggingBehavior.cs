using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace University.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = ResolveCorrelationId();
        var startTimestamp = Stopwatch.GetTimestamp();

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestName"] = requestName,
        }))
        {
            _logger.LogInformation("Handling {RequestName}", requestName);

            try
            {
                var response = await next().ConfigureAwait(false);
                var elapsed = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

                _logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds} ms",
                    requestName,
                    elapsed);

                return response;
            }
            catch (Exception exception)
            {
                var elapsed = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

                _logger.LogError(
                    exception,
                    "Request {RequestName} failed after {ElapsedMilliseconds} ms",
                    requestName,
                    elapsed);

                throw;
            }
        }
    }

    private static string ResolveCorrelationId()
    {
        return Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
    }
}