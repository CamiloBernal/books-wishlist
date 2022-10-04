namespace BooksWishlist.Application.Extensions;

public static class UserWishListExtensions
{
    public static void Merge(this UserWishlists.Entities.UserWishlists source,
        UserWishlists.Entities.UserWishlists target)
    {
        target.Id = source.Id;
        target.OwnerId = source.OwnerId;
        target.Name = source.Name; //For prevent user change wishlist name
    }
}
