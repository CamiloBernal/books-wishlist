namespace BooksWishlist.Presentation.Modules;

public static class ModelsValidationModule
{
    public static IServiceCollection AddModelValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<User>, UserValidator>();
        services.AddScoped<IValidator<TokenRequestDto>, TokenRequestDtoValidator>();
        services.AddScoped<IValidator<WishlistDto>, WishlistDtoValidator>();
        return services;
    }
}
