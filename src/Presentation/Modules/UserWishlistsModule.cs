using BooksWishlist.Application.Books.Entities;
using BooksWishlist.Application.Exceptions;
using BooksWishlist.Application.UserWishlists.Entities;
using BooksWishlist.Infrastructure.Store;
using BooksWishlist.Presentation.Extensions;

namespace BooksWishlist.Presentation.Modules;

public static class UserWishlistsModule
{
    public static IEndpointRouteBuilder MapUserWishlistsEndpoints(this IEndpointRouteBuilder routes)
    {
        MapWishlistsEndpoints(routes);
        return routes;
    }

    private static void MapWishlistsEndpoints(IEndpointRouteBuilder routes) =>
        routes.MapPost("/wishlists", [Authorize] async (IUserWishlistsRepository wishlistsRepository,
                IGoogleBooksService booksService,
                ILoggerService log, HttpContext ctx, [FromBody] WishlistDto wishlist,
                [FromQuery] string? apiKey,
                IValidator<WishlistDto> wishlistsDtoValidator,
                CancellationToken cancellationToken) =>
            {
                if (apiKey is null)
                {
                    return Utils.BuildBadRequestResult("Query ApiKey not provided",
                        "You must provide your query ApiKey for the Google Books service");
                }

                try
                {
                    var validationResult =
                        await wishlistsDtoValidator.HandleModelValidationAsync(wishlist, log,
                            cancellationToken);
                    if (validationResult != null)
                    {
                        return validationResult;
                    }

                    var currentUser = ctx.User.Identity?.Name;
                    if (currentUser is null)
                    {
                        return Results.Unauthorized();
                    }

                    var userWishList = (UserWishlists)wishlist;
                    userWishList.OwnerId = currentUser;
                    var validBooks =
                        await ValidateAndBindBookIdListAsync(booksService, wishlist.Books, apiKey, cancellationToken)!;
                    userWishList.Books = validBooks;
                    var createdList = await wishlistsRepository.Create(userWishList, cancellationToken);
                    return Results.Created("/wishlist", createdList);
                }
                catch (DuplicateEntityException duplicateUserException)
                {
                    log.LogError(duplicateUserException.Message, duplicateUserException);
                    return Results.Conflict(new ProblemDetails
                    {
                        Type = Constants.ConflictResponseType,
                        Title = "The wishlist is already taken",
                        Status = 409,
                        Detail = duplicateUserException.Message
                    });
                }
                catch (InvalidBookReferenceException invalidBookReferenceException)
                {
                    log.LogError(invalidBookReferenceException.Message, invalidBookReferenceException);
                    return Utils.BuildBadRequestResult("Invalid Book Reference", invalidBookReferenceException.Message);
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, title: "Error creating the wishlist");
                }
            })
            .WithName("/wishlists")
            .WithTags("Business Endpoints")
            .Produces(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500)
            .RequireAuthorization();


    private static async Task<IEnumerable<Book?>?>? ValidateAndBindBookIdListAsync(IGoogleBooksService booksService,
        IEnumerable<string>? booksIds, string apiKey, CancellationToken cancellationToken = default)
    {
        var validBooks = await booksService.ValidateAndBindWishListBooksAsync(booksIds, apiKey, cancellationToken)!;
        return validBooks;
    }
}
