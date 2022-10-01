namespace BooksWishlist.Presentation.Modules;

using Filters;

public static class OpenApiModule
{
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        _ = services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Description = "A simple coding challenge for MELI Assessment",
                    Title = "Books Wishlist API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Camilo Bernal", Url = new Uri("https://www.camilobernal.dev")
                    }
                });

            setup.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

            setup.OperationFilter<AddAuthorizationHeaderOperationFilter>();
            setup.OperationFilter<AddVersionHeaderOperationFilter>();
        });
        return services;
    }
}
