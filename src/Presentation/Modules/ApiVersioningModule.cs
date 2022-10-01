namespace BooksWishlist.Presentation.Modules;

public static class ApiVersioningModule
{
    public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        _ = services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });
        return services;
    }
}
