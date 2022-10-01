using BooksWishlist.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var issuer = builder.Configuration["Issuer"];
var audience = builder.Configuration["Audience"];
var signingKey = builder.Configuration["SigningKey"];

builder.Services.AddEndpointsApiExplorer()
    .ConfigureApiVersioning()
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureAuthentication(issuer, audience, signingKey)
    .AddEndpointsApiExplorer()
    .ConfigureOpenApi()
    .AddWatchDogServices(opt => opt.IsAutoClear = true)
    .AddModelValidators()
    .AddHealthChecks();
//IoC
builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();


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

app.MapGet("/health", async (HealthCheckService healthCheckService) =>
{
    WatchLogger.Log($"Health check validation at: {DateTime.UtcNow} (UTC)");
    var report = await healthCheckService.CheckHealthAsync();
    return report.Status == HealthStatus.Healthy
        ? Results.Ok(report)
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
}).WithTags("Health").Produces(200).ProducesProblem(503).ProducesProblem(401);

app.MapSecurityEndpoints();

//Run The App
WatchLogger.Log("...Starting Host...");
app.Run();
