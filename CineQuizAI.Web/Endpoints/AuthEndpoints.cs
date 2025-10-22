using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using CineQuizAI.Infrastructure.Identity;
using CineQuizAI.Application.Abstractions.Security;

namespace CineQuizAI.Web.Endpoints
{
    // TODO: add more auth endpoints here
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("/api"); // TODO: add versioning

            api.MapPost("/auth/token", async (
                UserManager<AppUser> users,
                SignInManager<AppUser> signIn,
                ITokenService tokens,
                LoginDto dto) =>
            {
                var user = await users.FindByNameAsync(dto.UserName);
                if (user is null) return Results.Unauthorized();

                var ok = await signIn.CheckPasswordSignInAsync(
                    user, dto.Password, lockoutOnFailure: false);
                if (!ok.Succeeded) return Results.Unauthorized();

                var jwt = tokens.CreateToken(user.Id, user.UserName!, roles: null); // TODO: roles
                return Results.Ok(new { access_token = jwt });
            })
            .AllowAnonymous();

            return app;
        }

        public record LoginDto(string UserName, string Password);
    }
}
