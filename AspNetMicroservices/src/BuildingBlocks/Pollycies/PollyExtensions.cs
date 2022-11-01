using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Pollycies;

public static class PollyExtensions
{
    public static IHttpClientBuilder AddDefaultPollycies(this IHttpClientBuilder httpClientBuilder) =>
        httpClientBuilder
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, retryCount, context) =>
                {
                    Log.Error("Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey}, due to: {Exception}.", retryCount,
                        context.PolicyKey, context.OperationKey, exception);
                });

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );
}
