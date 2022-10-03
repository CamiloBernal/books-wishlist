using BooksWishlist.Application.Exceptions;
using BooksWishlist.Application.UserWishlists.Entities;
using BooksWishlist.Infrastructure.Databases;
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
                ILoggerService log, HttpContext ctx, [FromBody] WishlistDto wishlist,
                IValidator<WishlistDto> wishlistsDtoValidator,
                CancellationToken cancellationToken) =>
            {
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
                    if (currentUser is null) return Results.Unauthorized();
                    var userWishList = (UserWishlists)wishlist;
                    userWishList.OwnerId = currentUser;
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
}
