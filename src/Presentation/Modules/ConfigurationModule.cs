namespace BooksWishlist.Presentation.Modules;

public static class ConfigurationModule
{
    public static IServiceCollection ConfigureBusinessServiceOptions(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        //Config services
        services.Configure<StoreDatabaseSettings>(builder.Configuration.GetSection("StoreDatabase"))
            .Configure<CryptoServiceSettings>(builder.Configuration.GetSection("CryptoServices"))
            .Configure<TokenGeneratorOptions>(builder.Configuration.GetSection("TokenGeneratorOptions"))
            .Configure<GoogleBooksServiceOptions>(builder.Configuration.GetSection("GoogleServicesConfig"));
        return services;
    }
}
