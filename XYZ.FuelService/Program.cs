using Microsoft.EntityFrameworkCore;
using Serilog;
using XYZ.FuelService.Application.Interfaces;
using XYZ.FuelService.Application.Services;
using XYZ.FuelService.Infrastructure.Persistence;
using XYZ.FuelService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<FuelDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
);

builder.Services.AddScoped<IFuelService, FuelService>();
builder.Services.AddScoped<FuelRepository>();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

// Add gRPC reflection (useful for grpcurl during development)
builder.Services.AddGrpcReflection();
builder.Services.AddHttpClient();

// Configure Kestrel for HTTP/2 (required for gRPC over plaintext/h2c)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo =>
        lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2
    );
});

var app = builder.Build();

// Enable reflection endpoint
app.MapGrpcReflectionService();

app.MapGrpcService<XYZ.FuelService.Controllers.FuelGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FuelDbContext>();
    // Apply database operations with retry since the database container may not be immediately available
    var maxAttempts = 10;
    var delayMs = 2000;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
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
