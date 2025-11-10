using CineQuizAI.Application.Abstractions.AI;
using CineQuizAI.Infrastructure.ExternalServices.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CineQuizAI.Infrastructure.DependencyInjection;

/// <summary>
/// Dependency injection configuration for OpenAI services
/// </summary>
public static class OpenAIServiceRegistration
{
    public static IServiceCollection AddOpenAIServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register OpenAI configuration
        services.Configure<OpenAIConfiguration>(
            configuration.GetSection("OpenAI"));

        // Register question generator service
        services.AddScoped<IQuestionGeneratorService, OpenAIQuestionGeneratorService>();

        return services;
    }
}
