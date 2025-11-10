using System.Globalization;
using System.IO;

using CineQuizAI.Application;
using CineQuizAI.Application.Abstractions.Security;
using CineQuizAI.Infrastructure.Data;
using CineQuizAI.Infrastructure.DependencyInjection;
using CineQuizAI.Infrastructure.Identity;
using CineQuizAI.Infrastructure.Services;
using CineQuizAI.Web.Components;
using CineQuizAI.Web.Endpoints;
using CineQuizAI.Web.Localization.JsonLocalization;
using CineQuizAI.Web;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

using Serilog;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Register IMemoryCache for localization and other services
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Logging (Serilog)
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
  .WriteTo.Console());

// DbContext (PostgreSQL) - Configure Npgsql with EnableDynamicJson for JSONB support
var connStr = builder.Configuration.GetConnectionString("Default")
         ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");

// Create NpgsqlDataSourceBuilder with EnableDynamicJson for JSONB serialization
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStr);
dataSourceBuilder.EnableDynamicJson(); // Enable System.Text.Json for JSONB columns
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(dataSource));

// Identity (GUID keys)
builder.Services
    .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
      options.Password.RequiredLength = 6;
    })
  .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Ställ in custom login-path för Identity/Blazor
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/login"; // eller egen AccessDenied-sida
});

// Token service
builder.Services.AddScoped<ITokenService, TokenService>();

// Application services
builder.Services.AddApplicationServices();

// Infrastructure - Repositories
builder.Services.AddRepositories();

// TMDb integration
builder.Services.AddTmdbServices(builder.Configuration);

// OpenAI integration
builder.Services.AddOpenAIServices(builder.Configuration);

// Blazor Server med Authentication
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// VIKTIGT: Registrera AuthenticationStateProvider för Blazor Server
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

// Lägg till HttpClientFactory för DI
builder.Services.AddHttpClient();

// RequestLocalization (sv, en)
var supportedCultures = new[] { new CultureInfo("sv"), new CultureInfo("en") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("sv");
    options.SupportedCultures = supportedCultures;
options.SupportedUICultures = supportedCultures;

    // Culture resolution order: query -> cookie -> Accept-Language
 options.RequestCultureProviders = new IRequestCultureProvider[]
    {
 new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
new AcceptLanguageHeaderRequestCultureProvider()
 };

    options.ApplyCurrentCultureToResponseHeaders = true;
});

// JSON localization registration
builder.Services.Configure<JsonLocalizationOptions>(opt =>
{
    // Folder where strings.en.json / strings.sv.json live
    opt.ResourcesPath = Path.Combine(builder.Environment.ContentRootPath, "Localization");
 opt.FileName = "strings"; // results in strings.en.json, strings.sv.json
});

builder.Services.AddSingleton<IJsonStringLocalizerFactory, JsonStringLocalizerFactory>();

// Global IJsonStringLocalizer instance
// If your factory has Create() with no parameters, call Create().
// If it expects one optional argument (e.g., category/scope), pass null positionally.
builder.Services.AddScoped<IJsonStringLocalizer>(sp =>
{
    var factory = sp.GetRequiredService<IJsonStringLocalizerFactory>();
    return factory.Create(string.Empty); // Use empty string instead of null
});

// Enable MVC controllers for attribute routing (needed for AntiforgeryController)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Global exception handler - VIKTIGT FÖR DEBUG
app.Use(async (context, next) =>
{
  try
  {
     await next();
 }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception occurred");
        context.Response.StatusCode = 500;
      await context.Response.WriteAsync($"Error: {ex.Message}\n\nStack: {ex.StackTrace}");
    }
});

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
 app.UseHsts();
}
else
{
    // Development: Visa detaljerade fel
    app.UseDeveloperExceptionPage();
}

// Optional: set process-wide default thread culture at startup
var culture = new CultureInfo("sv");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// RequestLocalization middleware (must run early)
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Add this to serve antiforgery JS
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/js/antiforgery.js")
    {
     context.Response.ContentType = "application/javascript";
  await context.Response.SendFileAsync("wwwroot/js/antiforgery.js");
   return;
    }
await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
 .AddInteractiveServerRenderMode();

app.MapAuthEndpoints();
app.MapLocalizationEndpoints();
app.MapUserPreferenceEndpoints();
app.MapQuizEndpoints();
app.MapDebugEndpoints(); // TMDb config verification

// Enable attribute routing for controllers (needed for AntiforgeryController)
app.MapControllers();

app.Run();
