using Microsoft.EntityFrameworkCore;
using Serilog;
using XYZ.DriversService.Application.Interfaces;
using XYZ.DriversService.Application.Services;
using XYZ.DriversService.Controllers;
using XYZ.DriversService.Infrastructure.Persistence;
using XYZ.DriversService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/drivers-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

// Add gRPC reflection
builder.Services.AddGrpcReflection();

// Configure Entity Framework
builder.Services.AddDbContext<DriversDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<DriverRepository>();

// Register application services
builder.Services.AddScoped<IDriverService, DriverService>();

// Configure Kestrel for HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo => lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

// Configure URLs for Docker
if (builder.Environment.EnvironmentName == "Development")
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcService<DriversGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DriversDbContext>();
    
    try
    {
        Log.Information("Ensuring database exists...");
        await context.Database.EnsureCreatedAsync();
        
        Log.Information("Seeding database...");
        await SeedData.SeedAsync(context);
        
        Log.Information("Database setup completed successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while setting up the database");
        throw;
    }
}

try
{
    Log.Information("Starting XYZ.DriversService");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}