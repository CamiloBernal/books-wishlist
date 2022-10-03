namespace BooksWishlist.Presentation.ViewModels;

public class WishlistDtoValidator : AbstractValidator<WishlistDto>
{
    public WishlistDtoValidator() =>
        RuleFor(l => l.Name)
            .NotEmpty()
            .WithMessage("The wish list must have a valid name");
}
