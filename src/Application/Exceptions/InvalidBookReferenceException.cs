namespace BooksWishlist.Application.Exceptions;

public class InvalidBookReferenceException : Exception
{
    public InvalidBookReferenceException(string message) : base(message)
    {
    }
}
