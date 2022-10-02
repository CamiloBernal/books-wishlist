using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BooksWishlist.Application.Exceptions;
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
        _ = services.AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

    private static void MapRequestTokenEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapPost("/sign-in",
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
                        log.LogInformation($"An authentication token has been generated for user {loggedUser.UserName}");
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
            .WithName("/sign-in")
            .WithDisplayName("Request token");

    private static void MapUserServiceEndPoint(IEndpointRouteBuilder routes) =>
        routes.MapPost("/sign-up",
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
                        return Results.Created("/sign-up", new { Name = user.UserName, user.Email });
                    }
                    catch (DuplicateUserException duplicateUserException)
                    {
                        log.LogError(duplicateUserException.Message, duplicateUserException);
                        return Results.Conflict(new ProblemDetails
                        {
                            Type = Constants.ConflictResponseType,
                            Title = "The username is already taken",
                            Status = 409,
                            Detail = duplicateUserException.Message
                        });
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
            .ProducesProblem(409)
            .WithTags("Security")
            .WithName("/sign-up")
            .WithDisplayName("User registration");


    private static AuthTokenResponse GenerateToken(User loggedInUser, IOptions<TokenGeneratorOptions> options)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, loggedInUser.UserName),
            new(JwtRegisteredClaimNames.Name, loggedInUser.UserName)
        };
        if (!string.IsNullOrEmpty(loggedInUser.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, loggedInUser.Email!));
        }

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
