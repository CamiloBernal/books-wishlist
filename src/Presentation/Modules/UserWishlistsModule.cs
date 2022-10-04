// ReSharper disable PossibleMultipleEnumeration

namespace BooksWishlist.Presentation.Modules;

public static class UserWishlistsModule
{
    public static IEndpointRouteBuilder MapUserWishlistsEndpoints(this IEndpointRouteBuilder routes)
    {
        MapCreateWishListEndpoint(routes);
        MapListWishlistsEndpoint(routes);
        MapDeleteWishlistsEndpoint(routes);
        MapAddBookToWishListEndpoint(routes);
        MapEditWishListEndpoint(routes);
        MapRemoveBookToWishListEndpoint(routes);
        return routes;
    }

    private static void MapRemoveBookToWishListEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapDelete("/wishlists/{listName?}/books/{bookId?}", [Authorize] async (
                IUserWishlistsRepository wishlistsRepository,
                IGoogleBooksService booksService,
                ILoggerService log, HttpContext ctx, [FromRoute] string? listName,
                [FromRoute] string? bookId,
                CancellationToken cancellationToken) =>
            {
                if (bookId is null)
                {
                    return Utils.BuildBadRequestResult("Book Id for remove not provided",
                        "You must send the id of the book to remove from the list");
                }

                if (listName == null)
                {
                    return Utils.BuildBadRequestResult("Wishlist name not provided",
                        "You must specify the name of the list that you want to delete.");
                }

                try
                {
                    var currentUser = ctx.User.Identity?.Name;
                    if (currentUser is null)
                    {
                        return Results.Unauthorized();
                    }

                    _ = await wishlistsRepository.RemoveBooksAsync(listName, bookId, currentUser, cancellationToken);
                    return Results.NoContent();
                }
                catch (InvalidBookReferenceException invalidBookReferenceException)
                {
                    log.LogError(invalidBookReferenceException.Message, invalidBookReferenceException);
                    return Utils.BuildBadRequestResult("Invalid Book Reference", invalidBookReferenceException.Message);
                }
                catch (WishListNotFoundException notFoundException)
                {
                    log.LogError(notFoundException, notFoundException.Message);
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Wishlist not found",
                        Detail =
                            "The specified Wishlist was not found in the database or is not associated with the current user",
                        Status = 404
                    });
                }
                catch (DuplicatedBookInListException duplicatedBookException)
                {
                    log.LogError(duplicatedBookException.Message, duplicatedBookException);
                    return Results.Conflict(new ProblemDetails
                    {
                        Status = 409,
                        Detail = duplicatedBookException.Message,
                        Title = "Book duplicated in wishlist",
                        Type = Constants.ConflictResponseType
                    });
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, title: "Error creating the wishlist");
                }
            })
            .WithName("/wishlists/books/delete")
            .WithTags("Business Endpoints")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500)
            .RequireAuthorization();


    private static void MapAddBookToWishListEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapPut("/wishlists/{listName?}/books/", [Authorize] async (IUserWishlistsRepository wishlistsRepository,
                IGoogleBooksService booksService,
                ILoggerService log, HttpContext ctx, [FromRoute] string? listName,
                [FromBody] IEnumerable<string>? booksList,
                [FromQuery] string? apiKey,
                CancellationToken cancellationToken) =>
            {
                if (apiKey is null)
                {
                    return Utils.BuildBadRequestResult("Query ApiKey not provided",
                        "You must provide your query ApiKey for the Google Books service");
                }

                if (listName == null)
                {
                    return Utils.BuildBadRequestResult("Wishlist name not provided",
                        "You must specify the name of the list that you want to delete. ex: /wishlists/{list_to_delete}");
                }

                if (booksList is null || !booksList.Any())
                {
                    return Utils.BuildBadRequestResult("Wishlist book list empty or not provided",
                        "The list of books to add to your wishlist is empty or was not provided.");
                }

                try
                {
                    var currentUser = ctx.User.Identity?.Name;
                    if (currentUser is null)
                    {
                        return Results.Unauthorized();
                    }

                    var uniqueBooks = booksList.ToHashSet();

                    var validBooks =
                        await ValidateAndBindBookIdListAsync(booksService, uniqueBooks, apiKey, cancellationToken)!;

                    _ = await wishlistsRepository.AddBooksAsync(listName, validBooks, currentUser, cancellationToken);
                    return Results.Accepted("/wishlists/{listName?}/books/", validBooks);
                }
                catch (InvalidBookReferenceException invalidBookReferenceException)
                {
                    log.LogError(invalidBookReferenceException.Message, invalidBookReferenceException);
                    return Utils.BuildBadRequestResult("Invalid Book Reference", invalidBookReferenceException.Message);
                }
                catch (WishListNotFoundException notFoundException)
                {
                    log.LogError(notFoundException, notFoundException.Message);
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Wishlist not found",
                        Detail =
                            "The specified Wishlist was not found in the database or is not associated with the current user",
                        Status = 404
                    });
                }
                catch (DuplicatedBookInListException duplicatedBookException)
                {
                    log.LogError(duplicatedBookException.Message, duplicatedBookException);
                    return Results.Conflict(new ProblemDetails
                    {
                        Status = 409,
                        Detail = duplicatedBookException.Message,
                        Title = "Book duplicated in wishlist",
                        Type = Constants.ConflictResponseType
                    });
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, title: "Error creating the wishlist");
                }
            })
            .WithName("/wishlists/books/put")
            .WithTags("Business Endpoints")
            .Produces(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500)
            .RequireAuthorization();


    private static void MapListWishlistsEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapGet("/wishlists", [Authorize] async (HttpContext ctx, IUserWishlistsRepository wishlistsRepository,
                ILoggerService log,
                CancellationToken cancellationToken) =>
            {
                var currentUser = ctx.User.Identity?.Name;
                if (currentUser is null)
                {
                    return Results.Unauthorized();
                }

                try
                {
                    var userLists = await wishlistsRepository.FindByOwnerAsync(currentUser, cancellationToken);
                    return Results.Ok(userLists);
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, title: "Error creating the wishlist");
                }
            })
            .WithName("wishlists")
            .WithTags("Business Endpoints")
            .Produces<IEnumerable<UserWishlists>>()
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500)
            .RequireAuthorization();


    private static void MapDeleteWishlistsEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapDelete("/wishlists/{listName?}", [Authorize] async (HttpContext ctx,
                IUserWishlistsRepository wishlistsRepository, ILoggerService log, [FromRoute] string? listName,
                CancellationToken cancellationToken) =>
            {
                if (listName == null)
                {
                    return Utils.BuildBadRequestResult("Wishlist name not provided",
                        "You must specify the name of the list that you want to delete. ex: /wishlists/{list_to_delete}");
                }

                var currentUser = ctx.User.Identity?.Name;
                if (currentUser is null)
                {
                    return Results.Unauthorized();
                }

                try
                {
                    _ = await wishlistsRepository.DeleteAsync(listName, currentUser, cancellationToken);
                    return Results.NoContent();
                }
                catch (WishListNotFoundException notFoundException)
                {
                    log.LogError(notFoundException, notFoundException.Message);
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Wishlist not found",
                        Detail =
                            "The specified Wishlist was not found in the database or is not associated with the current user",
                        Status = 404
                    });
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, e);
                    return Results.Problem(e.Message, title: "Error creating the wishlist");
                }
            })
            .WithName("wishlists/delete")
            .WithTags("Business Endpoints")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500)
            .RequireAuthorization();


    private static void MapCreateWishListEndpoint(IEndpointRouteBuilder routes) =>
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
                    var createdList = await wishlistsRepository.CreateAsync(userWishList, cancellationToken);
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


    private static void MapEditWishListEndpoint(IEndpointRouteBuilder routes) =>
        routes.MapPut("/wishlists/{listName?}", [Authorize] async (IUserWishlistsRepository wishlistsRepository,
                IGoogleBooksService booksService,
                ILoggerService log, HttpContext ctx,
                [FromRoute] string? listName,
                [FromBody] WishlistDto wishlist,
                [FromQuery] string? apiKey,
                IValidator<WishlistDto> wishlistsDtoValidator,
                CancellationToken cancellationToken) =>
            {
                if (apiKey is null)
                {
                    return Utils.BuildBadRequestResult("Query ApiKey not provided",
                        "You must provide your query ApiKey for the Google Books service");
                }

                if (listName == null)
                {
                    return Utils.BuildBadRequestResult("Wishlist name not provided",
                        "You must specify the name of the list that you want to delete. ex: /wishlists/{list_to_delete}");
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
                    var validBooks =
                        await ValidateAndBindBookIdListAsync(booksService, wishlist.Books, apiKey, cancellationToken)!;
                    userWishList.Books = validBooks;
                    var updatedList =
                        await wishlistsRepository.UpdateAsync(userWishList, listName, currentUser, cancellationToken);
                    return Results.Created("/wishlist", updatedList);
                }
                catch (WishListNotFoundException notFoundException)
                {
                    log.LogError(notFoundException, notFoundException.Message);
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Wishlist not found",
                        Detail =
                            "The specified Wishlist was not found in the database or is not associated with the current user",
                        Status = 404
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
            .WithName("/wishlists/put")
            .WithTags("Business Endpoints")
            .Produces<UserWishlists>(202)
            .ProducesProblem(400)
            .ProducesProblem(409)
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
