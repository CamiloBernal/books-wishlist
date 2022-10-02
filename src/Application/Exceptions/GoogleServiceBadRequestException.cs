namespace BooksWishlist.Application.Exceptions;

public class GoogleServiceBadRequestException:Exception
{
    public GoogleServiceBadRequestException() : base("The Google API reported an invalid request. Please review the search criteria and/or validate your ApiKey")
    {

    }

}
