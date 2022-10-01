namespace BooksWishlist.Presentation.Modules;

public static class AuthenticationModule
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
}
