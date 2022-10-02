namespace BooksWishlist.Presentation.ViewModels;

public record TokenRequestDto
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public static implicit operator User(TokenRequestDto token) =>
        new() { UserName = token.UserName, Password = token.Password };
}
