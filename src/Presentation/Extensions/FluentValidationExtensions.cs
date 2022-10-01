using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BooksWishlist.Presentation.Extensions;

public static class FluentValidationExtensions
{
    public static string JoinErrors(this IEnumerable<ValidationFailure> errors)
    {
        return string.Join(Environment.NewLine, errors.Select(e => $"Error on: {e.PropertyName}: {e.ErrorMessage}"));
    }
}
