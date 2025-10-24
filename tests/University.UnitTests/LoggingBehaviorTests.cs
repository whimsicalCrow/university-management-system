using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using University.Application.Common.Behaviors;

namespace University.UnitTests;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_LogsCorrelationIdAndDuration()
    {
        using var activity = new Activity("test");
        activity.SetIdFormat(ActivityIdFormat.W3C);
        activity.Start();

        try
        {
            var logger = new TestLogger<LoggingBehavior<TestRequest, string>>();
            var behavior = new LoggingBehavior<TestRequest, string>(logger);

            var response = await behavior.Handle(
                new TestRequest("value"),
                () => Task.FromResult("ok"),
                CancellationToken.None);

            Assert.Equal("ok", response);

            Assert.Collection(
                logger.Entries,
                entry =>
                {
                    Assert.Equal(LogLevel.Information, entry.LogLevel);
                    Assert.Equal("Handling TestRequest", entry.Message);
                },
                entry =>
                {
                    Assert.Equal(LogLevel.Information, entry.LogLevel);
                    Assert.StartsWith("Handled TestRequest in", entry.Message);
                });

            var scope = Assert.Single(logger.Scopes);
            Assert.Equal("TestRequest", scope["RequestName"]);
            Assert.Equal(activity.Id, scope["CorrelationId"]);
        }
        finally
        {
            activity.Stop();
        }
    }

    [Fact]
    public async Task Handle_WhenHandlerThrows_LogsError()
    {
        var logger = new TestLogger<LoggingBehavior<TestRequest, string>>();
        var behavior = new LoggingBehavior<TestRequest, string>(logger);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => behavior.Handle(
            new TestRequest("value"),
            () => ThrowAsync(),
            CancellationToken.None));

        Assert.Equal("boom", exception.Message);

        Assert.Equal(2, logger.Entries.Count);
        Assert.Equal(LogLevel.Information, logger.Entries[0].LogLevel);
        Assert.Equal(LogLevel.Error, logger.Entries[1].LogLevel);
        Assert.Equal("Request TestRequest failed after", logger.Entries[1].Message[.."Request TestRequest failed after".Length]);
        Assert.IsType<InvalidOperationException>(logger.Entries[1].Exception);
    }

    private static Task<string> ThrowAsync()
    {
        throw new InvalidOperationException("boom");
    }

    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = new();

        public List<IDictionary<string, object>> Scopes { get; } = new();

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            Scopes.Add(ConvertToDictionary(state));
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new LogEntry(
                logLevel,
                formatter(state, exception),
                exception,
                ConvertToDictionary(state)));
        }

        private static IDictionary<string, object> ConvertToDictionary(object? state)
        {
            if (state is IEnumerable<KeyValuePair<string, object>> keyValuePairs)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (var (key, value) in keyValuePairs)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary[key] = value ?? string.Empty;
                    }
                }

                return dictionary;
            }

            return new Dictionary<string, object>
            {
                ["State"] = state ?? string.Empty,
            };
        }

        public readonly record struct LogEntry(LogLevel LogLevel, string Message, Exception? Exception, IDictionary<string, object> State);

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}