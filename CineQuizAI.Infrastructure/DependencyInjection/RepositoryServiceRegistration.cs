using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CineQuizAI.Infrastructure.DependencyInjection;

/// <summary>
/// Dependency injection configuration for repositories
/// </summary>
public static class RepositoryServiceRegistration
{
  public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
   services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
services.AddScoped<IQuizSessionRepository, QuizSessionRepository>();
   services.AddScoped<IQuestionSetRepository, QuestionSetRepository>();
        services.AddScoped<IQuizQuestionRepository, QuizQuestionRepository>();

    return services;
  }
}
