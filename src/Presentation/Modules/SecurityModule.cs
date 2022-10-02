using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BooksWishlist.Presentation.Configuration;
using BooksWishlist.Presentation.Extensions;
using BooksWishlist.Presentation.Models;
using Microsoft.Extensions.Options;

namespace BooksWishlist.Presentation.Modules;

public static class SecurityModule
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
        TokenGeneratorOptions tokenOptions)
    {
        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SigningKey))
            });
        return services;
    }


    public static IEndpointRouteBuilder MapSecurityEndpoints(this IEndpointRouteBuilder routes)
    {
        MapUserServiceEndPoint(routes);
        MapRequestTokenEndpoint(routes);
        return routes;
    }

    private static void MapRequestTokenEndpoint(this IEndpointRouteBuilder routes) =>
        routes.MapPost("/auth/token",
                async (ISecurityService securityService,
                    ILoggerService log,
                    [FromBody] TokenRequestDto tokenRequest,
                    IValidator<TokenRequestDto> tokenRequestValidator,
                    IOptions<TokenGeneratorOptions> tokenGeneratorOptions,
                    CancellationToken cancellationToken) =>
                {
                    try
                    {
                        var validationResult =
                            await tokenRequestValidator.HandleModelValidationAsync(tokenRequest, log,
                                cancellationToken);
                        if (validationResult != null)
                        {
                            return validationResult;
                        }

                        var user = (User)tokenRequest;
                        var loggedUser = await securityService.ValidateUser(user, cancellationToken);
                        if (loggedUser is null)
                        {
                            return Results.Unauthorized();
                        }

                        var token = GenerateToken(loggedUser, tokenGeneratorOptions);
                        return Results.Json(token);
                    }
                    catch (Exception e)
                    {
                        log.LogError("Failed to generate the auth token", e);
                        return Results.Problem(e.Message, title: "Failed to generate the auth token");
                    }
                })
            .Produces<AuthTokenResponse>()
            .ProducesProblem(401)
            .ProducesProblem(500)
            .WithTags("Security")
            .WithName("auth/token")
            .WithDisplayName("Request token");

    private static void MapUserServiceEndPoint(IEndpointRouteBuilder routes) =>
        routes.MapPost("/register",
                async (ISecurityService securityService, ILoggerService log, [FromBody] User user,
                    IValidator<User> userValidator, CancellationToken cancellationToken) =>
                {
                    try
                    {
                        var validationResult =
                            await userValidator.HandleModelValidationAsync(user, log, cancellationToken);
                        if (validationResult != null)
                        {
                            return validationResult;
                        }

                        await securityService.RegisterUserAsync(user, cancellationToken);
                        return Results.Created("/register", new { Name = user.UserName, user.Email });
                    }
                    catch (Exception e)
                    {
                        log.LogError("Error creating user", e);
                        return Results.Problem(e.Message, title: "Error creating user");
                    }
                })
            .Produces<User>(201)
            .ProducesProblem(400)
            .ProducesProblem(500)
            .WithTags("Security")
            .WithName("/register")
            .WithDisplayName("User registration");


    private static AuthTokenResponse GenerateToken(User loggedInUser, IOptions<TokenGeneratorOptions> options)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, loggedInUser.UserName!),
            new Claim(JwtRegisteredClaimNames.Name, loggedInUser.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, loggedInUser.Email!)
        };

        var issuer = options.Value.Issuer;
        var audience = options.Value.Audience;
        var signingKey = options.Value.SigningKey;
        var expiresIn = options.Value.MinutesToExpire;

        var token = new JwtSecurityToken
        (
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256)
        );
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthTokenResponse
        {
            token_type = Constants.Bearer, expires_in = options.Value.MinutesToExpire, access_token = generatedToken
        };
    }
}
