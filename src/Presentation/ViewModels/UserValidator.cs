namespace BooksWishlist.Presentation.ViewModels;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage(
                "Only letters and numbers are allowed for the username. Special characters (including space) are not valid") //Only letters and numbers allowed, no spaces.
            .Length(5, 15).WithMessage("The allowed length for the username is between 5 and 15 characters")
            .NotEqual("admin");

        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(u => u.Password)
            .NotEmpty()
            .Must(HasValidPassword)
            .WithMessage(
                "The password must contain alphanumeric characters 1 uppercase, 1 lowercase and a special character.")
            .Length(5, 15).WithMessage("The length for the password must be between 5 and 10 characters.");
    }

    private static bool HasValidPassword(string pw)
    {
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");
        return lowercase.IsMatch(pw) && uppercase.IsMatch(pw) && digit.IsMatch(pw) && symbol.IsMatch(pw);
    }
}
