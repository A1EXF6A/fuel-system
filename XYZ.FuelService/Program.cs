using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using XYZ.FuelService.Application.Interfaces;
using XYZ.FuelService.Application.Services;
using XYZ.FuelService.Infrastructure.Persistence;
using XYZ.FuelService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddDbContext<FuelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    options.ConfigureEndpointDefaults(lo => lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

// Configure URLs for Docker in Development
if (builder.Environment.EnvironmentName == "Development")
{
    builder.WebHost.UseUrls("http://0.0.0.0:5006");
}

var app = builder.Build();

// Enable reflection endpoint in Development
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcService<XYZ.FuelService.Controllers.FuelGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FuelDbContext>();
    db.Database.Migrate();
}

app.Run();
