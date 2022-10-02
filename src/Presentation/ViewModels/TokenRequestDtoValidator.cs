namespace BooksWishlist.Presentation.ViewModels;

public class TokenRequestDtoValidator : AbstractValidator<TokenRequestDto>
{
    public TokenRequestDtoValidator()
    {
        RuleFor(t => t.UserName)
            .NotEmpty()
            .WithMessage("Username cannot be null for token generation");

        RuleFor(t => t.Password)
            .NotEmpty()
            .WithMessage("Password cannot be null for token generation");
    }
}
