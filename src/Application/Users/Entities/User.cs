namespace BooksWishlist.Application.Users.Entities;

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
}
