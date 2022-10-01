var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var issuer = builder.Configuration["Issuer"];
var audience = builder.Configuration["Audience"];
var signingKey = builder.Configuration["SigningKey"];

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureApiVersioning()
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureAuthentication(issuer, audience, signingKey)
    .AddEndpointsApiExplorer()
    .ConfigureOpenApi();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
}

app.UseSwagger()
    .UseSwaggerUI()
    .UseHttpsRedirection()
    .UseAuthorization()
    .UseAuthentication()
    .UseRouting();


//Health check Endpoint:

app.MapGet("/health", async (HealthCheckService healthCheckService) =>
{
    var report = await healthCheckService.CheckHealthAsync();
    return report.Status == HealthStatus.Healthy
        ? Results.Ok(report)
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
}).WithTags(new[] { "Health" }).Produces(200).ProducesProblem(503).ProducesProblem(401);

//Run The App
app.Run();
