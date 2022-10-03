namespace BooksWishlist.Application.UserWishlists.Entities;

public class UserWishlists
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OwnerId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public IEnumerable<string>? Tags { get; set; } = new List<string>();

    public IEnumerable<Book>? Books { get; set; } = new List<Book>();
}
