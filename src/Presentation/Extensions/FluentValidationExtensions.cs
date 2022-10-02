using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BooksWishlist.Presentation.Extensions;

public static class FluentValidationExtensions
{
    private static string JoinErrors(this IEnumerable<ValidationFailure> errors) => string.Join(Environment.NewLine,
        errors.Select(e => $"Error on: {e.PropertyName}: {e.ErrorMessage}"));

    public static async Task<IResult?> HandleModelValidationAsync<T>(this IValidator<T> validator, T instance,
        ILoggerService log, CancellationToken cancellationToken = default)
    {
        var instanceValidationResult = await validator.ValidateAsync(instance, cancellationToken);
        if (instanceValidationResult.IsValid)
        {
            return null;
        }

        var errors = instanceValidationResult.Errors.JoinErrors();
        var logMessage = $"Errors were found in the request. {errors}";
        log.LogWarning(logMessage);
        return Results.BadRequest(new ProblemDetails
        {
            Type = Constants.BadResponseType, Status = 400, Detail = logMessage, Title = "Invalid request"
        });
    }
}
