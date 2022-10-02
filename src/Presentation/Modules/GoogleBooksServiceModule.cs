namespace BooksWishlist.Presentation.Modules;

public static class GoogleBooksServiceModule
{
    public static IEndpointRouteBuilder MapBooksServiceEndpoints(this IEndpointRouteBuilder routes)
    {
        MapRequestTokenEndpoint(routes);
        return routes;
    }

    private static void MapRequestTokenEndpoint(IEndpointRouteBuilder routes) => routes.MapGet("/test",
            [Authorize](HttpContext ctx, string? id) =>
            {
                var u = ctx.User.Identity?.Name ?? "Anonymous";
                return Task.FromResult(Results.Ok(new { message = $"Hola {u}" }));
            })
        .WithTags("Testing")
        .WithName("Demo")
        .RequireAuthorization();
}
