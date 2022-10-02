namespace BooksWishlist.Presentation.Modules;

public static class LoggingModule
{
    public static IServiceCollection ConfigureLogger(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        services.AddWatchDogServices(opt => opt.IsAutoClear = true);
        //Logger
        using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        var logger = loggerFactory.CreateLogger<LoggerService>();
        builder.Services.AddSingleton(typeof(ILogger), logger);
        return services;
    }
}
