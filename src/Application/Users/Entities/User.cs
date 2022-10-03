namespace BooksWishlist.Application.Users.Entities;

public class User
{
    [OpenApiIgnoreMember] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
}
