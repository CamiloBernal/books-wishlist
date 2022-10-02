using BooksWishlist.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);

//Config services
builder.Services.Configure<StoreDatabaseSettings>(builder.Configuration.GetSection("StoreDatabase"))
    .Configure<CryptoServiceSettings>(builder.Configuration.GetSection("CryptoServices"))
    .Configure<TokenGeneratorOptions>(builder.Configuration.GetSection("TokenGeneratorOptions"));


var tokenOptions = builder.Configuration.GetSection("TokenGeneratorOptions").Get<TokenGeneratorOptions>();

builder.Services.AddEndpointsApiExplorer()
    .ConfigureApiVersioning()
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureAuthentication(tokenOptions)
    .AddEndpointsApiExplorer()
    .ConfigureOpenApi()
    .AddWatchDogServices(opt => opt.IsAutoClear = true)
    .AddModelValidators()
    .AddHealthChecks();

//Logger
using (var loggerFactory = LoggerFactory.Create(b => b.AddConsole()))
{
    var logger = loggerFactory.CreateLogger<LoggerService>();
    builder.Services.AddSingleton(typeof(ILogger), logger);
}

//IoC
builder.Services.AddSingleton<ILoggerService, LoggerService>()
    .AddScoped<ISecurityService, SecurityService>();


//App Config
var app = builder.Build();


app.UseWatchDogExceptionLogger()
    .UseHttpsRedirection()
    .UseSwagger()
    .UseSwaggerUI()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseRouting()
    .UseWatchDog(opt =>
    {
        opt.WatchPageUsername = "MELI";
        opt.WatchPagePassword = "MeliUser1234";
    });

if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
}

//Health check Endpoint:

app.MapGet("/health", async (HealthCheckService healthCheckService, CancellationToken cancellationToken) =>
{
    WatchLogger.Log($"Health check validation at: {DateTime.UtcNow} (UTC)");
    var report = await healthCheckService.CheckHealthAsync(cancellationToken);
    return report.Status == HealthStatus.Healthy
        ? Results.Ok(report)
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
}).WithTags("Health").Produces(200).ProducesProblem(503).ProducesProblem(401);

app.MapSecurityEndpoints();

//Run The App
WatchLogger.Log("...Starting Host...");
app.Run();
