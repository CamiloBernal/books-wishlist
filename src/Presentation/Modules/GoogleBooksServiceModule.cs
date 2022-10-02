using BooksWishlist.Application.Exceptions;
using BooksWishlist.Infrastructure.CodeBase;
using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;
using Constants = BooksWishlist.Presentation.CodeBase.Constants;

namespace BooksWishlist.Presentation.Modules;

public static class GoogleBooksServiceModule
{
    public static IEndpointRouteBuilder MapBooksServiceEndpoints(this IEndpointRouteBuilder routes)
    {
        MapRequestBookEndpoint(routes);
        return routes;
    }

    private static void MapRequestBookEndpoint(IEndpointRouteBuilder routes) => routes.MapGet(
            "/books/{queryType?}/{term?}/{q?}",
            [Authorize] async (IGoogleBooksService booksService, ILoggerService log, [FromRoute] string? queryType,
                [FromRoute] string? term, [FromQuery] string? q, [FromQuery] string? apiKey,
                CancellationToken cancellationToken) =>
            {
                if (apiKey is null)
                    return Utils.BuildBadRequestResult("Query ApiKey not provided",
                        "You must provide your query ApiKey for the Google Books service");
                if (string.IsNullOrEmpty(q))
                    return Utils.BuildBadRequestResult("No search criteria provided",
                        "You must provide a search criteria using the url parameter 'q'");
                if (string.IsNullOrEmpty(term))
                    return Utils.BuildBadRequestResult("No search term provided",
                        "You must specify a search term. For example, if you want to search by author, try using '/author/{term}/q?{criteria}'");
                var searchType = booksService.ParseSearchType(queryType);
                try
                {
                    var queryResults =
                        await QueryToService(booksService, q, apiKey, searchType, term, cancellationToken);
                    return Results.Ok(queryResults);
                }
                catch (GoogleServiceBadRequestException badRequestException)
                {
                    log.LogError(badRequestException.Message, badRequestException);
                    return Utils.BuildBadRequestResult("Error querying the Google Books api",
                        badRequestException.Message);
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, statusCode: 500, title: "Error querying the Google Books api");
                }
            })
        .WithTags("Google books services")
        .WithName("/books/")
        .Produces<GoogleBooksSearchResults?>()
        .ProducesProblem(400)
        .ProducesProblem(401)
        .ProducesProblem(500)
        .RequireAuthorization();


    private static async Task<GoogleBooksSearchResults?> QueryToService(IGoogleBooksService booksService, string q,
        string apiKey, BookSearchType? searchType, string? additionalTerm = "",
        CancellationToken cancellationToken = default)
    {
        var results = await booksService.Find(q, apiKey, searchType, additionalTerm, cancellationToken);
        return results;
    }
}
