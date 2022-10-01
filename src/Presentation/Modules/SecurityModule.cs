using BooksWishlist.Presentation.Extensions;

namespace BooksWishlist.Presentation.Modules;

public static class SecurityModule
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, string issuer,
        string audience, string signingKey)
    {
        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            });
        return services;
    }


    public static IEndpointRouteBuilder MapSecurityEndpoints(this IEndpointRouteBuilder routes)
    {
        MapUserServiceEndPoint(routes);
        return routes;
    }


    private static void MapUserServiceEndPoint(IEndpointRouteBuilder routes)
    {
        routes.MapPost("/register",
                async (ISecurityService securityService, [FromBody] User user, IValidator<User> userValidator) =>
                {
                    try
                    {
                        var userValidationResult = await userValidator.ValidateAsync(user);
                        if (!userValidationResult.IsValid)
                        {
                            var errors = userValidationResult.Errors.JoinErrors();
                            return Results.BadRequest(new
                            {
                                type = "Invalid request",
                                status = 400,
                                detail = $"Errors were found in the request. {errors}",
                                title = "Invalid request"
                            });
                        }

                        await securityService.RegisterUserAsync(user);
                        return Results.Created("/register", new { Name = user.UserName, user.Email });
                    }
                    catch (Exception e)
                    {
                        return Results.Problem(e.Message, title: "Error creating user");
                    }
                })
            .Produces<User>(201)
            .ProducesProblem(400)
            .ProducesProblem(500)
            .WithTags("Security")
            .WithName("UserRegistration")
            .WithDisplayName("User registration");
    }
}
