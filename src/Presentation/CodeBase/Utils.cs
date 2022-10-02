namespace BooksWishlist.Presentation.CodeBase;

public static class Utils
{
    public static IResult BuildBadRequestResult(string title, string details)
    {
        var problemDetails = new ProblemDetails
        {
            Type = Constants.BadResponseType, Title = title, Detail = details, Status = 400
        };
        return Results.BadRequest(problemDetails);
    }
}
