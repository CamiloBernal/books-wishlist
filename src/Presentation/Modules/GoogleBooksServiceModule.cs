using BooksWishlist.Infrastructure.CodeBase;
using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

namespace BooksWishlist.Presentation.Modules;

public static class GoogleBooksServiceModule
{
    public static IEndpointRouteBuilder MapBooksServiceEndpoints(this IEndpointRouteBuilder routes)
    {
        MapTypedQueryEndpoint(routes);
        MapFullSearchEndpoint(routes);
        return routes;
    }

    private static void MapFullSearchEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapGet("/books/search/{q?}", [Authorize] async (HttpContext ctx, IGoogleBooksService booksService,
                ILoggerService log, [FromQuery] string? q, [FromQuery] int? page, [FromQuery] string? apiKey,
                CancellationToken cancellationToken) =>
            {
                if (apiKey is null)
                {
                    return Utils.BuildBadRequestResult("Query ApiKey not provided",
                        "You must provide your query ApiKey for the Google Books service");
                }

                if (string.IsNullOrEmpty(q))
                {
                    return Utils.BuildBadRequestResult("No search criteria provided",
                        "You must provide a search criteria using the url parameter 'q'");
                }

                try
                {
                    var queryResults =
                        await QueryToService(booksService, q, apiKey, BookSearchType.FullSearch, string.Empty, page,
                            cancellationToken);
                    return Results.Ok(queryResults);
                }
                catch (GoogleServiceBadRequestException badRequestException)
                {
                    log.LogError(badRequestException.Message, badRequestException);
                    return Utils.BuildBadRequestResult("Error querying the Google Books api",
                        badRequestException.Message);
                }
                catch (NonSupportedSearchTypeException nonSupportedSearchTypeException)
                {
                    log.LogError(nonSupportedSearchTypeException.Message, nonSupportedSearchTypeException);
                    return Utils.BuildBadRequestResult("Invalid query type",
                        nonSupportedSearchTypeException.Message);
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, statusCode: 500, title: "Error querying the Google Books api");
                }
            })
            .WithTags("Google books services")
            .WithName("/books/search")
            .Produces<GoogleBooksSearchResultDto?>()
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500)
            .RequireAuthorization();


    private static void MapTypedQueryEndpoint(IEndpointRouteBuilder routes) => routes.MapGet(
            "/books/{queryType?}/{term?}/{q?}",
            [Authorize] async (IGoogleBooksService booksService, ILoggerService log, [FromRoute] string? queryType,
                [FromRoute] string? term, [FromQuery] string? q, [FromQuery] int? page, [FromQuery] string? apiKey,
                CancellationToken cancellationToken) =>
            {
                if (apiKey is null)
                {
                    return Utils.BuildBadRequestResult("Query ApiKey not provided",
                        "You must provide your query ApiKey for the Google Books service");
                }

                if (string.IsNullOrEmpty(q))
                {
                    return Utils.BuildBadRequestResult("No search criteria provided",
                        "You must provide a search criteria using the url parameter 'q'");
                }

                if (string.IsNullOrEmpty(term))
                {
                    return Utils.BuildBadRequestResult("No search term provided",
                        "You must specify a search term. For example, if you want to search by author, try using '/author/{term}/q?{criteria}'");
                }

                try
                {
                    var searchType = booksService.ParseSearchType(queryType);
                    var queryResults =
                        await QueryToService(booksService, q, apiKey, searchType, term, page, cancellationToken);
                    return Results.Ok(queryResults);
                }
                catch (GoogleServiceBadRequestException badRequestException)
                {
                    log.LogError(badRequestException.Message, badRequestException);
                    return Utils.BuildBadRequestResult("Error querying the Google Books api",
                        badRequestException.Message);
                }
                catch (NonSupportedSearchTypeException nonSupportedSearchTypeException)
                {
                    log.LogError(nonSupportedSearchTypeException.Message, nonSupportedSearchTypeException);
                    return Utils.BuildBadRequestResult("Invalid query type",
                        nonSupportedSearchTypeException.Message);
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, statusCode: 500, title: "Error querying the Google Books api");
                }
            })
        .WithTags("Google books services")
        .WithName("/books/queryType/")
        .Produces<GoogleBooksSearchResultDto?>()
        .ProducesProblem(400)
        .ProducesProblem(401)
        .ProducesProblem(500)
        .RequireAuthorization();


    private static async Task<GoogleBooksSearchResultDto?> QueryToService(IGoogleBooksService booksService, string q,
        string apiKey, BookSearchType? searchType, string? additionalTerm = "", int? page = 0,
        CancellationToken cancellationToken = default) =>
        await booksService.Find(q, apiKey, searchType, additionalTerm, page, cancellationToken);
}
