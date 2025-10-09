using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using XYZ.AuthService.Application.Interfaces;
using XYZ.AuthService.Application.Services;
using XYZ.AuthService.Infrastructure.Persistence;
using XYZ.AuthService.Infrastructure.Repositories;
using XYZ.AuthService.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para gRPC
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

// Configuración de logging con Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// Base de datos
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Servicios de autorización y autenticación
builder.Services.AddAuthorization();

// Inyección de dependencias
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddGrpc().AddServiceOptions<XYZ.AuthService.Controllers.AuthGrpcService>(options =>
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

app.MapGet("/", () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db, scope.ServiceProvider.GetRequiredService<PasswordHasher>());
}

app.Run();
