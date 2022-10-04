namespace BooksWishlist.Application.Exceptions;

public class BookNotFoundInListException : Exception
{
    public BookNotFoundInListException() : base("The book was not found in the collection.")
    {
    }
}
