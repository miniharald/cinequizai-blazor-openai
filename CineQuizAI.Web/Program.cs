// DI: interface (Application) + impl (Infrastructure)
using CineQuizAI.Application.Abstractions.Security;
using CineQuizAI.Infrastructure.Data;
using CineQuizAI.Infrastructure.Identity;
using CineQuizAI.Infrastructure.Services;
using CineQuizAI.Web.Components; // App component
using CineQuizAI.Web.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Logging (Serilog) — TODO: move to config
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
      .WriteTo.Console());

// DbContext (PostgreSQL)
var connStr = builder.Configuration.GetConnectionString("Default")
              ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

// Identity (GUID keys) — TODO: tune options
builder.Services
    .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Token service via interface — TODO: tune lifetime
builder.Services.AddScoped<ITokenService, TokenService>();

// Blazor Server
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Supported cultures
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("sv") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("sv");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.ApplyCurrentCultureToResponseHeaders = true; // TODO: keep/disable
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true); // TODO: custom error page
    app.UseHsts();
}

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();   // TODO: static assets

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();   // TODO: required for components

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapAuthEndpoints();
app.MapLocalizationEndpoints(); // TODO: map more endpoint groups later

app.Run();
