namespace BooksWishlist.Application.Exceptions;

public class WishListNotFoundException : Exception
{
    public WishListNotFoundException() : base("The specified Wishlist was not found in the database")
    {
    }
}
