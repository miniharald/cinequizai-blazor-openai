using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;

namespace CineQuizAI.Web.Endpoints
{
    public static class LocalizationEndpoints
    {
        public static IEndpointRouteBuilder MapLocalizationEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/culture/set/{culture}", (string culture, HttpContext http, string? redirectUri) =>
            {
                var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
                http.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    cookieValue,
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

                return Results.Redirect(!string.IsNullOrWhiteSpace(redirectUri) ? redirectUri : "/");
            })
            .WithTags("i18n");

            return app;
        }
    }
}
