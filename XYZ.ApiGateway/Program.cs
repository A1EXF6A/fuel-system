using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using XYZ.ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "supersecretkeythatislongenoughforthejwttoken";
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };
    });

// Make HttpContext accessible to services (needed to forward Authorization header)
builder.Services.AddHttpContextAccessor();

// Dependency Injection for Gateway Services
builder.Services.AddScoped<AuthGatewayService>();
builder.Services.AddScoped<DriversGatewayService>();
builder.Services.AddScoped<VehiclesGatewayService>();
builder.Services.AddScoped<RoutesGatewayService>();
builder.Services.AddScoped<FuelGatewayService>();

// Configure URLs - respect --urls parameter and validate port conflicts
ConfigureUrls(builder);

static void ConfigureUrls(WebApplicationBuilder builder)
{
    // Get configured service URLs to check for port conflicts
    var serviceUrls = builder
        .Configuration.GetSection("Services")
        .GetChildren()
        .Select(s => s.Value)
        .Where(url => !string.IsNullOrEmpty(url))
        .ToList();

    // Extract ports from service URLs for validation
    var servicePorts = serviceUrls
        .Select(ExtractPortFromUrl)
        .Where(port => port.HasValue)
        .Select(port => port.Value)
        .ToHashSet();

    // Check if --urls parameter was provided
    var commandLineUrls = builder.Configuration["urls"];

    if (!string.IsNullOrEmpty(commandLineUrls))
    {
        // Validate that URLs from --urls don't conflict with service ports
        var urlsToUse = commandLineUrls.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var url in urlsToUse)
        {
            var port = ExtractPortFromUrl(url);
            if (port.HasValue && servicePorts.Contains(port.Value))
            {
                var conflictingService = serviceUrls.FirstOrDefault(sUrl =>
                    ExtractPortFromUrl(sUrl) == port.Value
                );
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Port conflict detected!");
                Console.WriteLine($"Gateway URL: {url}");
                Console.WriteLine($"Conflicts with service: {conflictingService}");
                Console.WriteLine(
                    $"Available ports: {string.Join(", ", GetAvailablePorts(servicePorts))}"
                );
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        // URLs are valid, use them with Kestrel configuration to ensure they're respected
        builder.WebHost.UseUrls(urlsToUse);

        // Configure Kestrel to only use specified URLs
        builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(ExtractPortFromUrl(urlsToUse[0]) ?? 8080);
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Using URLs from --urls parameter: {string.Join(", ", urlsToUse)}");
        Console.ResetColor();
    }
    else if (builder.Configuration.GetSection("DefaultKestrelEndpoint").Exists())
    {
        // Use default Kestrel configuration from appsettings if available
        var defaultUrl =
            builder.Configuration["DefaultKestrelEndpoint:Url"] ?? "http://localhost:5010";
        builder.WebHost.UseUrls(defaultUrl);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚙️  Using URLs from configuration: {defaultUrl}");
        Console.ResetColor();
    }
    else
    {
        // Default fallback URL only if nothing else is specified
        const string defaultUrl = "http://localhost:5010";
        var defaultPort = ExtractPortFromUrl(defaultUrl);

        if (defaultPort.HasValue && servicePorts.Contains(defaultPort.Value))
        {
            var availablePorts = GetAvailablePorts(servicePorts);
            var newDefaultUrl = $"http://localhost:{availablePorts.First()}";
            builder.WebHost.UseUrls(newDefaultUrl);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                $"⚠️  Default port 5010 conflicts with services. Using: {newDefaultUrl}"
            );
            Console.ResetColor();
        }
        else
        {
            builder.WebHost.UseUrls(defaultUrl);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"🔧 Using default URL: {defaultUrl}");
            Console.ResetColor();
        }
    }
}

static int? ExtractPortFromUrl(string url)
{
    if (string.IsNullOrEmpty(url))
        return null;

    try
    {
        var uri = new Uri(url);
        return uri.Port;
    }
    catch
    {
        return null;
    }
}

static List<int> GetAvailablePorts(HashSet<int> usedPorts)
{
    var commonPorts = new[] { 5010, 5020, 5030, 5040, 5050, 8080, 8081, 8082, 3000, 3001 };
    return commonPorts.Where(port => !usedPorts.Contains(port)).Take(3).ToList();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

