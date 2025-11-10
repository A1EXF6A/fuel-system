using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using XYZ.AuthService.Application.Interfaces;
using XYZ.AuthService.Application.Services;
using XYZ.AuthService.Infrastructure.Persistence;
using XYZ.AuthService.Infrastructure.Repositories;
using XYZ.AuthService.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para gRPC (HTTP/2)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo =>
        lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2
    );
});

// Configuración de logging con Serilog
builder.Host.UseSerilog(
    (ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

string conectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

// Base de datos
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(conectionString).UseSnakeCaseNamingConvention()
);

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

// Servicios de autorización y autenticación
builder.Services.AddAuthorization();

// Inyección de dependencias
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddSingleton<PasswordHasher>();

builder
    .Services.AddGrpc()
    .AddServiceOptions<XYZ.AuthService.Controllers.AuthGrpcService>(options =>
    {
        options.EnableDetailedErrors = true;
    });
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<XYZ.AuthService.Controllers.AuthGrpcService>();
app.MapGrpcReflectionService();

app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    // Apply database operations with retry since the database container may not be immediately available
    var maxAttempts = 10;
    var delayMs = 2000;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
            SeedData.Initialize(db, scope.ServiceProvider.GetRequiredService<PasswordHasher>());
            break;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(
                ex,
                "Attempt {Attempt} of {MaxAttempts} setting up database failed.",
                attempt,
                maxAttempts
            );
            if (attempt == maxAttempts)
            {
                app.Logger.LogError(ex, "Database setup failed after {MaxAttempts} attempts.");
                throw;
            }
            System.Threading.Thread.Sleep(delayMs);
        }
    }
}

app.Run();
