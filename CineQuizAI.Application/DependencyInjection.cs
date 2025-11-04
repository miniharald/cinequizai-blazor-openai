using CineQuizAI.Application.Features.Quiz.Handlers;
using CineQuizAI.Application.Features.UserPreferences.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CineQuizAI.Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register handlers
        services.AddScoped<GetUserPreferenceHandler>();
 services.AddScoped<UpdateUserPreferenceHandler>();
  services.AddScoped<StartQuizHandler>();

 return services;
    }
}
