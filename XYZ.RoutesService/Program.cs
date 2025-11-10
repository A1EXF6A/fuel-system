using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using XYZ.RoutesService.Application.Interfaces;
using XYZ.RoutesService.Application.Services;
using XYZ.RoutesService.Infrastructure.Persistence;
using XYZ.RoutesService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<RoutesDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
);

builder.Services.AddHttpClient(
    "GeoClient",
    client =>
    {
        client.DefaultRequestHeaders.Add(
            "User-Agent",
            "XYZ.RoutesService/1.0 (stevan.henao@gmail.com)"
        );
        client.DefaultRequestHeaders.Add("Accept-Language", "es");
    }
);

builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<RouteRepository>();
builder.Services.AddScoped<GeocodingService>();
builder.Services.AddGrpc();

// Add gRPC reflection to help debugging and allow tools like grpcurl to discover services
builder.Services.AddGrpcReflection();
builder.Services.AddHttpClient();

// Configure Kestrel for HTTP/2 (required for gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo =>
        lo.Protocols = HttpProtocols.Http2
    );
});

var app = builder.Build();

app.MapGrpcService<XYZ.RoutesService.Controllers.RoutesGrpcService>();
if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoutesDbContext>();
    // Apply migrations with retry since the database container may not be immediately available
    var maxAttempts = 10;
    var delayMs = 2000;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(
                ex,
                "Attempt {Attempt} of {MaxAttempts} applying migrations failed.",
                attempt,
                maxAttempts
            );
            if (attempt == maxAttempts)
            {
                app.Logger.LogError(ex, "Migrations failed after {MaxAttempts} attempts.");
                throw;
            }
            System.Threading.Thread.Sleep(delayMs);
        }
    }
}

app.Run();
