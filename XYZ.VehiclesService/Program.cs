using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using XYZ.VehiclesService.Application.Interfaces;
using XYZ.VehiclesService.Application.Services;
using XYZ.VehiclesService.Infrastructure.Persistence;
using XYZ.VehiclesService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<VehiclesDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
);

builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<VehicleRepository>();
builder.Services.AddScoped<VehicleTypeRepository>();
builder.Services.AddGrpc();

// Add gRPC reflection to allow tools like grpcurl to discover services
builder.Services.AddGrpcReflection();

// Configure Kestrel for HTTP/2 (required for gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo =>
        lo.Protocols = HttpProtocols.Http2
    );
});

var app = builder.Build();

app.MapGrpcService<XYZ.VehiclesService.Controllers.VehiclesGrpcService>();

// Map reflection service in Development only
if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VehiclesDbContext>();
    // Apply migrations with retry since the database container may not be immediately available
    var maxAttempts = 10;
    var delayMs = 2000;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            db.Database.Migrate();
            SeedData.Initialize(db);
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
