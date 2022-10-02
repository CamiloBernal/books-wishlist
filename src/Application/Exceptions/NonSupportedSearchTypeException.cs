namespace BooksWishlist.Application.Exceptions;

public class NonSupportedSearchTypeException:Exception
{
    public NonSupportedSearchTypeException() : base("The search type you are trying to perform is not currently supported.")
    {

    }
}
