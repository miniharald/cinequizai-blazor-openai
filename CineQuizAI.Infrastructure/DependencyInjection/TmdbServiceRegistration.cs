using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Infrastructure.ExternalServices.Tmdb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CineQuizAI.Infrastructure.DependencyInjection;

public static class TmdbServiceRegistration
{
    public static IServiceCollection AddTmdbServices(this IServiceCollection services, IConfiguration configuration)
{
        // Configure TMDb settings
        services.Configure<TmdbConfiguration>(configuration.GetSection(TmdbConfiguration.SectionName));

    // Register HttpClient for TmdbClient with Polly retry policy
   services.AddHttpClient<ITmdbClient, TmdbClient>()
  .AddPolicyHandler(GetRetryPolicy())
         .AddPolicyHandler(GetCircuitBreakerPolicy());

 // Register TitleFactsService
  services.AddScoped<ITitleFactsService, TitleFactsService>();

        // Register IMemoryCache if not already registered
        services.AddMemoryCache();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
 .HandleTransientHttpError()
      .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
    retryCount: 3,
    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
 onRetry: (outcome, timespan, retryAttempt, context) =>
    {
   // Log retry attempt if needed
      });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
         handledEventsAllowedBeforeBreaking: 5,
         durationOfBreak: TimeSpan.FromSeconds(30));
    }
}
