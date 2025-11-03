using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using CineQuizAI.Infrastructure.Identity;
using CineQuizAI.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineQuizAI.Web.Endpoints
{
    // TODO: add more auth endpoints here
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("/api"); // TODO: add versioning

            // Issue JWT (username or email)
            api.MapPost("/auth/token", async (
                UserManager<AppUser> users,
                SignInManager<AppUser> signIn,
                ITokenService tokens,
                LoginDto dto) =>
            {
                // Try username
                var user = await users.FindByNameAsync(dto.UserNameOrEmail);

                // Fallback: try email -> resolve username
                if (user is null)
                    user = await users.FindByEmailAsync(dto.UserNameOrEmail);

                if (user is null) return Results.Unauthorized();

                var ok = await signIn.CheckPasswordSignInAsync(
                    user, dto.Password, lockoutOnFailure: false);

                if (!ok.Succeeded) return Results.Unauthorized();

                var jwt = tokens.CreateToken(user.Id, user.UserName!, roles: null); // TODO: roles
                return Results.Ok(new { access_token = jwt });
            })
            .AllowAnonymous()
            .WithTags("auth");

            // Register new user (cookie-based auth)
            api.MapPost("/auth/register", async (
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                RegisterDto dto) =>
            {
                // Validate passwords match
                if (dto.Password != dto.ConfirmPassword)
                    return Results.BadRequest(new RegisterErrorResponse { Error = "PasswordMismatch" });

                // Check if username is taken
                if (await userManager.FindByNameAsync(dto.UserName) is not null)
                    return Results.BadRequest(new RegisterErrorResponse { Error = "UsernameTaken" });

                // Check if email is in use
                if (await userManager.FindByEmailAsync(dto.Email) is not null)
                    return Results.BadRequest(new RegisterErrorResponse { Error = "EmailInUse" });

                // Create user
                var user = new AppUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email
                };

                var result = await userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return Results.BadRequest(new RegisterErrorResponse
                    {
                        Error = "CreationFailed",
                        Details = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Sign in the user
                await signInManager.SignInAsync(user, isPersistent: true);

                return Results.Ok(new RegisterSuccessResponse { RedirectUrl = dto.ReturnUrl ?? "/welcome" });
            })
            .AllowAnonymous()
            .WithTags("auth");

            // Cookie sign-out
            api.MapPost("/auth/logout", async (SignInManager<AppUser> signIn, HttpContext ctx) =>
            {
                await signIn.SignOutAsync();
                return Results.Redirect("/signed-out");
            })
            .RequireAuthorization()
            .WithTags("auth");

            return app;
        }

        // TODO: expand DTO when adding MFA/remember-me
        public record LoginDto(string UserNameOrEmail, string Password);

        public record RegisterDto(
            string UserName,
            string Email,
            string Password,
            string ConfirmPassword,
            string? ReturnUrl);

        public record RegisterErrorResponse
        {
            public string Error { get; set; } = string.Empty;
            public List<string>? Details { get; set; }
        }

        public record RegisterSuccessResponse
        {
            public string RedirectUrl { get; set; } = string.Empty;
        }
    }
}
