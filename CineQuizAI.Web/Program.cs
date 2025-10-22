using CineQuizAI.Infrastructure.Data;
using CineQuizAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using CineQuizAI.Web.Components; // for App component

var builder = WebApplication.CreateBuilder(args);

// Logging (Serilog) — TODO: move to config later
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

// Blazor Server
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Root component
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
