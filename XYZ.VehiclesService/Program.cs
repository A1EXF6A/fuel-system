using Microsoft.EntityFrameworkCore;
using Serilog;
using XYZ.VehiclesService.Application.Interfaces;
using XYZ.VehiclesService.Application.Services;
using XYZ.VehiclesService.Infrastructure.Persistence;
using XYZ.VehiclesService.Infrastructure.Repositories;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddDbContext<VehiclesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<VehicleRepository>();
builder.Services.AddScoped<VehicleTypeRepository>();
builder.Services.AddGrpc();
// Add gRPC reflection to allow tools like grpcurl to discover services
builder.Services.AddGrpcReflection();

// Configure Kestrel endpoints explicitly so gRPC (HTTP/2) runs on container port 5002
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC (HTTP/2) endpoint
    options.ListenAnyIP(5002, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

    // HTTP/1.1 management/health endpoint
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
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
    db.Database.Migrate();
    SeedData.Initialize(db);
}

app.Run();
