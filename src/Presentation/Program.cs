var builder = WebApplication.CreateBuilder(args);

var tokenOptions = builder.Configuration.GetSection("TokenGeneratorOptions").Get<TokenGeneratorOptions>();

builder.Services.AddEndpointsApiExplorer()
    .ConfigureBusinessServiceOptions(builder)
    .ConfigureApiVersioning()
    .AddHttpContextAccessor()
    .ConfigureLogger(builder)
    .ConfigureAuthentication(tokenOptions)
    .AddEndpointsApiExplorer()
    .ConfigureOpenApi()
    .AddModelValidators()
    .ConfigureDiContainer()
    .AddHealthChecks();


//App Config
var app = builder.Build();
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

//Map Business EndPoints
app.MapSecurityEndpoints()
    .MapBooksServiceEndpoints()
    .MapUserWishlistsEndpoints();


app.UseWatchDogExceptionLogger()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseWatchDog(opt =>
    {
        opt.WatchPageUsername = "MELI";
        opt.WatchPagePassword = "MeliUser1234";
    })
    .UseHttpsRedirection()
    .UseSwagger()
    .UseSwaggerUI();

//Run The App
WatchLogger.Log("...Starting Host...");
app.Run();
