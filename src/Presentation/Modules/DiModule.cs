using BooksWishlist.Infrastructure.Databases;

namespace BooksWishlist.Presentation.Modules;

public static class DiModule
{
    public static IServiceCollection ConfigureDiContainer(this IServiceCollection services)
    {
        //DI Containers
        services.AddSingleton<ILoggerService, LoggerService>()
            .AddScoped<ISecurityService, SecurityService>()
            .AddScoped<IGoogleBooksService, GoogleBooksService>()
            .AddScoped<IUserWishlistsRepository, UserWishlistsRepository>();

        return services;
    }
}
