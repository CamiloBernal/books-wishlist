namespace BooksWishlist.Application.Exceptions;

public class DuplicatedBookInListException : Exception
{
    public DuplicatedBookInListException(string message) : base(message)
    {
    }
}
